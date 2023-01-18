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
		printf("2 - Ispis svih projekcija\n");
		printf("3 - Pretraga po ceni projekcija\n");
		printf("4 - Pretraga po vrsti projekcija\n");
		printf("5 - Logicko brisanje svih jeftinijih projekcija\n");
		printf("0 - Izlaz\n");

		if (fajl == NULL)
			printf("!!! PAZNJA: datoteka jos uvek nije otvorena !!!\n");
		
		scanf("%d", &userInput);
		getc(stdin);

		switch(userInput) 
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

			// Ispis svih projekcija
			case 2:
			{
				ispisiSveProjekcije(fajl);
				printf("\n");
				break;
			}

			// Pretraga po ceni projekcija
			case 3:
			{
				int cena;
				printf("Unesite cenu za koju trazite skuplje projekcije: ");
				scanf("%d", &cena);

				ispisiSveTrazenePoCeni(fajl, cena);

				printf("\n");
				break;
			}

			// Pretraga po vrsti projekcija
			case 4:
			{
				char vrsta[6 + 1];
				printf("Unesite vrstu projekcija koju zelite da pretrazite: ");
				scanf("%s", vrsta);

				ispisiSveTrazenePoVrsti(fajl, vrsta);

				printf("\n");
				break;
			}

			// Logicko brisanje svih jeftinijih projekcija
			case 5:
			{
				int cena;
				printf("Unesite cenu projekcija za logicko brisanje: ");
				scanf("%d", &cena);

				obrisiSlogLogicki(fajl, cena);
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
