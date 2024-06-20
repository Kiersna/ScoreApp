CREATE DATABASE ScoreApp1;
GO
USE ScoreApp1;

CREATE TABLE REJESTRACJA(
ID INT not null identity(1,1) primary key,
IMIE VARCHAR(14) NOT NULL,
NAZWISKO VARCHAR(18) NOT NULL,
DATA_URODZENIA DATE NOT NULL,
EMAIL VARCHAR(24) NOT NULL,
HASLO VARCHAR(24) NOT NULL)

CREATE TABLE LOGOWANIE(
ID INT not null identity(1,1) primary key,
EMAIL VARCHAR(100) NOT NULL,
HASLO VARCHAR(99) NOT NULL)

CREATE TABLE Zespol
(ID int not null identity(1,1) primary key,
id_trenera int null,
nazwa_zespolu varchar(30) null,
liczba_zawodnikow int not null,
ID_TABELI int null)

CREATE TABLE tabela_ligi
(ID int NOT NULL IDENTITY(1,1) PRIMARY KEY,
liczba_zespolow int NOT NULL ,
ID_zespolu int null,)

CREATE TABLE TABLEA_LACZNIKOWA_ZESPOL_TABELA(
ID_zespolu int,
ID_tabeli INT,
FOREIGN KEY (ID_zespolu) references ZESPOL(ID),
FOREIGN KEY (ID_tabeli) references tabela_ligi(ID))


CREATE TABLE MECZ(
ID INT NOT NULL IDENTITY (1,1) PRIMARY KEY,
DATA_MECZU DATE NOT NULL,
SEDZIA VARCHAR(16) NULL,
WYNIK VARCHAR(5) NULL,
ZESPOL1 VARCHAR(14) NULL,
ZESPOL2 VARCHAR(14) NULL)



CREATE TABLE TRENER(
ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
IMIE VARCHAR(20) NOT NULL,
NAZWISKO VARCHAR(20) NOT NULL,
DATA_URODZENIA DATE NULL,
ID_ZESPOLU INT NULL,
NAZWA_AKUTALNEJ_DRUZYNY VARCHAR(50) NULL)

CREATE TABLE ZAWODNIK(
ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
IMIE VARCHAR(30) NOT NULL,
NAZWISKO VARCHAR(30) NOT NULL,
DATA_URODZENIA DATE NULL,
ILOSC_GOLI_W_SEZONIE INT NULL,
ILOSC_ASYST_W_SEZONIE INT NULL,
ID_ZESPOLU INT NULL)




CREATE TABLE czlonkowie_sztabu_trenerskiego
(ID int NOT NULL IDENTITY(1,1) PRIMARY KEY,
ID_TRENERA INT NULL,
ID_sztabu_medycznego INT NULL,
ID_ZAWODNIKA INT NULL)

CREATE TABLE Sztab_medyczny
(ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
nazwa varchar(30) NULL,
ID_Sztabu INT NULL,
ID_ZESPOLU INT NULL,)

CREATE TABLE TABELA_LACZNIKOWA_MECZ_ZESPOL(
ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
ID_ZESPOLU INT,
ID_MECZU INT);

alter table Zespol add constraint relacja_zespol_trener foreign key (ID_TRENERA) references TRENER(ID) on delete cascade on update cascade;
alter table Zawodnik add constraint relacja_zawodnik_zespol foreign key (ID_zespolu) references zespol(id) on delete cascade on update cascade;
alter table trener add constraint relacja_trener_zespol foreign key (ID_zespolu) references zespol(id) on delete NO ACTION on update NO ACTION;
alter table SZTAB_MEDYCZNY add constraint relacja_SZTAB_MEDYCZNY_ZESPOL foreign key (ID_zespolu) references zespol(id) on delete NO ACTION on update NO ACTION;
alter table SZTAB_MEDYCZNY add constraint relacja_SZTAB_MEDYCZNY_SZTAB_TRENERSKI foreign key (ID_SZTABU) references CZLONKOWIE_SZTABU_TRENERSKIEGO(ID) on delete NO ACTION on update NO ACTION;
alter table TABELA_LACZNIKOWA_MECZ_ZESPOL add constraint relacja_MECZ_ZESPOL FOREIGN KEY (ID_MECZU) references MECZ(ID) on delete cascade on update cascade;
alter table TABELA_LACZNIKOWA_MECZ_ZESPOL add constraint relacja_ZESPOL_MECZ FOREIGN KEY (ID_ZESPOLU) references ZESPOL(ID) on delete NO ACTION on update NO ACTION;
/*alter table MECZ add constraint relacja_MECZ_ZESPOL_GLOWNA  FOREIGN KEY (ID_ZESPOLU) references ZESPOL(ID_ZESPOLU) on delete NO ACTION on update NO ACTION;*/
alter table CZLONKOWIE_SZTABU_TRENERSKIEGO add constraint relacja_CZLONKOWIE_SZTABU_TRENERSKIEGO_TRENER  FOREIGN KEY (ID_TRENERA) references TRENER(ID) on delete NO ACTION on update NO ACTION;
alter table CZLONKOWIE_SZTABU_TRENERSKIEGO add constraint relacja_CZLONKOWIE_SZTABU_TRENERSKIEGO_ZAWODNIK  FOREIGN KEY (ID_ZAWODNIKA) references ZAWODNIK(ID) on delete NO ACTION on update NO ACTION;
alter table CZLONKOWIE_SZTABU_TRENERSKIEGO add constraint relacja_CZLONKOWIE_SZTABU_TRENERSKIEGO_SZTAB_MEDYCZNY  FOREIGN KEY (ID_SZTABU_MEDYCZNEGO) references SZTAB_MEDYCZNY(ID) on delete NO ACTION on update NO ACTION;