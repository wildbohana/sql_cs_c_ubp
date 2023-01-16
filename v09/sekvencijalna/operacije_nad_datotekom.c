#include "operacije_nad_datotekom.h"

FILE* otvoriDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "rb+");

	if (fajl == NULL)
		printf("Doslo je do greske pri otvaranju datoteke \"%s\"! Moguce da datoteka ne postoji.\n", filename);
	else
		printf("Datoteka \"%s\" uspesno otvorena!\n", filename);
	
	return fajl;
}

void kreirajDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "wb");

	if (fajl == NULL) 
	{
		printf("Doslo je do greske prilikom kreiranja datoteke \"%s\"!\n", filename);
	} 
	else 
	{
		// Upisi pocetni blok
		BLOK blok;
		strcpy(blok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
		fwrite(&blok, sizeof(BLOK), 1, fajl);

		printf("Datoteka \"%s\" uspesno kreirana.\n", filename);
		fclose(fajl);
	}
}

SLOG* pronadjiSlog(FILE* fajl, char* evidBroj) 
{
	if (fajl == NULL) return NULL;

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Nema vise slogova posle sloga sa oznakom kraja datoteke, ili smo stigli do sloga 
			// sa vrednoscu kljuca vecom od vrednosti kljuca sloga koji dodajemo u datoteku
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0 || atoi(blok.slogovi[i].evidBroj) > atoi(evidBroj)) 
			{
				return NULL;
			} 
			// Pronadjen trazeni slog
			else if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0) 
			{
				if (!blok.slogovi[i].deleted) 
				{
					SLOG* slog = (SLOG*)malloc(sizeof(SLOG));
					memcpy(slog, &blok.slogovi[i], sizeof(SLOG));
					return slog;
				}
			}
		}
	}

	return NULL;
}

// Dodavanje novog sloga u datoteku
void dodajSlog(FILE* fajl, SLOG* slog) 
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	// Treba pronaci poziciju prvog sloga sa vrednoscu kljuca vecom od vrednosti kljuca sloga koji upisujemo u datoteku
	// Na to mesto treba smestiti slog koji upisujemo, a slog koji je vec bio na toj poziciji upisati u datoteku na isti nacin

	// Slog koji se trenutno upisuje ce se nalaziti u promenljivoj "slogKojiUpisujemo"
	SLOG slogKojiUpisujemo;
	memcpy(&slogKojiUpisujemo, slog, sizeof(SLOG));

	BLOK blok;
	fseek(fajl, 0, SEEK_SET);

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Ako smo stigli do kraja datoteke, znaci da nema slogova u datoteci sa vrednoscu kljuca vecom od vrednosti kljuca sloga koji upisujemo; 
			// na to mesto cemo staviti slog koji se upisuje u datoteku, a oznaku kraja cemo pomeriti za jedno mesto unapred
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{			
				memcpy(&blok.slogovi[i], &slogKojiUpisujemo, sizeof(SLOG));

				// Na mesto ispred unetog, sad treba staviti slog sa oznakom kraja datoteke
				// Kako to izvesti, zavisi od toga da li ima praznih mesta u trenutnom bloku

				// Ako tekuci slog nije poslednji slog u bloku, ima jedno prazno mesto posle njega,
				// pa tu mozemo smestiti slog sa oznakom kraja datoteke
				if (i != FBLOKIRANJA - 1) 
				{					
					strcpy(blok.slogovi[i+1].evidBroj, OZNAKA_KRAJA_DATOTEKE);

					// To je to, mozemo vratiti blok u datoteku
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);

					printf("Novi slog evidentiran u datoteci.\n");
					return;
				} 
				// U suprotnom, nemamo vise mesta u ovom bloku
				// Oznaku kraja moramo prebaciti u sledeci blok (koji prvo moramo napraviti)
				else
				{
					// Kao prvo, trenutni blok je pun, pa ga mozemo vratiti nazad u datoteku
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);

					// Sada mozemo napraviti novi blok i upisati u njega slog sa oznakom kraja datoteke (na prvu poziciju)
					BLOK noviBlok;
					strcpy(noviBlok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
					fwrite(&noviBlok, sizeof(BLOK), 1, fajl);

					printf("Novi slog evidentiran u datoteci.\n");
					printf("(dodat novi blok)\n");
					return;
				}
			} 
			else if (strcmp(blok.slogovi[i].evidBroj, slogKojiUpisujemo.evidBroj) == 0) 
			{
				//printf("vec postoji %s == %s \n", blok.slogovi[i].evidBroj, slogKojiUpisujemo.evidBroj);

				if (!blok.slogovi[i].deleted) 
				{
					printf("Slog sa tom vrednoscu kljuca vec postoji!\n");
					return;
				}
				// Imamo logicki obrisan slog sa istom vrednoscu kljuca sloga kao slog koji upisujemo u datoteku
				// Na to mesto treba prepisati novi slog preko tog logicki izbrisanog
				else 
				{
					memcpy(&blok.slogovi[i], &slogKojiUpisujemo, sizeof(SLOG));

					// Sad samo vratimo ceo taj blok u datoteku i to je to:
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);

					printf("Novi slog evidentiran u datoteci.\n");
					printf("(prepisan preko logicki izbrisanog)\n");
					return;
				}
			} 
			else if (atoi(blok.slogovi[i].evidBroj) > atoi(slogKojiUpisujemo.evidBroj)) 
			{
				// Pronasli smo prvi slog sa kljucem vecim od ovog koji upisujemo u datoteku
				// Na to mesto smo planirali da smestimo novi slog
				// Pre nego sto to uradimo, sacuvacemo slog koji se vec nalazi na tom mestu, u promenljivu tmp,
				// a potom ga odatle prepisati u "slogKojiUpisujemo", jer cemo njega sledeceg upisivati (na isti nacin kao i prethodni slog)
				SLOG tmp;
				memcpy(&tmp, &blok.slogovi[i], sizeof(SLOG));
				memcpy(&blok.slogovi[i], &slogKojiUpisujemo, sizeof(SLOG));
				memcpy(&slogKojiUpisujemo, &tmp, sizeof(SLOG));

				// Sada u "slogKojiUpisujemo" stoji slog koji je bio na tekucoj poziciji (pre nego sto smo na nju upisali onaj koji dodajemo)
				// U narednoj iteraciji, taj slog ce se dodavati u datoteku na isti nacin

				// Takodje, ako je to bio poslednji slog u bloku, mozemo ceo taj blok vratiti u datoteku
				if (i == FBLOKIRANJA - 1) 
				{
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);
					fflush(fajl);
				}
			}
		}
	}
}

