#ifndef OPERACIJE_NAD_DATOTEKOM_H
#define OPERACIJE_NAD_DATOTEKOM_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <unistd.h>
#include <sys/types.h>

#include "definicije_struktura_podataka.h"

FILE* otvoriDatoteku(char* filename);
void kreirajDatoteku(char* filename);
SLOG* pronadjiSlog(FILE* fajl, char* evidBroj);
void dodajSlog(FILE* fajl, SLOG* slog);
void ispisiSveSlogove(FILE* fajl);
void ispisiSlog(SLOG* slog);
void azurirajSlog(FILE* fajl, char* evidBroj, char* oznakaCelije, int duzinaKazne);
void obrisiSlogLogicki(FILE* fajl, char* evidBroj);
void obrisiSlogFizicki(FILE* fajl, char* evidBroj);

// Otvaranje postojece datoteke
// Identicno
FILE* otvoriDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "rb+");

	if (fajl == NULL)
		printf("Doslo je do greske pri otvaranju datoteke %s.\n", filename);
	else
		printf("Datoteka %s je uspesno otvorena.\n", filename);
	
	return fajl;
}

// Kreiranje datoteke
// Identicno
void kreirajDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "wb");
	
	if (fajl == NULL) 
	{
		printf("Doslo je do greske prilikom kreiranja datoteke %s!\n", filename);
	}
	else 
	{
		// Upisujemo pocetni blok
		BLOK blok;
		strcpy(blok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
		fwrite(&blok, sizeof(BLOK), 1, fajl);

		printf("Datoteka %s je uspesno kreirana.\n", filename);
		fclose(fajl);
	}
}

// Pronalazenje sloga sa trazenim kljucem
// Skoro identicno
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
// Velike razlike u ove dve funkcije
void dodajSlog(FILE* fajl, SLOG* slog) 
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	// Provera da li u datoteci vec postoji slog sa tim kljucem
	// Tada ne mozemo da upisemo novi slog
	SLOG* slogStari = pronadjiSlog(fajl, slog->evidBroj);
	if (slogStari != NULL) 
	{
		printf("Vec postoji slog sa tim evid brojem!\n");
		return;
	}

	// Novi slog upisujemo u poslednji blok
	BLOK blok;
	fseek(fajl, -sizeof(BLOK), SEEK_END); 	
	fread(&blok, sizeof(BLOK), 1, fajl);

	int i;
	for (i = 0; i < FBLOKIRANJA; i++) 
	{
		// Ovde se nalazi slog sa oznakom kraja datoteke i tu upisujemo novi slog
		if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
		{            
			memcpy(&blok.slogovi[i], slog, sizeof(SLOG));
			break;
		}
	}

	// Cuvamo tu poziciju u bloku da bi smo kasnije upisali krajDatoteke
	i++;

	// Ako jos uvek ima mesta u ovom bloku, tu smestamo slog sa oznakom kraja datoteke
	if (i < FBLOKIRANJA) 
	{        
		strcpy(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE);

		// Blok vracamo u datoteku
		fseek(fajl, -sizeof(BLOK), SEEK_CUR);
		fwrite(&blok, sizeof(BLOK), 1, fajl);
		fflush(fajl);
	}
	// Ako nema vise mesta u tom bloku, moramo praviti novi blok
	else 
	{
		// Prvo vracamo u datoteku blok koji smo upravo popunili
		fseek(fajl, -sizeof(BLOK), SEEK_CUR);
		fwrite(&blok, sizeof(BLOK), 1, fajl);

		// Pravimo novi blok
		BLOK noviBlok;
		strcpy(noviBlok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
		
		// Upisujemo novi blok na kraj datoteke (ne treba nam fseek)
		fwrite(&noviBlok, sizeof(BLOK), 1, fajl);
	}

	if (ferror(fajl))
		printf("Greska pri upisu u fajl!\n");
	else
		printf("Upis novog sloga zavrsen.\n");
}

// Ispis svih slogova datoteke
// Identicno
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
				break;
			}

			// Ispisuje slog
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

// Ispis sloga
// Identicno
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

// Azuriranje sloga
// Skoro identicno
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

			// Pronasli smo trazeni slog i on NIJE logicki obrisan -> modifikujemo ga
			if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0) 
			{
				if (blok.slogovi[i].deleted) 
				{
					printf("Slog koji zelite modifikovati ne postoji!\n");
					return;
				}

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
// Skoro identicno
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
			// Pronasli smo trazeni slog i on NIJE logicki obrisan -> brisemo ga
			else if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0) 
			{
				if (blok.slogovi[i].deleted == 1) 
				{
					printf("Slog koji zelite obrisati ne postoji!\n");
					return;
				}

				// Logicko brisanje
				blok.slogovi[i].deleted = 1;

				// Upis izmenjenog sloga u fajl
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
// Identicno
void obrisiSlogFizicki(FILE* fajl, char* evidBroj) 
{
	SLOG* slog = pronadjiSlog(fajl, evidBroj);
	if (slog == NULL) 
	{
		printf("Slog koji zelite obrisati ne postoji!\n");
		return;
	}

	// Trazimo slog sa odgovarajucom vrednoscu kljuca
	// potom sve slogove ispred njega povlacimo za jedno mesto unazad
	BLOK blok, naredniBlok;
	char evidBrojZaBrisanje[8 + 1];
	strcpy(evidBrojZaBrisanje, evidBroj);

	fseek(fajl, 0, SEEK_SET);

	while (fread(&blok, 1, sizeof(BLOK), fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Ako je oznaka kraja bila prvi slog u poslednjem bloku, 
			// posle brisanja onog sloga, ovaj poslednji blok nam vise ne treba
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{				
				if (i == 0) 
				{
					printf("(skracujem fajl...)\n");
					fseek(fajl, -sizeof(BLOK), SEEK_END);

					// ftell(fajl) - ukupna velicina fajla do trenutnog pokazivaca
					long bytesToKeep = ftell(fajl);
					ftruncate(fileno(fajl), bytesToKeep);

					fflush(fajl); 
				}

				printf("Slog je fizicki obrisan.\n");
				return;
			}

			// Ako smo pronasli slog koji treba da obrisemo
			// Obrisacemo taj slog iz bloka tako sto cemo sve slogove ispred njega
			// povuci jedno mesto unazad
			if (strcmp(blok.slogovi[i].evidBroj, evidBrojZaBrisanje) == 0) 
			{                
				for (int j = i + 1; j < FBLOKIRANJA; j++)
					memcpy(&(blok.slogovi[j-1]), &(blok.slogovi[j]), sizeof(SLOG));

				// Na poslednju poziciju u tom bloku upisujemo prvi slog iz narednog
				// Zato ucitavamo naredni blok
				int podatakaProcitano = fread(&naredniBlok, sizeof(BLOK), 1, fajl);

				// Proveravamo da li uopste ima jos blokova posle trenutnog
				if (podatakaProcitano) 
				{
					// Ako ih ima, poziciju u fajlu trebamo vratiti nazad
					// i zatim prepisati njegov prvi slog umesto poslednjeg
					// sloga za trenutan blok
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					memcpy(&(blok.slogovi[FBLOKIRANJA - 1]), &(naredniBlok.slogovi[0]), sizeof(SLOG));

					// U narednoj iteraciji petlje, brisemo prvi slog iz njega
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

				// NE ZABORAVI BREJK
				break;
			}
		}
	}
}

#endif
