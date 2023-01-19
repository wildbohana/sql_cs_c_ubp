/*
AMSS, uverenja za automobile
0. Napraviti blok sa faktorom blokiranja 3, kraj datoteke je -1
1. Ispisati sve mehanicare (ovo sam mozda trebala malo drugacije)
2. Za sve automobile uvecaj cenu pregleda za 1000 dinara
3. Korisnik unosi prezime po kom trazis mehanicara, i ispisujes sva uverenja koja je on izdao
4. Logicko brisanje uverenja (ne znam po kom kriterijumu, valjda po vrsti automobila)
*/

#include "stdio.h"
#include "stdlib.h"

#include "operacije_nad_datotekom.h"

// gcc *.c && ./a.out

int main() 
{
	int running = 1;
	int userInput;

	FILE* fajl = NULL;

	while (running) 
	{
		printf("Odaberite opciju:\n");
		printf("1 - Otvaranje datoteke\n");
		printf("2 - Ispis svih mehanicara\n");
		printf("3 - Uvecaj cenu pregleda za sve automobile\n");
		printf("4 - Ispis svih uverenja izdatih od trazenog mehanicara\n");
		printf("5 - Logicko brisanje automobila zadate vrste\n");
		printf("0 - Izlaz\n");

		if (fajl == NULL)
			printf("!!! PAZNJA: datoteka jos uvek nije otvorena !!!\n");
		
		scanf("%d", &userInput);
		getc(stdin);

		switch (userInput) 
		{
			// Otvaranje datoteke
			case 1:
			{
				char filename[20];
				printf("Unesite ime datoteke za otvaranje: ");
				scanf("%s", &filename[0]);
				
				fajl = otvoriDatoteku(filename);
				printf("\n");
				break;
			}

			// Ispis svih mehanicara
			case 2:
			{
				ispisiSvaUverenja(fajl);
				printf("\n");
				break;
			}

			// Uvecaj cenu pregleda za sve automobile za 1000 dinara
			case 3:
			{
				uvecajCenu(fajl);
				printf("\n");
				break;
			}

			// Ispis svih uverenja izdatih od trazenog mehanicara
			case 4:
			{
				char mehanicar[10 + 1];
				printf("Unesite prezime trazenog mehanicara: ");
				scanf("%s", mehanicar);

				pretragaMehanicar(fajl, mehanicar);

				printf("\n");
				break;
			}
			
			// Logicko brisanje automobila zadate vrste
			case 5:
			{
				char vrsta[2 + 1];
				printf("Unesite vrstu automobila za logicko brisanje: ");
				scanf("%s", vrsta);

				obrisiSlogLogicki(fajl, vrsta);
				printf("\n");
				break;
			}

			// Izlaz
			case 0:
			{
				// Zatvori datoteku
				if (fajl != NULL) fclose(fajl);

				// Ugasi program
				running = 0;
			}
		}
	}

	return 0;
}
