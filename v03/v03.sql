-- POVEZANI UPITI
-- Prikazati mbr, ime, prz, plt radnika čiji je
-- broj  sati angažovanja  na nekom projektu veći
-- od prosečnog broja sati angažovanja na tom projektu
select distinct r.mbr, ime, prz, plt, brc
from radnik r, radproj rp1
where r.mbr = rp1.mbr
and rp1.brc > (
	select avg(brc)
	from radproj rp2
	where rp2.spr = rp1.spr
);

-- <EXISTS>
-- Ko je najstariji radnik?
select ime, prz, god
from radnik r
where not exists (
	select mbr
	from radnik r1 
	where r1.god < r.god
);

-- Izlistati mbr, ime, prz radnika koji ne rade na projektu sa šifrom 10
select mbr, ime, prz
from radnik r
where not exists (
	select *
	from radproj rp
	where r.mbr = rp.mbr
	and rp.spr = 10
);

-- Izlistati radnike koji ne rade ni na jednom projektu
select mbr, ime, prz
from radnik r 
where not exists (
	select *
	from radproj rp
	where r.mbr = rp.mbr
);
-- Ili
select mbr, ime, prz
from radnik r
where mbr not in (
	select rp.mbr
	from radproj rp
);

-- Izlistati radnike koji nisu rukovodioci projekata
select mbr, ime, prz
from radnik r
where not exists (
	select *
	from projekat
	where mbr = ruk
);
-- Ili
select mbr, ime, prz
from radnik r
where mbr not in (
	select ruk
	from projekat
);

-- Ko je najmlađi rukovodilac projekta?
select distinct mbr, ime, prz, god
from radnik r, projekat p 
where r.mbr = p.ruk 
and not exists (
	select mbr
	from radnik r1, projekat p1
	where r1.mbr = p1.ruk 
	and r1.god > r.god
);

-- <UNION>
-- Izlistati mbr, ime, prz radnika koji rade na projektu sa 
-- šifrom 20 ili im je plata veća od prosečne (unija)
select mbr, ime, prz
from radnik
where mbr in (
	select mbr
	from radproj
	where spr = 20) 
union
select mbr, ime, prz
from radnik
where plt > (
	select avg(plt) 
	from radnik
);

-- <UNION ALL>
-- Izlistati mbr, ime, prz radnika koji rade na projektu sa 
-- šifrom 20 ili im je plata veća od prosečne (unija)
select mbr, ime, prz
from radnik
where mbr in (
	select mbr
	from radproj
	where spr = 20) 
union all
select mbr, ime, prz
from radnik
where plt > (
	select avg(plt) 
	from radnik
);

-- <INTERSECT>
-- Izlistati mbr, ime, prz radnika čije prezime počinje
-- na slovo M ili slovo R i mbr, ime, prz radnika čije
-- prezime počinje na slovo M ili slovo P
select mbr, ime, prz
from radnik
where prz like 'M%'
or prz like 'R%'
intersect
select mbr, ime, prz 
from radnik
where prz like 'M%' 
or prz like 'P%';

-- <MINUS> - razlika
-- Izlistati mbr, ime, prz radnika čije prezime počinje
--  na slovo M ili slovo R i mbr, ime, prz radnika čije
-- prezime počinje na slovo M ili slovo P
select mbr, ime, prz
from radnik
where prz like 'M%'
or prz like 'R%'
minus
select mbr, ime, prz
from radnik
where prz like 'M%'
or prz like 'P%';

-- <NATURAL> - prirodno spajanje (na osnovu imena kolona)
-- Prikazati ime i prz radnika koji rade na projektu sa šifrom 30
select ime, prz
from radnik natural join radproj
where spr = 30;

-- <INNER> - unutrasnje spajanje
-- Prikazati ime i prz radnika koji rade na projektu sa šifrom 30
select ime, prz 
from radnik r inner join radproj rp 
on r.mbr = rp.mbr
where spr = 30;

-- <LEFT OUTER>
-- Prikazati mbr, ime i prz radnika i šifre projekata na kojima rade
-- Prikazati, takođe, iste podatke i za radnike koji ne rade ni na
-- jednom projektu, pri čemu za šifru projekta treba, u tom
-- slučaju, prikazati nedostajuću vrednost
select r.mbr, ime, prz, spr
from radnik r left outer join radproj rp
on r.mbr = rp.mbr;

-- Prikazati mbr, ime i prz svih radnika i nazive projekata
-- kojima rukovode. Ukoliko radnik ne rukovodi ni jednim
-- projektom ispisati: ne rukovodi projektom
select r.mbr, ime, prz, nvl(nap, 'ne rukovodi projektom') Projekat 
from radnik r left outer join projekat p 
on r.mbr = p.ruk;

-- <RIGHT OUTER>
-- Prikazati nazive svih projekata i mbr radnika koji rade
-- na njima. Ukoliko na projektu ne radi ni jedan radnik 
-- ispisati nulu umesto matičnog broja
select nvl(rp.mbr, 0) "MBR radnika", nap
from radproj rp right outer join projekat p 
on rp.spr = p.spr;

