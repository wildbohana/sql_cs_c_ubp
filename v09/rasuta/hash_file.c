#include "hash_file.h"

#include <stdio.h>
#include <stdlib.h>

#define OVERFLOW_FILE_NAME "overflow.dat"

int createHashFile(FILE* pFile) 
{
	// calloc inicializuje zauzeti memorijski prostor nulama
	Bucket* emptyContent = calloc(B, sizeof(Bucket));               
	fseek(pFile, 0, SEEK_SET);

	int retVal = fwrite(emptyContent, sizeof(Bucket), B, pFile);
	free(emptyContent);

	return retVal;
}

int saveBucket(FILE* pFile, Bucket* pBucket, int bucketIndex) 
{
	fseek(pFile, bucketIndex * sizeof(Bucket), SEEK_SET);
	int retVal = fwrite(pBucket, sizeof(Bucket), 1, pFile) == 1;

	fflush(pFile);
	return retVal;
}

int readBucket(FILE* pFile, Bucket* pBucket, int bucketIndex) 
{
	fseek(pFile, bucketIndex * sizeof(Bucket), SEEK_SET);
	return fread(pBucket, sizeof(Bucket), 1, pFile) == 1;
}

int readRecordFromSerialFile(FILE* pFile, Record* pRecord) 
{
	return fread(pRecord, sizeof(Record), 1, pFile);
}

int saveRecordToOverflowFile(FILE* pFile, Record* pRecord) 
{
	return fwrite(pRecord, sizeof(Record), 1, pFile);
}

int isBucketFull(Bucket bucket) 
{
	return bucket.records[BUCKET_SIZE - 1].status != EMPTY;
}

int initHashFile(FILE* pFile, FILE* pInputSerialFile) 
{
	if (feof(pFile))
		createHashFile(pFile);

	FILE* pOverflowFile = fopen(OVERFLOW_FILE_NAME, "wb+");
	Record r;

	while (readRecordFromSerialFile(pInputSerialFile, &r)) 
	{
		int h = transformKey(r.key);

		Bucket bucket;
		readBucket(pFile, &bucket, h);
		
		// ukoliko nema mesta u maticnom baketu, slog se smesta u privremenu serijsku datoteku prekoracilaca
		if (isBucketFull(bucket))                             
			saveRecordToOverflowFile(pOverflowFile, &r); 
		else
			insertRecord(pFile, r);
	}

	fclose(pInputSerialFile);
	rewind(pOverflowFile);

	// upis prekoracilaca
	while(readRecordFromSerialFile(pOverflowFile, &r))
		insertRecord(pFile, r);

	// zatvaranje i brisanje privremene serijske datoteke prekoracilaca
	fclose(pOverflowFile);
	remove(OVERFLOW_FILE_NAME); 

	return 0;
}

FindRecordResult findRecord(FILE* pFile, int key) 
{
	int bucketIndex = transformKey(key);		// transformacija kljuca u redni broj baketa
	int initialIndex = bucketIndex;				// redni broj maticnog baketa
	Bucket bucket;
	FindRecordResult retVal;

	retVal.ind1 = 99;			// indikator uspesnosti trazenja
	retVal.ind2 = 0;			// indikator postojanja slobodnih lokacija

	while (retVal.ind1 == 99) 
	{
		int q = 0;		// brojac slogova unutar baketa

		readBucket(pFile, &bucket, bucketIndex);
		retVal.bucket = bucket;
		retVal.bucketIndex = bucketIndex;

		while (q < BUCKET_SIZE && retVal.ind1 == 99) 
		{
			Record record = bucket.records[q];
			retVal.record = record;
			retVal.recordIndex = q;
			
			// uspesno trazenje
			if (key == record.key && record.status != EMPTY)
				retVal.ind1 = 0;
			// neuspesno trazenje 
			else if (record.status == EMPTY) 
				retVal.ind1 = 1;
			// nastavak trazenja
			else 
				q++;
		}

		if (q >= BUCKET_SIZE) 
		{
			// prelazak na sledeci baket
			bucketIndex = nextBucketIndex(bucketIndex);
			
			// povratak na maticni baket
			if (bucketIndex == initialIndex) 
			{
				retVal.ind1 = 1;
				retVal.ind2 = 1;
			}
		}
	}

	return retVal;
}

int alreadyExistsForInsert(FindRecordResult findResult) 
{
	if (findResult.ind1 == 0) 
	{
		// za verziju sa logickim brisanjem
		#ifdef LOGICAL
		if (findResult.record.status == ACTIVE) 
		{
			return 1;
		}

		// za verziju sa fizickim brisanjem
		#else 
		return 1;
		#endif
	}

	return 0;
}

int insertRecord(FILE* pFile, Record record) 
{
	record.status = ACTIVE;
	FindRecordResult findResult = findRecord(pFile, record.key);
	
	// ukoliko slog sa datim kljucem vec postoji
	if (alreadyExistsForInsert(findResult))
		return -1;

	if (findResult.ind2 == 1) 
	{
		puts("Unos nemoguc. Datoteka popunjena.");
		return -1;
	}
	
	// upis sloga u baket na mesto gde je neuspesno zavrseno trazenje
	// ili aktivacija pothodno logicki obrisanog sloga sa istim kljucem
	findResult.bucket.records[findResult.recordIndex] = record;
	
	// ukoliko je upis baketa u datoteku uspesan, povratna vrednost je redni broj baketa
	if (saveBucket(pFile, &findResult.bucket, findResult.bucketIndex))
		return findResult.bucketIndex;                                  
	else
		return -2;
}