// Ispis svih slogova iz datoteke
void ispisiSveSlogove(FILE* fajl) 
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;
	int rbBloka = 0;

	printf("BL SL Evid.Br   Sif.Zat      Dat.Vrem.Dol  Celija  Kazna\n");
	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Kraj datoteke
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				printf("B%d S%d *\n", rbBloka, i);
				return;
			}
			// Ispis bloka 
			else if (!blok.slogovi[i].deleted) 
			{
				printf("B%d S%d ", rbBloka, i);
				ispisiSlog(&blok.slogovi[i]);
				printf("\n");
			}
		}

		rbBloka++;
	}
}

// Ispis sloga
void ispisiSlog(SLOG* slog) 
{
	printf("%8s  %7s  %02d-%02d-%4d %02d:%02d %7s %6d",
		slog->evidBroj,
		slog->sifraZatvorenika,
		slog->datumVremeDolaska.dan,
		slog->datumVremeDolaska.mesec,
		slog->datumVremeDolaska.godina,
		slog->datumVremeDolaska.sati,
		slog->datumVremeDolaska.minuti,
		slog->oznakaCelije,
		slog->duzinaKazne);
}

void azurirajSlog(FILE* fajl, char* evidBroj, char* oznakaCelije, int duzinaKazne) 
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;
	
	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0 || atoi(blok.slogovi[i].evidBroj) > atoi(evidBroj)) 
			{
				printf("Slog koji zelite modifikovati ne postoji!\n");
				return;
			}
			else if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0) 
			{
				if (blok.slogovi[i].deleted) 
				{
					printf("Slog koji zelite modifikovati ne postoji!\n");
					return;
				}

				// Azuriraj oznaku celije i duzinu kazne
				strcpy(blok.slogovi[i].oznakaCelije, oznakaCelije);
				blok.slogovi[i].duzinaKazne = duzinaKazne;

				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);

				printf("Slog izmenjen.\n");
				return;
			}
		}
	}
}

