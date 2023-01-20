// Blokirane datoteke - jedan blok ima n slogova (ovde ima 3 sloga)
#define FBLOKIRANJA 3
#define OZNAKA_KRAJA_DATOTEKE "********"

typedef struct DateTime {
	int dan;
	int mesec;
	int godina;
	int sati;
	int minuti;
} DATETIME;

// Svaki slog predstavlja jedan dolazak
// +1 za dužinu char[] zbog "\n"
typedef struct Slog {
	char evidBroj[8 + 1]; 			// koristi se kao kljuc sloga
	char sifraZatvorenika[7 + 1];
	DATETIME datumVremeDolaska;
	char oznakaCelije[5 + 1];
	int duzinaKazne;
	int deleted;
} SLOG;

typedef struct Blok {
	SLOG slogovi[FBLOKIRANJA];
} BLOK;
