USE ScoreApp1;

-- 1. REJESTRACJA
INSERT INTO REJESTRACJA (IMIE, NAZWISKO, DATA_URODZENIA, EMAIL, HASLO)
VALUES 
('Jan', 'Kowalski', '1985-01-15', 'jan.kowalski@example.com', 'haslo123'),
('Anna', 'Nowak', '1990-02-20', 'anna.nowak@example.com', 'haslo456'),
('Piotr', 'Wiœniewski', '1988-03-10', 'piotr.w@example.com', 'haslo789');

-- 2. LOGOWANIE
INSERT INTO LOGOWANIE (EMAIL, HASLO)
VALUES 
('jan.kowalski@example.com', 'haslo123'),
('anna.nowak@example.com', 'haslo456'),
('piotr.w@example.com', 'haslo789');

-- 3. TRENER (tymczasowo ID_ZESPOLU = NULL)
INSERT INTO TRENER (IMIE, NAZWISKO, DATA_URODZENIA, ID_ZESPOLU, NAZWA_AKUTALNEJ_DRUZYNY)
VALUES 
('Marek', 'Zieliñski', '1970-05-15', NULL, 'FC Kraków'),
('Tomasz', 'Kowalczyk', '1965-07-20', NULL, 'Legia Warszawa');

-- 4. ZESPOL (tymczasowo id_trenera = NULL)
INSERT INTO ZESPOL (id_trenera, nazwa_zespolu, liczba_zawodnikow, ID_TABELI)
VALUES 
(NULL, 'FC Kraków', 22, NULL),
(NULL, 'Legia Warszawa', 20, NULL);

-- 5. Aktualizacja TRENER z prawid³owym ID_ZESPOLU
UPDATE TRENER SET ID_ZESPOLU = 1 WHERE ID = 1;
UPDATE TRENER SET ID_ZESPOLU = 2 WHERE ID = 2;

-- 6. Aktualizacja ZESPOL z prawid³owym id_trenera
UPDATE ZESPOL SET id_trenera = 1 WHERE ID = 1;
UPDATE ZESPOL SET id_trenera = 2 WHERE ID = 2;

-- 7. tabela_ligi
INSERT INTO tabela_ligi (liczba_zespolow, ID_zespolu)
VALUES 
(10, 1),
(10, 2);

-- 8. TABLEA_LACZNIKOWA_ZESPOL_TABELA
INSERT INTO TABLEA_LACZNIKOWA_ZESPOL_TABELA (ID_zespolu, ID_tabeli)
VALUES 
(1, 1),
(2, 2);

-- 9. MECZ
INSERT INTO MECZ (DATA_MECZU, SEDZIA, WYNIK, ZESPOL1, ZESPOL2)
VALUES 
('2024-06-15', 'Jan Nowak', '2-1', 'FC Kraków', 'Legia Warszawa'),
('2024-07-01', 'Anna Kowalska', '1-1', 'Legia Warszawa', 'FC Kraków');

-- 10. ZAWODNIK
INSERT INTO ZAWODNIK (IMIE, NAZWISKO, DATA_URODZENIA, ILOSC_GOLI_W_SEZONIE, ILOSC_ASYST_W_SEZONIE, ID_ZESPOLU)
VALUES 
('Robert', 'Lewandowski', '1988-08-21', 25, 10, 1),
('Kamil', 'Glik', '1988-02-03', 5, 2, 2);

-- 11. Sztab_medyczny
INSERT INTO Sztab_medyczny (nazwa, ID_Sztabu, ID_ZESPOLU)
VALUES 
('FC Kraków Medical Team', NULL, 1),
('Legia Warszawa Medical Team', NULL, 2);

-- 12. czlonkowie_sztabu_trenerskiego
INSERT INTO czlonkowie_sztabu_trenerskiego (ID_TRENERA, ID_sztabu_medycznego, ID_ZAWODNIKA)
VALUES 
(1, 1, 1),
(2, 2, 2);

-- 13. Aktualizacja Sztab_medyczny po dodaniu cz³onków sztabu
UPDATE Sztab_medyczny SET ID_Sztabu = 1 WHERE ID = 1;
UPDATE Sztab_medyczny SET ID_Sztabu = 2 WHERE ID = 2;

-- 14. TABELA_LACZNIKOWA_MECZ_ZESPOL
INSERT INTO TABELA_LACZNIKOWA_MECZ_ZESPOL (ID_ZESPOLU, ID_MECZU)
VALUES 
(1, 1),
(2, 2);
