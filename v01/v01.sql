-- <SELECT>
-- Izlistati sadržaj svih tabela
select * from radnih;
select * from projekat;
select * from radproj;

-- Prikazati imena i prezimena svih radnika
select ime, prz 
from radnik;

-- <DISTINCT>
-- Izlistati različita imena radnika
select distinct ime 
from radnik;

-- <WHERE>
-- Izlistati mbr, ime i prezime radnika koji imaju platu veću od 25000
select mbr, ime, prz
from radnik
where plt > 25000;

-- Aritmetički izrazi
-- Izlistati godišnju platu svakog radnika
select mbr, ime, prz, plt*12
from radnik;

-- NULL vrednost
-- Izlistati mbr, ime, prz radnika koji nemaju šefa
select mbr, ime, prz
from radnik
where sef is null;

-- <BETWEEN>
-- Izlistati mbr, ime, prz radnika čija je
-- plata između 20000 i 24000 dinara
select mbr, ime, prz
from radnik
where plt between 20000 and 24000;

-- Izlistati ime, prz, god radnika rođenih između 1953 i 1975
select ime, prz, god
from radnik
where god between '01-jan-1953' and '31-dec-1975';

-- Izlistati ime, prz, god radnika koji nisu rođeni između 1953 i 1975
select ime, prz, god
from radnik
where god not between '01-jan-1953' and '31-dec-1975';

-- <LIKE>
-- Izlistati mbr, ime, prz radnika čije prezime počinje na slovo M
select mbr, ime, prz
from radnik
where prz like 'M%';

-- Izlistati mbr, ime, prz radnika čije ime ne počinje slovom A
select mbr, ime, prz
from radnik
where ime not like 'A%';

-- Izlistati mbr, ime, prz radnika čije ime 
-- sadrži slovo a na drugoj poziciji
select mbr, ime, prz
from radnik
where ime like '_a%';

-- Izlistati imena radnika koja počinju na slovo E
-- Imena ne bi trebalo da se ponavljaju
select distinct ime
from radnik
where ime like 'E%';

-- Izlistati radnike koji u svom imenu imaju slovo E (e)
select mbr, ime, prz
from radnik
where ime like '%e%' or ime like '%E%';

-- <IN>
-- Izlistati matične brojeve radnika koji 
-- rade na projektima sa šifrom 10, 20 ili 30
select distinct mbr
from radproj
where spr in (10, 20, 30);

-- Izlistati matične brojeve radnika koji rade na
-- projektu sa šifrom 10, ili rade 2, 4, ili 6 sati.
select distinct mbr
from radproj
where brc in (2, 4, 6) or spr='10';

-- Izlistati matične brojeve radnika koji se ne zovu Ana ili Sanja
select mbr, ime, prz
from radnik
where ime not in ('Ana', 'Sanja');

-- <ORDER BY>
-- Prikazati radnike koji imaju šefa sortirano po prezimenu
select mbr, ime, prz
from radnik
where sef is not null
order by prz asc;

-- Neki primeri za ORDER BY
select mbr, prz, ime, plt
from radnik 
order by prz asc, ime desc;

select mbr, prz, ime
from radnik
order by 2, 3, plt;

select mbr, prz, ime
from radnik
order by 2, 3, plt * 1.18;

-- Prikazati matične brojeve, imena, prezimena i plate radnika,
-- po opadajućem redosledu iznosa plate
select mbr, ime, prz Plata
from radnik
order by Plata desc;

-- UREĐIVANJE IZLAZNIH REZULTATA
-- Prikazati matične brojeve, spojena (konkatenirana) imena i
-- prezimena radnika, kao i plate, uvećane za 17%
select mbr, ime || '' || prz "Ime i prezime", plt * 1.17 Plata 
from radnik;

-- Prikazati radnike čije prezime sadrži ime (Marko Marković npr)
select * 
from radnik
where lower(prz) like '%' || lower(ime) || '%';

-- <ANY>
-- Prikazati matične brojeve radnika, imena i prezimena i platu 
-- radnika koji se zovu Pera ili Moma
select mbr, ime, prz, plt
from radnik
where ime = any('Pera', 'Moma');

-- <ALL>
-- Prikazati matične brojeve radnika, imena i prezimena i platu
-- radnika koji se ne zovu Pera ili Moma.
select mbr, ime, prz, plt
from radnik
where ime != all('Pera', 'Moma');

-- SKUPOVNE FUNKCIJE
-- Prikazati matične brojeve radnika, kao i plate, 
-- uvećane za NULL vrednost
select mbr, plt + null
from radnik;
--> problem zbog null

-- Prikazati matične brojeve radnika, kao i plate,
-- uvećane za godišnju premiju
select mbr, plt + pre 
from radnik;
--> problem jer nemaju svi premiju

-- <NVL(izraz, konstanta)>
-- Prikazati matične brojeve radnika, kao i plate, uvećane za 
-- godišnju premiju. Ukoliko za nekog radnika vrednost
-- premije ne postoji, smatrati da ona iznosi 0
select mbr, plt + NVL(pre, 0)
from radnik;

-- <COUNT>
-- Koliko ima radnika?
select count(*)
from radnik;

-- Koliko ima šefova?
select count(distinct sef) broj_sefova
from radnik;

-- <MIN> <MAX>
-- Prikazati minimalnu i maksimalnu platu radnika
select min(plt) minimalna, max(plt) maksimalna
from radnik;

-- <SUM> - ignoriše NULL vrednost
-- Prikazati broj radnika i ukupnu mesečnu platu svih radnika
select count(*) "Broj radnika", sum(plt) "Ukupna mesecna plata"
from radnik;

-- <AVG> - ignoriše NULL vrednost
-- Prikazati broj radnika, prosečnu platu i 
-- ukupnu godišnju platu svih radnika.
select 
	count(*) "Broj radnika", 
	avg(plt) "Prosecna plata", 
	12*sum(plt) "Godisnja plata"
from radnik;

-- <ROUND>
-- Prikazati prosečnu platu svih radnika pomnoženu 
-- sa koren iz 2 (1,41), zaokruženo na dve decimale.
select round(avg(plt * 1.41), 2)
from radnik;

-- select u select-u
select * from 
(select mbr, ime, from radnik);

-- <ROWNUM>
-- Prikazati 10 radnika koji imaju najveću platu,
-- sortiranih po plati u opadajućem redosledu
select mbr, plt, rownum 
from (select * from radnik order by plt desc)
where rownum <= 10;

-- Prikazati za svakog radnika red koji sadrži 
-- njegovu platu, prosečnu platu i apsolutnu (ABS)
-- razliku prosečne plate i njegove plate
select 
	plt,
	(select round(avg(plt), 2) from radnik) as prosecna_plata,
	abs((select round(avg(plt), 2) from radnik) - plt) as razlika
from radnik;
