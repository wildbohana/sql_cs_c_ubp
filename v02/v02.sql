-- <GROUP BY> - na osnovu čega ih grupišemo
-- Bez grupisanja - dobijemo 5 vrsta
select mbr, spr
from radproj
where mbr < 40;

-- Sa grupisanjem - dobijemo 3 vrste
select mbr, count(spr)
from radproj
where mbr < 40
group by mbr;

-- Prikazati koliko radnika radi na svakom projektu
-- i koliko je ukupno angažovanje na tom projektu
select spr, count(mbr), sum(brc)
from radproj
group by spr;

-- <HAVING>
-- Izlistati mbr radnika koji rade na više od dva projekta, 
-- pored mbr-a, prikazati i broj projekata na kojima radnici rade
select mbr
from radproj
having count(spr)>2;
-- Ili
select mbr, count(spr)
from radproj
group by mbr
having count(spr)>2;

-- Svaka kolona koja se nađe među izrazima SELECT klauzule,
-- osim onih kolona koji su pod skupovnom funkcijom,
-- se mora naći i u izrazima GROUP BY klauzule (prethodni upit)

-- WHERE filtrira torke, a ne grupe
-- HAVING filtrira grupe

-- NEZAVISNI UGNJEŽDENI UPITI
-- Izlistati u rastućem redosledu plate mbr, ime, prz i plt 
-- radnika koji imaju platu veću od prosečne
select mbr, ime, prz, plt
from radnik
where plt > (select avg(plt) from radnik)
order by plt asc;

-- Izlistati imena i prezimena radnika koji 
-- rade na projektu sa šifrom 30
select ime, prz
from radnik
where mbr in (select mbr from radproj where spr = 30);

-- Izlistati mbr, ime, prz radnika koji rade na projektu
-- sa šifrom 10, a ne rade na projektu sa šifrom 30
select mbr, ime, prz
from radnik
where mbr in (select mbr from radproj where spr = 10)
and mbr not in (select mbr from radproj where spr = 30);

-- Izlistati ime, prz i god najstarijeg radnika
select mbr, ime, prz, god
from radnik
where god <= all(select god from radnik);
-- Ili
select mbr, ime, prz, god
from radnik
where god = (select min(god) from radnik);

-- SPAJANJE TABELA
-- Prikazati mbr, prz, ime, plt i brc angažovanja svih
-- radnika koji rade na projektu sa šifrom 10.
select radnik.mbr, prz, ime, plt, brc
from radnik, radproj
where spr = 10 
and radnik.mbr = radproj.mbr;
-- Ili
select r.mbr, r.prz, r.ime, r.plt, rp.brc
from radnik r, radproj rp
where rp.spr = 10
and r.mbr = rp.mbr;

-- Prikazati mbr, ime, prz i plt radnika koji su rukovodioci projekata
select distinct mbr, ime, prz, plt
from radnik, projekat
where ruk = mbr;

-- Izlistati imena, prezimena svih radnika osim 
-- rukovodioca projekta sa šifrom 10
select mbr, ime, prz
from radnik r, radproj rp
where p.spr = 10 and r.mbr != p.ruk;
-- Ili
select ime, prz, mbr
from radnik
where mbr != (select ruk from projekat where spr = 10);

-- Izlistati nazive projekata na kojima radi bar jedan radnik
-- koji radi i na projektu sa šifrom 60
select p.nap
from projekat p 
where spr in (
	select spr 
	from radproj 
	where mbr in (
		select mbr
		from radproj
		where spr = 60
));

-- Prikazati imena i prezimena rukovodilaca
-- projekata i broj projekata kojima rukovode
select prz, ime, count(spr)
from radnik r, projekat p 
where ruk = mbr
group by mbr, prz, ime;

-- Prikazati za svakog radnika mbr, prz, ime, ukupan broj
-- projekata i ukupno angažovanje na projektima na kojima radi
select r.mbr, r.prz, r.ime, count(*), sum(rp.brc)
from radnik r, radproj rp
where r.mbr = rp.mbr
group by r.mbr, r.prz, r.ime;

-- Prikazati imena i prezimena rukovodilaca
-- projekata i broj projekata na kojima rade
select ime, prz, count(rp.spr) bp
from radnik r, radproj rp
where r.mbr = rp.mbr
and r.mbr in (select ruk from projekat)
group by r.mbr, prz, ime;
-- Ili
select ime, prz, count(distinct rp.spr)
from radnik r, projekat p, radproj rp
where rp.mbr = r.mbr 
and p.ruk = r.mbr
group by r.mbr, ime,prz;

-- Izlistati nazive projekata na kojima se ukupno radi više od 15 časova
select nap
from projekat p, radproj rp
where p.spr = rp.spr
group by p.spr, nap
having sum(brc) > 15;

-- Izlistati šifre i nazive projekata na kojima radi više od dva radnika
select p.spr, p.nap
from projekat p, radproj rp
where rp.spr = p.spr
group by p.spr, p.nap
having count(mbr) > 2;

-- Izlistati nazive i šifre projekata na kojima je prosečno
-- angažovanje veće od prosečnog angažovanja na svim projektima
select p.spr, p.nap
from projekat p, radproj rp
where rp.spr = p.spr
group by p.spr, p.nap
having avg(brc) > (select avg(brc) from radproj);

-- Izlistati nazive i šifre projekata sa najvećim prosečnim angažovanjem
select p.spr, p.nap
from projekat p, radproj rp
where rp.spr = p.spr
group by p.spr, p.nap
having avg(brc) >= all(
	select avg(brc)
	from radproj
	group by spr
);

-- Prikazati mbr, ime, prz, plt radnika koji zarađuju
-- više od radnika sa matičnim brojem 40
select r.mbr, r.prz, r.ime, r.plt
from radnik r1, radnik r2
where r1.plt > r2.plt
and r2.mbr = 40;

-- Prikazati imena, prezimena i plate radnika koji zarađuju bar
-- 1000 dinara manje od rukovodioca projekta na kom radnik radi
select r1.ime, r1.prz, r1.plt, p.nap
from radnik r1, radnik r2, projekat p, radproj rp
where r1.mbr = rp.mbr
and rp.spr = p.spr
and p.ruk = r2.mbr 
and r1.plt + 1000 < r2.plt;
