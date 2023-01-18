#ifndef OPERACIJE_NAD_DATOTEKOM_H
#define OPERACIJE_NAD_DATOTEKOM_H

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <unistd.h>
#include <sys/types.h>

#include "definicije_struktura_podataka.h"

FILE* otvoriDatoteku(char* filename);
void ispisiSlog(SLOG* slog);
void ispisiSveProjekcije(FILE* fajl);
void ispisiSveTrazenePoCeni(FILE* fajl, int cena);
void ispisiSveTrazenePoVrsti(FILE* fajl, char* vrsta);
void obrisiSlogLogicki(FILE* fajl, int cena);

// Otvaranje postojece datoteke
FILE* otvoriDatoteku(char* filename) 
{
	FILE* fajl = fopen(filename, "rb+");

	if (fajl == NULL)
		printf("Doslo je do greske pri otvaranju datoteke %s.\n", filename);
	else
		printf("Datoteka %s je uspesno otvorena.\n", filename);
	
	return fajl;
}

// Ispis sloga
void ispisiSlog(SLOG* slog) 
{
	printf("%d   %21s %02d:%02d:%02d %7s %6d",
		slog->rBr,
		slog->imeFilma,
		slog->trajanje.sati,
		slog->trajanje.minuti,
		slog->trajanje.sekunde,
		slog->vrstaProjekcije,
		slog->cenaKarte);
}

// Ispis svih slogova datoteke
// Identicno
void ispisiSveProjekcije(FILE* fajl) 
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;
	int rbBloka = 0;

	printf("BL SL RBr  Naziv      Trajanje  Vrsta  Cena\n");
	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Kraj datoteke
			if (blok.slogovi[i].rBr == OZNAKA_KRAJA_DATOTEKE) 
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

// Pronalazenje projekcija sa cenom vecom od zadate
void ispisiSveTrazenePoCeni(FILE* fajl, int cena) 
{
	if (fajl == NULL) return;

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	printf("Projekcije sa cenom vecom od %d:\n", cena);
	printf("BL SL RBr  Naziv      Trajanje  Vrsta  Cena\n");

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Ako nema vise slogova
			if (blok.slogovi[i].rBr == OZNAKA_KRAJA_DATOTEKE) 
				return;
			
			// Ako se evidBroj poklapa i slog NIJE logicki obrisan
			if (blok.slogovi[i].cenaKarte >= cena && !blok.slogovi[i].deleted) 
			{
				ispisiSlog(&blok.slogovi[i]);
			}
		}
	}
}

// Pronalazenje projekcija koje trazena vrsta
void ispisiSveTrazenePoVrsti(FILE* fajl, char* vrsta) 
{
	if (fajl == NULL) return;

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	printf("Projekcije vrste %s:\n", vrsta);
	printf("BL SL RBr  Naziv      Trajanje  Vrsta  Cena\n");

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Ako nema vise slogova
			if (blok.slogovi[i].rBr == OZNAKA_KRAJA_DATOTEKE)
				return;
			
			// Ako se evidBroj poklapa i slog NIJE logicki obrisan
			if (strcmp(blok.slogovi[i].vrstaProjekcije, vrsta) == 0 && !blok.slogovi[i].deleted) 
			{
				ispisiSlog(&blok.slogovi[i]);
			}
		}
	}
}

// Logicko brisanje sloga sa cenom manjom od trazene
void obrisiSlogLogicki(FILE* fajl, int cena) 
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
			if (blok.slogovi[i].rBr == OZNAKA_KRAJA_DATOTEKE)
			{
				printf("Nema tog sloga u datoteci\n");
				return;
			}
			// Pronasli smo trazeni slog i on NIJE logicki obrisan -> brisemo ga
			else if (blok.slogovi[i].cenaKarte < cena) 
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

#endif
