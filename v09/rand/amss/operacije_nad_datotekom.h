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
void ispisiSvaUverenja(FILE* fajl);
void uvecajCenu(FILE* fajl);
void pretragaMehanicar(FILE* fajl, char* mehanicar);
void obrisiSlogLogicki(FILE* fajl, char* vrsta);

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
	printf("%d  %7s  %02d-%02d-%4d %02d:%02d %7s %6d",
		slog->sifraUverenja,
		slog->prezimeMehanicara,
		slog->datumIzdavanja,
		slog->cenaPregleda,
		slog->prezimeVlasnika,
		slog->vrstaVozila);
}

// Ispis svih uverenja datoteke
void ispisiSvaUverenja(FILE* fajl)
{
	if (fajl == NULL) 
	{
		printf("Datoteka nije otvorena!\n");
		return;
	}

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;
	int rbBloka = 0;

	printf("BL SL SIFRA   Prz.Meh      Datum  Cena  Prz.Vla   Vrsta\n");
	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Kraj datoteke
			if (blok.slogovi[i].sifraUverenja == OZNAKA_KRAJA_DATOTEKE) 
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

// Povecanje cene pregleda za 1000 dinara
void uvecajCenu(FILE* fajl) 
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
			if (blok.slogovi[i].sifraUverenja == OZNAKA_KRAJA_DATOTEKE) 
			{
				printf("Cene pregleda su azurirane.\n");
				return;
			}

			if (blok.slogovi[i].deleted) 
			{
				continue;
			}

			// Azuriraj cenu
			blok.slogovi[i].cenaPregleda += 1000;

			// Upis izmenjenog sloga u fajl
			fseek(fajl, -sizeof(BLOK), SEEK_CUR);
			fwrite(&blok, sizeof(BLOK), 1, fajl);
			fflush(fajl);
		}
	}
}

// Ispis svih uverenja izdatih od trazenog mehanicara
void pretragaMehanicar(FILE* fajl, char* mehanicar)
{
	if (fajl == NULL) return;

	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	printf("Uverenja izdata od mehanicara %s:\n", mehanicar);
	printf("BL SL SIFRA   Prz.Meh      Datum  Cena  Prz.Vla   Vrsta\n");

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++)
		{
			// Ako nema vise slogova
			if (blok.slogovi[i].sifraUverenja == OZNAKA_KRAJA_DATOTEKE)
				return;
			
			// Ako se evidBroj poklapa i slog NIJE logicki obrisan
			if (strcmp(blok.slogovi[i].prezimeMehanicara, mehanicar) == 0 && !blok.slogovi[i].deleted)
				ispisiSlog(&blok.slogovi[i]);
		}
	}
	return;
}

// Logicko brisanje automobila zadate vrste
void obrisiSlogLogicki(FILE* fajl, char* vrsta) 
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
			// Ako smo dosli do kraja datoteke
			if (blok.slogovi[i].sifraUverenja == OZNAKA_KRAJA_DATOTEKE)
			{
				return;
			}
			else if (strcmp(blok.slogovi[i].vrstaVozila, vrsta) == 0) 
			{
				if (blok.slogovi[i].deleted == 1)
					continue;

				// Logicko brisanje
				blok.slogovi[i].deleted = 1;

				// Upis izmenjenog sloga u fajl
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);
				fflush(fajl);

				printf("Uverenje je logicki obrisano.\n");
				return;
			}
		}
	}
}

#endif
