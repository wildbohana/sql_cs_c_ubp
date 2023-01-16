#include "operacije_nad_datotekom.h"

// Otvaranje postojece datoteke
FILE* otvoriDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "rb+");

	if (fajl == NULL)
		printf("Doslo je do greske! Moguce da datoteka \"%s\" ne postoji.\n", filename);
	else
		printf("Datoteka \"%s\" otvorena.\n", filename);
	
	return fajl;
}

// Kreiranje datoteke ako ona vec ne postoji
void kreirajDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "wb");
	
	if (fajl == NULL) 
	{
		printf("Doslo je do greske prilikom kreiranja datoteke \"%s\"!\n", filename);
	} 
	else 
	{
		// Upisujemo pocetni blok
		BLOK blok;
		strcpy(blok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
		fwrite(&blok, sizeof(BLOK), 1, fajl);

		printf("Datoteka \"%s\" uspesno kreirana.\n", filename);
		fclose(fajl);
	}
}

// Pronalatenje sloga sa trazenim kljucem
SLOG* pronadjiSlog(FILE* fajl, char* evidBroj) 
{
	if (fajl == NULL) return NULL;

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Ako nema vise slogova
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
				return NULL;
			
			// Ako se evidBroj poklapa i slog NIJE logicki obrisan
			if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0 && !blok.slogovi[i].deleted) 
			{
				SLOG* slog = (SLOG*)malloc(sizeof(SLOG));
				memcpy(slog, &blok.slogovi[i], sizeof(SLOG));
				return slog;
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

	SLOG* slogStari = pronadjiSlog(fajl, slog->evidBroj);

	// Ako u datoteci vec postoji slog sa tim evid. brojem, ne mozemo upisati novi slog
	if (slogStari != NULL) 
	{
		printf("Vec postoji slog sa tim evid brojem!\n");
		return;
	}

	BLOK blok;
	
	// Novi slog upisujemo u poslednji blok
	fseek(fajl, -sizeof(BLOK), SEEK_END); 	
	fread(&blok, sizeof(BLOK), 1, fajl);

	int i;
	for (i = 0; i < FBLOKIRANJA; i++) 
	{
		// Ovo je mesto gde se nalazi slog sa oznakom kraja datoteke; tu treba upisati novi slog
		// strcmp(s1, s2) vraca vrednost 0 ako se dva stringa poklapaju
		if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
		{            
			memcpy(&blok.slogovi[i], slog, sizeof(SLOG));
			break;
		}
	}

	// na to mesto u bloku cemo upisati krajDatoteke
	i++; 

	// Ako jos uvek ima mesta u ovom bloku, mozemo tu smestiti slog sa oznakom kraja datoteke
	if (i < FBLOKIRANJA) 
	{        
		strcpy(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE);

		// Sada blok mozemo vratiti u datoteku
		fseek(fajl, -sizeof(BLOK), SEEK_CUR);
		fwrite(&blok, sizeof(BLOK), 1, fajl);
		fflush(fajl);
	} 
	// Ako nema vise mesta u tom bloku, moramo praviti novi blok u koji cemo upisati slog sa oznakom kraja datoteke
	else 
	{
		// Prvo ipak moramo vratiti u datoteku blok koji smo upravo popunili
		fseek(fajl, -sizeof(BLOK), SEEK_CUR);
		fwrite(&blok, sizeof(BLOK), 1, fajl);

		// Okej, sad pravimo novi blok
		BLOK noviBlok;
		strcpy(noviBlok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
		
		// Nema potrebe za fseekom jer smo vec na kraju datoteke
		fwrite(&noviBlok, sizeof(BLOK), 1, fajl);
	}

	if (ferror(fajl))
		printf("Greska pri upisu u fajl!\n");
	else
		printf("Upis novog sloga zavrsen.\n");
}

// Ispis svih slogova datoteke
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
			// Kraj datoteke ???
			// Ispisuje broj blokova i slogova na kraju
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				printf("B%d S%d *\n", rbBloka, i);
				break;
			}

			// Ispisuje broj bloka i sloga, a zatim i podatke iz sloga
			if (!blok.slogovi[i].deleted) 
			{
				printf("B%d S%d ", rbBloka, i);
				ispisiSlog(&blok.slogovi[i]);
				printf("\n");
			}
		}

		rbBloka++;
	}
}

// Ispis trazenog sloga
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

// Azuriranje sloga sa trazenim kljucem
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
			// Ako smo dosli do kraja datoteke, to znaci da tog sloga nema u datoteci
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				printf("Slog koji zelite menjati ne postoji!\n");
				return;
			}

			// Pronasli smo trazeni slog i on NIJE logicki obrisan -> modifikujemo ga:
			if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0 && !blok.slogovi[i].deleted) 
			{
				// Azuriraj oznaku celije i duzinu kazne
				strcpy(blok.slogovi[i].oznakaCelije, oznakaCelije);
				blok.slogovi[i].duzinaKazne = duzinaKazne;

				// Upis izmenjenog sloga u fajl
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);
				fflush(fajl);

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
			// Ako smo dosli do kraja datoteke, to znaci da tog sloga nema u datoteci
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{
				printf("Nema tog sloga u datoteci\n");
				return;
			}

			// Pronasli smo trazeni slog i on NIJE logicki obrisan -> brisemo ga:
			if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0 && !blok.slogovi[i].deleted) 
			{
				blok.slogovi[i].deleted = 1;
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);
				fflush(fajl);

				printf("Slog je logicki obrisan.\n");
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

	// Trazimo slog sa odgovarajucom vrednoscu kljuca, i potom sve slogove ispred njega povlacimo za jedno mesto unazad
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
				// Ako je oznaka kraja bila prvi slog u poslednjem bloku, 
				// posle brisanja onog sloga, ovaj poslednji blok nam vise ne treba
				if (i == 0) 
				{
					printf("(skracujem fajl...)\n");
					fseek(fajl, -sizeof(BLOK), SEEK_END);
					long bytesToKeep = ftell(fajl);
					ftruncate(fileno(fajl), bytesToKeep);

					// (da bi se primenile promene usled poziva truncate)
					fflush(fajl); 
				}

				printf("Slog je fizicki obrisan.\n");
				return;
			}

			// Obrisemo taj slog iz bloka tako sto sve slogove ispred njega povucemo jedno mesto unazad
			if (strcmp(blok.slogovi[i].evidBroj, evidBrojZaBrisanje) == 0) 
			{                
				for (int j = i+1; j < FBLOKIRANJA; j++)
					memcpy(&(blok.slogovi[j-1]), &(blok.slogovi[j]), sizeof(SLOG));

				// Jos bi hteli na poslednju poziciju u tom bloku da upisemo prvi
				// slog iz narednog bloka, pa zato ucitavamo naredni blok...
				int podatakaProcitano = fread(&naredniBlok, sizeof(BLOK), 1, fajl);

				// ...i pod uslovom da uopste ima jos blokova posle trenutnog...
				if (podatakaProcitano) 
				{
					// ako smo nesto procitali, poziciju u fajlu treba vratiti nazad
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);

					// ...prepisati njegov prvi slog na mesto poslednjeg sloga u trenutnom bloku
					memcpy(&(blok.slogovi[FBLOKIRANJA-1]), &(naredniBlok.slogovi[0]), sizeof(SLOG));

					// U narednoj iteraciji while petlje, brisemo prvi slog iz narednog bloka
					strcpy(evidBrojZaBrisanje, naredniBlok.slogovi[0].evidBroj);
				}

				// Vratimo trenutni blok u fajl.
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
