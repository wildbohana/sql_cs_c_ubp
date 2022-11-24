/*
Tipovi podataka:
- VARCHAR2(size) - string promenljive dužine
- CHAR(size) - string fiksne dužine
- NUMBER(p,s) - broj, p cifara ispred i s cifara iza decimalnog zareza
- DATE
- LONG
*/

-- Kreirati tabelu faze_projekta
-- faze_projekta({Spr, Sfp, Rukfp, Nafp, Datp}, {Spr+ Sfp})
-- Obeležja Spr i Sfp ne smeju imati null vrednost
-- Obeležje Nafp mora imati jedinstvenu vrednost
CREATE TABLE faze_projekta;

-- U tabelu faze_projekta dodati atribut:
-- Datz - datum završetka faze projekta
-- Datz ne sme biti manji od Datp
alter table faze_projekta
add datz date [null];

-- U tabelu faze_projekta dodati bar dve faze
-- za jedan projekat i jednu za drugi projekat

-- Za svaki projekat prikazati sifru projekta, naziv projekta,
-- ime i prezime rukovodioca projekta, prezime njegovog šefa,
-- nazive faza projekta, imena i prezimena rukovodioca faza projekta.
-- Ako projekat nije podeljen u faze napisati: nema faze.

-- Brisanje definicije tabele
drop table faze_projekta;

-- CREATE TABLE AS SELECT
-- Kreirati tabelu radnik2 koja ima iste kolone kao 
-- tabela radnik, pri čemu radnik2 sadrži samo podatke
-- o radnicima koji imaju platu manju od 10000
--> tabela radnik2 neće imati indekse i sva ograničenja koja ima tabela radnik
create table radnik2 as 
(select * from radnik where plt < 10000);

-- INSERT INTO SELECT
-- Dopuniti tabelu radnik2 podacima koji joj nedostaju iz tabele radnik
insert into radnik2 
(select * from radnik where plt >= 10000);

-- Izbrisati sadržaj i definiciju tabele radnik2
drop table radnik2;

-- <WITH>
-- Prikazati za svakog radnika angažovanog na projektu
-- mbr, prz, ime, spr i broj drugih radnika koji su
-- angažovani na istom projektu
select r.mbr, r.ime, r.prz, rp1.spr, count(rp2.mbr - 1) ostali
from radnik r, radproj rp1, radproj rp2
where r.mbr = rp1.mbr 
and rp1.spr = rp2.spr 
group by r.mbr, r.ime, r.prz, rp1.spr;
-- Ili sa upotrebom WITH
with projinfo as (
	select rp.spr, count(rp.mbr) as rad_proj
	from radproj rp group by rp.spr )
select r.mbr, r.ime, r.prz, rp.spr, pi.rad_broj - 1 ostali 
from radnik r, radproj rp, projinfo pi 
where r.mbr = rp.mbr and rp.spr = pi.spr;

-- Prikazati za svakog radnika angažovanog na projektu
-- mbr, prz, ime, spr i udeo u ukupnom broju časova rada
-- na tom projektu (zaokruženo na dve decimale)
with projinfo as (
	select rp.spr, sum(rp.brc) as cas_suma
	from radproj rp
	group by rp.spr)
select r.mbr, r.ime, r.prz, rp.spr, round(rp.brc / pi.cas_suma, 2) udeo 
from radnik r, radproj rp, projinfo pi
where r.mbr = rp.mbr 
and rp.spr = pi.spr;

-- Prikazati mbr, ime, prz, plt radnika čiji je broj sati 
-- angažovanja na nekom projektu veći od prosečnog broja
-- sati angažovanja na tom projektu
with projinfo as (
	select spr, avg(brc) prosek 
	from radproj
	group by spr)
select distinct r.mbr, r.ime, r.prz, r.plt
from radnik r, radproj rp, projinfo pi
where r.mbr = rp.mbr 
and rp.spr = pi.spr 
group by r.mbr, r.ime, r.prz, r.plt, pi.spr 
having avg(rp.brc) > (
	select prosek 
	from projinfo pi2 
	where pi2.spr = pi.spr
);

-- Prikazati mbr, ime, prz, plt radnika čiji je broj sati
-- angažovanja na nekom projektu veći od
-- prosečnog angažovanja na svim projektima
with projinfo as (
	select spr, avg(brc) pros 
	from radproj
	group by spr)
select distinct r.mbr, r.ime, r.prz, r.plt 
from radnik r, radproj rp, projinfo pi 
where r.mbr = rp.mbr 
and rp.spr = pi.spr 
group by r.mbr, r.ime, r.prz, r.plt, pi.spr
having avg(rp.brc) < (select avg(pros) from projinfo);

-- Prikazati mbr, ime i prz rukovodilaca projekata kao i 
-- ukupan broj radnika kojima rukovode na projektima
with rukovodilac as (
	select mbr, ime, prz, plt, spr 
	from radnik, projekat 
	where mbr = ruk)
projinfo as (
	select spr, count(mbr) ljudi
	from radproj
	group by spr)
