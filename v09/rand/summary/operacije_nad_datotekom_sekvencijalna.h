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
void dodajSlog(FILE* fajl, SLOG* dolazak);
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
		printf("Doslo je do greske pri otvaranju datoteke %s!.\n", filename);
	else
		printf("Datoteka %s uspesno otvorena!\n", filename);
	
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
		// Napravi i upisi pocetni blok
		BLOK blok;
		strcpy(blok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
		fwrite(&blok, sizeof(BLOK), 1, fajl);

		printf("Datoteka %s uspesno kreirana.\n", filename);
		fclose(fajl);
	}
}

// Pronalazenje sloga sa trazenim kljucem
// Skoro identicno
SLOG* pronadjiSlog(FILE* fajl, char* evidBroj) 
{
	if (fajl == NULL) return NULL;

	// Pozicioniraj se na pocetak fajla
	fseek(fajl, 0, SEEK_SET);
	BLOK blok;

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Nismo pronasli slog
			// Razlika: provera da li je evidBroj veci od trazenog kljuca
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0 
			|| atoi(blok.slogovi[i].evidBroj) > atoi(evidBroj)) 
			{
				return NULL;
			} 
			// Ako se evidBroj poklapa i slog NIJE logicki obrisan
			else if (strcmp(blok.slogovi[i].evidBroj, evidBroj) == 0 && !blok.slogovi[i].deleted) 
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

	// Slog koji se trenutno upisuje ce se nalaziti u promenljivoj "slogKojiUpisujemo"
	SLOG slogKojiUpisujemo;
	memcpy(&slogKojiUpisujemo, slog, sizeof(SLOG));

	BLOK blok;
	fseek(fajl, 0, SEEK_SET);

	while (fread(&blok, sizeof(BLOK), 1, fajl)) 
	{
		for (int i = 0; i < FBLOKIRANJA; i++) 
		{
			// Ako smo stigli do kraja datoteke, na to mesto cemo ubaciti novi slog 
			// a oznaku kraja cemo pomeriti za jedno mesto unapred
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0) 
			{			
				memcpy(&blok.slogovi[i], &slogKojiUpisujemo, sizeof(SLOG));

				// Ako tekuci slog nije poslednji slog u bloku, 
				// ima jedno prazno mesto posle njega,
				// pa tu mozemo smestiti slog sa oznakom kraja datoteke
				if (i != FBLOKIRANJA - 1) 
				{					
					strcpy(blok.slogovi[i + 1].evidBroj, OZNAKA_KRAJA_DATOTEKE);

					// Vracamo blok u datoteku
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);

					printf("Novi slog evidentiran u datoteci.\n");
					return;
				} 
				// U suprotnom, oznaku kraja moramo prebaciti u sledeci blok 
				// (koji prvo moramo napraviti)
				else
				{
					// Trenutni blok je pun, pa ga vracamo nazad u datoteku
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);

					// Sada pravimo novi blok i u njega upisujemo
					// slog sa oznakom kraja datoteke (na prvu poziciju)
					BLOK noviBlok;
					strcpy(noviBlok.slogovi[0].evidBroj, OZNAKA_KRAJA_DATOTEKE);
					fwrite(&noviBlok, sizeof(BLOK), 1, fajl);

					printf("Novi slog evidentiran u datoteci.\n");
					printf("(dodat novi blok)\n");
					return;
				}
			} 
			// Ako smo u datoteci pronasli slog sa istim kljucem
			// moramo proveriti da li je taj slog logicki obrisan ili ne
			else if (strcmp(blok.slogovi[i].evidBroj, slogKojiUpisujemo.evidBroj) == 0) 
			{
				// Slog nije logicki obrisan - ne radimo nista
				if (!blok.slogovi[i].deleted) 
				{
					printf("Slog sa tom vrednoscu kljuca vec postoji!\n");
					return;
				}
				// Slog je logicki obrisan - prepisujemo novi slog preko njega
				else 
				{
					memcpy(&blok.slogovi[i], &slogKojiUpisujemo, sizeof(SLOG));

					// Vracamo ceo taj blok u datoteku
					fseek(fajl, -sizeof(BLOK), SEEK_CUR);
					fwrite(&blok, sizeof(BLOK), 1, fajl);

					printf("Novi slog evidentiran u datoteci.\n");
					printf("(prepisan preko logicki izbrisanog)\n");
					return;
				}
			} 
			// Ako smo stigli do sloga sa kljucem vecim od onog koji upisujemo
			// Na to mesto smestamo novi slog
			else if (atoi(blok.slogovi[i].evidBroj) > atoi(slogKojiUpisujemo.evidBroj)) 
			{
				// Prvo cemo da sacuvamo slog koji se vec nalazi na tom mestu u
				// promenljivu tmp, potom ga odatle prepisati u "slogKojiUpisujemo",
				// jer cemo njega sledeceg upisivati (u sledecoj iteraciji)
				SLOG tmp;
				memcpy(&tmp, &blok.slogovi[i], sizeof(SLOG));
				memcpy(&blok.slogovi[i], &slogKojiUpisujemo, sizeof(SLOG));
				memcpy(&slogKojiUpisujemo, &tmp, sizeof(SLOG));

				// Ako je to bio poslednji slog u bloku, taj blok vracamo u datoteku
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
				return;
			}
			// Ispis sloga 
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
			// Dosli smo do kraja datoteke, ili do kljuca veceg od trazenog
			// Razlika: proverava se i da li je evidBroj veci od trazenog kljuca
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0 
			|| atoi(blok.slogovi[i].evidBroj) > atoi(evidBroj)) 
			{
				printf("Slog koji zelite modifikovati ne postoji!\n");
				return;
			}
			// Pronasli smo blok, ali prvo moramo da proverimo da li je obrisan
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

				// Upisi azurirani blok u datoteku
				fseek(fajl, -sizeof(BLOK), SEEK_CUR);
				fwrite(&blok, sizeof(BLOK), 1, fajl);

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
			// Razlika: provera da li je evidBroj veci od trazenog kljuca
			if (strcmp(blok.slogovi[i].evidBroj, OZNAKA_KRAJA_DATOTEKE) == 0 
			|| atoi(blok.slogovi[i].evidBroj) > atoi(evidBroj)) 
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

	// Trazimo slog sa odgovarajucom vrednoscu kljuca, 
	// potom sve slogove ispred njega povlacimo jedno mesto unazad
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

					// Da bi se primenile promene usled poziva truncate
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
					memcpy(&(blok.slogovi[j - 1]), &(blok.slogovi[j]), sizeof(SLOG));

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