int alreadyExistsForModify(FindRecordResult findResult) 
{
	// da li je bilo neuspesno trazenje
	if (findResult.ind1)                              
		return 0;

	// za verziju sa logickim brisanjem
	#ifdef LOGICAL                                      
	if (findResult.record.status == DELETED) 
	{
		return 0;
	}
	#endif

	return 1;
}

int modifyRecord(FILE* pFile, Record record) 
{
	record.status = ACTIVE;
	FindRecordResult findResult = findRecord(pFile, record.key);

	// ukoliko slog nije pronadjen ili je logicki obrisan
	if (!alreadyExistsForModify(findResult)) 
		return -1;

	// upis sloga u baket na mesto gde je pronadjen
	findResult.bucket.records[findResult.recordIndex] = record;         

	// ukoliko je upis baketa u datoteku uspesna, povratna vrednost je redni broj baketa
	if (saveBucket(pFile, &findResult.bucket, findResult.bucketIndex))
		return findResult.bucketIndex;
	else
		return -2;
}

int removeRecordLogically(FILE* pFile, int key) 
{
	FindRecordResult findResult = findRecord(pFile, key);
	
	// slog nije pronadjen
	if (findResult.ind1) 
		return -1;
	
	// logicko brisanje sloga
	findResult.bucket.records[findResult.recordIndex].status = DELETED; 

	// upis baketa u datoteku
	// ukoliko je brisanje uspesno, povratna vrednost je redni broj baketa
	if (saveBucket(pFile, &findResult.bucket, findResult.bucketIndex))
		return findResult.bucketIndex;                                  
	else
		return -2;
}
// pomocna funkcija za proveru da li se odredjeni slog moze prebaciti u ciljni baket kako bi bio blize maticnom baketu
void testCandidate(Record record, int bucketIndex, int targetBucketIndex, int* pFound) 
{
	int m = transformKey(record.key);

	if (bucketIndex > targetBucketIndex)
		if (m > bucketIndex || m <= targetBucketIndex)
			*pFound = 1;
	else 
		if (m > bucketIndex && m <= targetBucketIndex)
			*pFound = 1;
}

int removeRecordPhysically(FILE* pFile, int key) 
{
	FindRecordResult findResult = findRecord(pFile, key);

	// slog nije pronadjen
	if (findResult.ind1)
		return -1;
	
	// indeks sloga za brisanje u baketu
	int q = findResult.recordIndex;
	Bucket bucket = findResult.bucket;
	int bucketIndex = findResult.bucketIndex;

	// indikator pomeranja slogova
	int move = 1;

	while (move) 
	{
		// pomeranje slogova unutar baketa (ulevo)
		while (q < BUCKET_SIZE - 1 && bucket.records[q].status != EMPTY) 
		{
			bucket.records[q] = bucket.records[q + 1];
			q++;
		}

		// ciljni baket u koji ce se (eventualno) vrsiti premestanje slogova prekoracilaca iz narednih baketa
		Bucket targetBucket = bucket;
		int targetBucketIndex = bucketIndex;

		// baket nije bio pun
		if (bucket.records[q].status == EMPTY) 
		{
			move = 0;
		} 
		else 
		{
			int found = 0;

			while (!found && move) 
			{
				// ukoliko se doslo do kraja baketa dobavlja se naredni baket
				if (q == BUCKET_SIZE - 1) 
				{
					bucketIndex = nextBucketIndex(bucketIndex);
					readBucket(pFile, &bucket, bucketIndex);
					q = -1;
				}

				q++;

				if (bucket.records[q].status != EMPTY && bucketIndex != targetBucketIndex)
					testCandidate(bucket.records[q], bucketIndex, targetBucketIndex, &found);
				// pomeranje se zaustavlja bilo da se naidje na prazan slog ili ciljni baket
				else
					move = 0;
			}

			// nadjeni prekoracilac se prebacuje na kraj ciljnog baketa
			if (found)
				targetBucket.records[BUCKET_SIZE - 1] = bucket.records[q];
			// poslednji slog se proglasava praznim
			else
				targetBucket.records[BUCKET_SIZE - 1].status = EMPTY;
		}

		// ciljni baket se smesta u datoteku
		// ukoliko je nadjen i premesten odgovorajuci prekoracilac,
		// ceo postupak brisanja se ponavlja sad sa njegove prethodne pozicije
		// iz baketa u kojem je nadjen i time se ostavlja prostor da eventualno
		// neki drugi prekoracilac zauzme njegovo prethodno mesto, sto znaci da
		// imamo lancano pomeranje prekoracilaca prema maticnom baketu
		saveBucket(pFile, &targetBucket, targetBucketIndex);
	}

	return 0;
}

int removeRecord(FILE* pFile, int key) 
{
	#ifdef LOGICAL
		return removeRecordLogically(pFile, key);
	#else
		return removeRecordPhysically(pFile, key);
	#endif
}

void printContent(FILE* pFile) 
{
	int i;
	Bucket bucket;

	for (i = 0; i < B; i++) 
	{
		readBucket(pFile, &bucket, i);
		printf("\n####### BUCKET - %d #######\n", i+1);
		printBucket(bucket);
	}
}
