-- Upiti u SQL
SELECT * FROM radnik;
SELECT * FROM projekat;
SELECT * FROM radproj;

SELECT ime, prz FROM radnik;

-- IzlIStati različita imena radnika
SELECT DISTINCT ime FROM radnik;

-- Prikaži radnike čija plata je veća od 25000
SELECT mbr, ime, prz FROM radnik 
WHERE plt>25000;

-- Izračunati godišnju platu svih radnika
SELECT mbr, ime, prz, plt*12 FROM radnik;

-- Radnici koji nemaju šefa
SELECT mbr, ime, prz FROM radnik 
WHERE sef IS NULL;

-- Prikaži radnike čija plata je između 20000 i 24000
SELECT mbr, ime, prz FROM radnik 
WHERE plt BETWEEN 20000 AND 24000;

-- Prikaži radnike rođene između 1953 i 1975
SELECT ime, prz, god FROM radnik
WHERE god BETWEEN '01-jan-1953' AND '31-dec-1975';

-- Prikaži radnike koji nISu rođeni između 1953 i 1975
SELECT ime, prz, god FROM radnik
WHERE god NOT BETWEEN '01-jan-1953' AND '31-dec-1975';

-- Prikaži radnike čije prezime počINje sa slovom M
SELECT mbr, ime, prz FROM radnik
WHERE prz LIKE 'M%';

-- Prikaži radnike čije ime ne počINje sa slovom A
SELECT mbr, ime, prz FROM radnik
WHERE ime NOT LIKE 'A%';

-- Prikaži radnike čije ime sadrži slovo a na drugoj poziciji
SELECT mbr, ime, prz FROM radnik
WHERE ime LIKE '_a%';

-- Prikaži radnike čije ime počINje sa slovom E
SELECT DISTINCT ime FROM radnik
WHERE ime LIKE 'E%';

-- Prikaži radnike koji u svom imenu imaju slovo E(e)
SELECT mbr, ime, prz FROM radnik
WHERE ime LIKE '%e%' OR ime LIKE '%E%';

-- Prikaži matične brojeve radnika koji rade na projektima sa šiFROM 10,20 ili 30
SELECT DISTINCT mbr FROM radproj
WHERE spr IN (10, 20, 30);

-- Prikaži matične brojeve radnika koji rade na projektima sa šiFROM 10,
-- ili rade 2,4 ili 6 sati
SELECT DISTINCT mbr FROM radproj
WHERE brc IN (2, 4, 6) OR spr='10';

-- IzlIStati matične brojeve radnika koji se ne zovu Ana ili Sanja
SELECT mbr, ime, prz FROM radnik
WHERE ime NOT IN ('Ana', 'Sanja');

-- Prikazati radnike koji imaju šefa, sORtirano po prezimenu
SELECT mbr, ime, prz, plt FROM radnik
WHERE sef IS NOT NULL ORDER BY prz ASC;

-- SORtiraj ranike rAStući po prezimenu i opadajući po imenu
SELECT Mbr, Prz, Ime, Plt
FROM Radnik ORDER BY Prz ASC, Ime DESC;

-- Umesto imena oznaka, možemo i kORIStiti brojeve 1,2,...
-- onim redosledom kojim su kORišćeni kod SELECT
SELECT Mbr, Prz, Ime FROM Radnik
ORDER BY 2, 3, Plt;

-- Možemo kORistiti i aliASe
SELECT Mbr, Ime, Prz, Plt "Plata" FROM Radnik
ORDER BY Plata DESC;

-- Prikazati radnike čije prezime sadrži ime
SELECT * FROM radnik WHERE LOWER(prz)
LIKE '%' || LOWER(ime) || '%';

-- Prikazati radnike i platu koji se zovu Pera ili Moma
SELECT Mbr, Ime, Prz, Plt FROM Radnik
WHERE Ime = ANY ('Pera', 'Moma');

-- Prikazati radnike i platu koji se ne zovu Pera ili Moma
SELECT Mbr, Ime, Prz, Plt FROM Radnik
WHERE Ime !=ALL ('Pera', 'Moma');

-- Prikazati šta se dobije kada se plata ranika uveća za NULL vrednost
SELECT Mbr, Plt + NULL FROM Radnik;

-- Prikazati platu svih radnika uvećane za godišnju premiju
SELECT Mbr, Plt + Pre FROM Radnik;

-- Prikazati platu svih radnika uvećane za godišnju premiju
-- Ukoliko za nekog radnika ne postoji vrednost premije, smatrati da ona iznosi 0
SELECT Mbr, Plt + NVL(Pre, 0) FROM Radnik;

-- Prikazati koliko ima radnika
SELECT count(*) FROM radnik;

-- Prikazati koliko ima šefova
SELECT count(DISTINCT sef) broj_sefova FROM radnik;

-- Prikazati minimalnu i maksimalnu platu radnika
SELECT min(plt) minimalna, max(plt) maksimalna FROM radnik;

-- Prikazati broj radnika i ukupnu mesečnu platu svih radnika
SELECT count(*) "Broj radnika", sum(plt) "Ukupna mesecna plata"
FROM radnik;

-- Prikazati broj radnika, prosečnu platu i ukupnu godišnju platu svih radnika
SELECT count(*) "Broj radnika", avg(plt) "Prosecna plata",
12*sum(plt) "Godisnja plata" FROM radnik;

-- Prikazati ukupnu premiju svih radnika čiji je mbr veći od 100
SELECT SUM(pre) FROM radnik 
WHERE mbr >100;

-- Prikazati prosečnu platu svih radnika zaokruženu na dve decimale
SELECT round(avg(plt *1.41), 2)
FROM radnik;

-- Možemo selektovati iz tabela (select u select-u)
SELECT * FROM (SELECT MBR,IME FROM radnik);

-- Prikazati 10 radnika koji imaju najveću platu,
-- sORtiranih po plati u opadajućem redosledu
SELECT mbr, plt, rownum FROM
(SELECT * FROM Radnik ORDER BY plt desc)
WHERE ROWNUM < 11;

-- Za svakog radnika prikazati red koji sadrži njegovu platu, 
-- prosečnu platu i apsolutnu (ABS) razliku prosečne plate i njegove plate
SELECT PLT, (SELECT ROUND(AVG(PLT), 2) 
FROM radnik) AS prosecna_plata,
ABS((SELECT ROUND(AVG(PLT), 2) 
FROM RADNIK) – plt) AS razlika FROM radnik;