-- <FULL OUTER>
-- Prikazati nazive svih projekata i mbr radnika koji rade
-- na njima. Ukoliko na projektu ne radi ni jedan radnik 
-- ispisati nulu umesto matičnog broja (prethodni upit)
select nvl(rp.mbr, 0) "Mbr radnika", nap
from radproj rp full outer join projekat p
on rp.spr = p.spr;

-- Prikazati matične brojeve, imena i prezimena radnika,
-- zajedno sa šiframa projekata na kojima rade. Prikazati,
-- takođe, iste podatke i za radnike koji ne rade ni na
-- jednom projektu, pri čemu za šifru projekta treba,
-- u tom slučaju, prikazati nedostajuću vrednost
select r.mbr, r.prz, r.ime, rp.spr
from radnik r left outer join radproj rp
on r.mbr = rp.mbr;
-- Ili kraće
select r.mbr, r.prz, r.ime, rp.spr 
from radnik r, radproj rp
where r.mbr = rp.mbr(+);

-- Prikazati za sve radnike i projekte na kojima rade 
-- Mbr, Prz, Ime, Spr i Nap. Za radnike koje ne rade ni na jednom
-- projektu, treba prikazati Mbr, Prz i Ime, dok za vrednosti obeležja 
-- Spr i Nap treba zadati, redom, konstante 0 i "Ne postoji".
-- Urediti izlazni rezultat saglasno rastućim vrednostima obeležja Mbr.
select r.mbr, r.prz, r.ime, nvl(p.spr, 0) as spr, nvl(p.nap, 'Ne postoji') as nap 
from radnik r, radproj rp, projekat p 
where r.mbr = rp.mbr(+)
and rp.spr = p.spr(+)
order by mbr;

-- Prikazati imena i prezimena svih radnika i prezimena njihovih
-- šefova ako ih imaju. Ako nema šefa ispisati: nema sefa
select r1.ime, r1.prz "Radnik", nvl(r2.prz, 'Nema sefa') sef 
from radnik r1 left outer join radnik r2 on r1.sef = r2.mbr 
order by r1.prz;

-- <CROSS JOIN> - Dekartov proizvod između dve tabele

-- Za svaku satnicu angažovanja (brc), prikazati koliko 
-- radnika radi na nekom projektu sa tom satnicom. 
-- Rezultate urediti u opadajućem redosledu satnice
select brc, count(mbr)
from radproj
group by brc
order by brc desc;

-- Za svakog radnika prikazati matični broj, ime, prezime,
-- kao i broj projekata kojima rukovodi, pri čemu je potrebno
-- prikazati isključivo one radnike koji su rukovodioci na 
-- manjem broju projekata od prosečnog broja projekata na
-- kojima rade radnici čije se prezime ne završava na “ic”
select mbr, ime, count(spr) br_pr_rukovodi
from radnik r left outer join projekat p on r.mbr = p.ruk 
group by mbr, ime
having count(spr) < (
	select avg(count(spr))
	from radproj rp, radnik r 
	where rp.mbr = r.mbr 
	and prz not like '%ic'
	group by r.mbr
);

-- <CASE>
--Za svakog radnika prikazati mbr, ime, prz, kao kategoriju kojoj
-- pripada na osnovu visine plate.
-- Kategorije po visini plate su sledeće:
---- Plata manja od 10000 – kategorija: ’mala primanja’,
---- plata između 10000 i 20000 – kategorija: ’srednje visoka primanja’,
---- plata između 20000 i 40000 – kategorija: ’visoka primanja’,
---- plata veća od 40000 – kategorija: ’izuzetno visoka primanja’.
-- Takođe, radnike urediti prema kategoriji kojoj pripadaju,
-- u redosledu od najniže ka najvišoj kategoriji po visini plate
select mbr, ime, plt 
case 
	when plt < 10000 then 'mala primanja'
	when plt >= 10000 and plt < 20000 then 'srednja primanja'
	when plt >= 20000 and plt < 40000 then 'visoka primanja'
	else 'izuzetno visoka primanja'
end as visina_primanja 
from radnik 
order by 
case visina_primanja
	when 'izuzetno visoka primanja' then 1
	when 'visoka primanja' then 2
	when 'srednja primanja' then 3
	else 4
end desc, plt asc;

-- Za svakog radnika ispisati mbr, ime, prz, platu i mbr šefa
-- Pri ispisu treba obezbediti da su radnici uređeni saglasno
-- visini plate, od najviše ka najnižoj, pri čemu bi direktor
-- firme trebalo da se ispiše prvi
select mbr, ime, plt, sef 
from radnik 
order by case
	when sef i null then 1
	else 2
end, plt desc;

-- AŽURIRANJE BAZE PODATAKA --

-- <INSERT> - dodavanje nove torke
insert  into Radnik (mbr, ime, prz, plt, sef, god) 
values (201, 'Ana', 'Savic', 30000, null, '18-aug-71');

insert into Projekat (spr, nap, ruk)
values (90, 'P1', 201);

insert into RadProj (mbr, spr, brc)
values (201, 90, 5);

-- <DELETE> - brisanje postojećih torki
delete radnik where mbr = 201;

-- <UPDATE> - modifikacija postojećih torki
update radnik
set plt = plt * 1.2;
where mbr = 201;