// Logicko brisanje sloga sa trazenim kljucem
void obrisiSlogLogicki(FILE* fajl, char* evidBroj) 
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0 || atoi(blok.slogovi[i].evidBroj) > atoi(evidBroj)) 
			{
				printf("Slog koji zelite obrisati ne postoji!\n");
				return;
			} 
			else if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0) 
			{
				if (blok.slogovi[i].deleted == 1) 
				{
					printf("Slog koji zelite obrisati ne postoji!\n");
					return;
				}
				
				blok.slogovi[i].deleted = 1;
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);
				
				printf("Brisanje sloga zavrseno.\n");
				return;
			}
		}
	}
}

// Fizicko brisanje sloga sa trazenim kljucem
void obrisiSlogFizicki(FILE* fajl, char* evidBroj) 
{
	SLOG* slog = pronadjiSlog(fajl, evidBroj);

	if (slog == NULL) 
	{
		printf("Slog koji zelite obrisati ne postoji!\n");
		return;
	}

	// Trazimo slog sa odgovarajucom vrednoscu kljuca, i potom sve slogove ispred njega povlacimo jedno mesto unazad
	BLOK blok, naredniBlok;
	char evidBrojZaBrisanje[8 + 1];
	strcpy(evidBrojZaBrisanje, evidBroj);

	fseek(fajl, 0, SEEK_SET);

	while (fread(&blok, 1, sizeof(BLOK), fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				// Ako je oznaka kraja bila prvi slog u poslednjem bloku posle brisanja onog sloga,
				// ovaj poslednji blok nam vise ne treba
				if (i == 0) 
				{					
					printf("(skracujem fajl...)\n");
					fseek(fajl, -sizeof(BLOK), SEEK_END);
					long bytesToKeep = ftell(fajl);
					ftruncate(fileno(fajl), bytesToKeep);

					// Da bi se primenile promene usled poziva truncate
					fflush(fajl); 
				}

				printf("Slog je fizicki obrisan.\n");
				return;
			}

			if (strcmp(blok.slogovi[i].evidBroj, evidBrojZaBrisanje) == 0) 
			{
				// Obrisemo taj slog iz bloka tako sto sve slogove ispred njega povucemo jedno mesto unazad
				for (int j = i+1; j < FBLOKIRANJA; j++)
					memcpy(&(blok.slogovi[j - 1]), &(blok.slogovi[j]), sizeof(SLOG));

				// Jos bi hteli na poslednju poziciju u tom bloku upisati prvi
				// slog iz narednog bloka, pa cemo zato ucitati naredni blok...
				int podatakaProcitano = fread(&naredniBlok, sizeof(BLOK), 1, fajl);

				// ...i pod uslovom da uopste ima jos blokova posle trenutnog...
				if (podatakaProcitano) 
				{
					// ako smo nesto procitali, poziciju u fajlu treba vratiti nazad
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);

					// ...prepisati njegov prvi slog na mesto poslednjeg sloga u trenutnom bloku
					memcpy(&(blok.slogovi[FBLOKIRANJA - 1]), &(naredniBlok.slogovi[0]), sizeof(SLOG));

					// U narednoj iteraciji while petlje, brisemo prvi slog iz narednog bloka.
					strcpy(evidBrojZaBrisanje, naredniBlok.slogovi[0].evidBroj);
				}

				// Vratimo trenutni blok u fajl
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);
				fflush(fajl);

				// Ako nema vise blokova posle trentnog, mozemo prekinuti algoritam
				if (!podatakaProcitano) 
				{
					printf("Slog je fizicki obrisan.\n");
					return;
				}

				// To je to, citaj sledeci blok
				break;
			}
		}
	}
}

