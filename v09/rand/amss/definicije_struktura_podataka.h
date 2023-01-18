// Blokirane datoteke - jedan blok ima n slogova (ovde ima 3 sloga)
#define FBLOKIRANJA 3
#define OZNAKA_KRAJA_DATOTEKE -1

// Svaki slog predstavlja jedno uverenje
typedef struct Slog {
	int sifraUverenja;
	char prezimeMehanicara[10 + 1];
	char datumIzdavanja[10 + 1];
	float cenaPregleda;
	char prezimeVlasnika[10 + 1];
	char vrstaVozila[2 + 1];
	int deleted;
} SLOG;

typedef struct Blok {
	SLOG slogovi[FBLOKIRANJA];
} BLOK;
