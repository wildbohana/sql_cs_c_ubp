// Blokirane datoteke - jedan blok ima n slogova (ovde ima 3 sloga)
#define FBLOKIRANJA 3
#define OZNAKA_KRAJA_DATOTEKE -1

typedef struct Trajanje {
	int sati;
	int minuti;
	int sekunde;
} TRAJANJE;

// Svaki slog predstavlja jedan dolazak
typedef struct Slog {
	int rBr; 			// koristi se kao kljuc sloga
	char imeFilma[20 + 1];
	TRAJANJE trajanje;
	char vrstaProjekcije[6 + 1];
	int cenaKarte;
	int deleted;
} SLOG;

typedef struct Blok {
	SLOG slogovi[FBLOKIRANJA];
} BLOK;