// Fizicko brisanje sloga koji je logicki obrisan
void fizickiObrisiLogickiObrisanSlog(FILE* fajl, char* evidBroj) 
{
	// Prepravljena verzija funckije 'obrisiSlogFizicki' koja fizicki brise slog u datoteci bez obzira da li je on vec logicki obrisan ili ne
	// Koristi se za reorganizaciju, u funckiji 'reorganizujDatoteku'

	BLOK blok, naredniBlok;
	char evidBrojZaBrisanje[8+1];
	strcpy(evidBrojZaBrisanje, evidBroj);

	fseek(fajl, 0, SEEK_SET);

	while (fread(&blok, 1, sizeof(BLOK), fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				if (i == 0) 
				{
					// Ako je oznaka kraja bila prvi slog u poslednjem bloku posle brisanja onog sloga,
					// ovaj poslednji blok nam vise ne treba
					printf("(skracujem fajl...)\n");
					fseek(fajl, -sizeof(BLOK), SEEK_END);
					long bytesToKeep = ftell(fajl);
					ftruncate(fileno(fajl), bytesToKeep);

					// Da bi se odmah prihvatio truncate
					fflush(fajl); 
				}

				return;
			}

			if (strcmp(blok.slogovi[i].evidBroj, evidBrojZaBrisanje) == 0) 
			{
				// Obrisemo taj slog iz bloka tako sto sve slogove ispred njega povucemo jedno mesto unazad
				for (int j = i+1; j < FBLOKIRANJA; j++)
					memcpy(&(blok.slogovi[j - 1]), &(blok.slogovi[j]), sizeof(SLOG));

				// Jos bi hteli da na poslednju poziciju u tom bloku upisemo prvi
				// slog iz narednog bloka, pa cemo zato ucitati naredni blok...
				int podatakaProcitano = fread(&naredniBlok, sizeof(BLOK), 1, fajl);

				// ...i pod uslovom da uopste ima jos blokova posle trenutnog...
				if (podatakaProcitano) 
				{
					// ako smo nesto procitali, poziciju u fajlu treba vratiti nazad
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);

					// ...prepisati njegov prvi slog na mesto poslednjeg sloga u trenutnom bloku
					memcpy(&(blok.slogovi[FBLOKIRANJA - 1]), &(naredniBlok.slogovi[0]), sizeof(SLOG));

					// U narednoj iteraciji while loopa, brisemo prvi slog iz narednog bloka
					strcpy(evidBrojZaBrisanje, naredniBlok.slogovi[0].evidBroj);
				}

				// Vratimo trenutni blok u fajl
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);
				fflush(fajl);

				// Ako nema vise blokova posle trentnog, mozemo prekinuti algoritam
				if (!podatakaProcitano) return;

				// To je to, citaj sledeci blok
				break;
			}
		}
	}
}

void reorganizujDatoteku(FILE* fajl) 
{
	if (fajl == NULL)
		printf("Datoteka nije otvorena!\n");

	// U jednom prolazu cemo zabeleziti evidBrojeve svih logicki obrisanih slogova, 
	// i potom nad svakim od njih pozvati fizicko brisanje

	// U nekom nizu cemo da cuvamo evidBrojeve logicki obrisanih slogova,
	// pa mozemo npr napraviti niz, duzine otprilike koliko ima slogova u datoteci
	// (za slucaj da su nam svi slogovi u datoteci logicki obrisani)
	fseek(fajl, 0, SEEK_END);
	long duzinaFajla = ftell(fajl);
	int ppBrojSlogova = (int)((float)duzinaFajla/(float)sizeof(SLOG));
	//printf("Prepostavljeni broj slogova = %d\n", ppBrojSlogova);
	char** logickiObrisani = (char**)malloc(sizeof(char*) * ppBrojSlogova);

	// index da mozemo da indeksiramo taj niz kad ga budemo punili vrednostima
	int loIndex = 0;

	// Inicijalizujemo svaki od ovih "stringova" na null,
	// da bi posle znali sta nam je popunjeno, sta nije
	for (int i = 0; i < ppBrojSlogova; i++)
		logickiObrisani[i] = NULL;

	BLOK blok;
	fseek(fajl, 0, SEEK_SET);

	// Prvo, pronadjimo sve logicki izbrisane slogove
	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				break;
			}

			if (blok.slogovi[i].deleted) 
			{
				logickiObrisani[loIndex] = (char*)malloc(sizeof(char)*strlen(blok.slogovi[i].evidBroj));
				strcpy(logickiObrisani[loIndex], blok.slogovi[i].evidBroj);
				loIndex++;
			}
		}
	}

	// I onda za svaki od njih pozovemo fizicko brisanje
	printf("Fizicki brisem logicki obrisane slogove:\n");
	for (int i = 0; i < loIndex; i++) 
	{
		printf("\tSlog sa evid. brojem: %s\n", logickiObrisani[i]);
		fizickiObrisiLogickiObrisanSlog(fajl, logickiObrisani[i]);
	}

	// Oslobodi memoriju
	free(logickiObrisani);

	printf("Reogranizacija zavrsena.\n");
}