select ru.mbr, ru.ime, ru.prz, sum(pi.ljudi) ljudi
from rukovodilac ru, projinfo pi
where ru.spr = pi.spr 
group by ru.mbr, ru.ime, ru.prz;

-- Koliko je ukupno angažovanje svih šefova na projektima?
with angaz_po_radnicima (mbr, sbrc) as (
	select r.mbr, nvl(suma(rp.brc), 0)
	from radnik r, radproj rp
	where r.mbr = rp.mbr (+)
	group by r.mbr)
angaz_sefova (mbr, prz, ime, brrad, brsat) as (
	select distinct r.sef, r1.prz, r1.ime, count(*), a.sbrc 
	from radnik r, radnik r1, angaz_po_radnicima a 
	where r.sef = r1.mbr and r1.sef = a.mbr 
	group by r.sef, r1.prz, r1.ime, a.sbrc)
select sum(brsat) as ukangsef
from angaz_sefova;

-- <VIEW> - podupiti mogu se trajno sačuvati kao pogledi
-- Brisanje pogleda
drop view pogled;

-- Napraviti pogled koji će za sve radnike prikazati
-- samo njihova imena, prezimena i platu
create or replace view 
plate_radnika (ime, prezime, plata) as
select ime, prz, plt
from radnik;

-- Napraviti pogled koji će za sve radnike prikazati Mbr i ukupan
-- broj sati angažovanja radnika na projektima na kojima radi
create or replace view 
angaz_po_radnicima (mbr, sbrc) as
select r.mbr, nvl(sum(rp.brc), 0)
from radnik r, radproj rp 
where r.mbr = rp.mbr (+)
group by r.mbr;

-- Napraviti pogled koji će za svakog šefa (rukovodioca radnika) 
-- prikazati njegov matični broj, prezime, ime, ukupan broj radnika
-- kojima šefuje i njegovo ukupno angažovanje na svim projektima, 
-- na kojima radi. Koristiti prethodno definisani pogled
create view angaz_sefova (mbr, prz, ime, brrad, brsat) as 
select r.sef, r1.prz, r1.ime, count (*), a.sbrc 
from radnik r, radnik r1, angaz_po_radnicima a 
where r.sef = r1.mbr and r.sef = a.mbr 
group by r.sef, r1.prz, r1.ime, a.sbrc;

-- Koliko je ukupno angažovanje svih šefova na projektima?
select sum(brsat) as ukangsef 
from angaz_sefova;

-- TABELE
-- Prikazati nazive tabela čiji je vlasnik korisnik
select table_name
from user_tables;
-- Prikazati različite tipove objekata čiji je vlasnik korisnik
select distinct object_type
from user_objects;
-- Prikazati tabele, poglede, sinonime i sekvence čiji je vlasnik korisnik
select *
from user_catalog;

/*
Neke funkcije sa karakterima:
- LOWER(char)
- UPPER(char)
- INITCAP(char) - prvo slovo svake reči pretvara u veliko, ostala su mala
- SUBSTR(char, m, n) - koristi se za izdvajanje dela niza znakova (od m do n karaktera)
- TRIM(LEADING | TRAILING | BOTH trim_character from trim_source)
- LENGTH(char)
*/

-- Primer
select mbr, prz, ime 
from radnik 
where upper(prz) = 'PETRIC';

-- Prikazati radnike čije prezime na početku sadrži prva 3 
-- slova imena, na primer: Petar Petric
select * from radnik
where prz like substr(ime, 0, 3) || '%';

-- Prikazati imena i prezimena radnika tako da se sva imena
-- koja imaju poslednje slovo 'a', prikazuju bez poslednjeg slova
select trim(trailing 'a' from ime)
from radnik; 

-- Svim radnicima promeniti ime tako da poslednje slovo bude uvećano
-- Primer: AnA -> AnA, Marko -> MarkO
update radnik set 
ime = substr(ime, 1, lenght(ime) - 1) 
|| upper(substr(ime, lenght(ime), 1));

/*
Funkcije za konverziju podataka:
- TO_CHAR(d) - iz datuma u varchar2 (string)
- TO_CHAR(n) - iz broja u varchar2 (string)
- TO_DATE(char) - iz stringa u datum
- TO_NUMBER(char) - iz stringa u broj
*/

-- Za svakog radnika prikazati ime, prz, i projekte na kojima radi.
-- Ako ne radi ni na jednom projektu, napisati ‘Ne radi na projektu’.
-- Imena radnika prikazati velikim slovima, a prezimena malim.
select upper(ime), lower(prz), nvl(to_char(spr), 'Ne radi na projektu') broj_proj
from radnik left outer join radproj
on radnik.mbr = radproj.mbr;

-- Za svakog radnika prikazati datum rođenja u formatu yyyy/mm/dd
select to_char(god, 'yyyy/mm/dd')
from radnik;

-- OVER, PARTITION... (pdf 52)
-- I OSTATAK PDF-A; NEMEREM
