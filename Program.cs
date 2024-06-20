using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Data;
using System.Threading;





namespace ScoreApp
{
    class Program
    {
        public static string ConnectionString = "Data Source=WINDELL-DMU7VR7;Database=ScoreApp;Integrated Security=SSPI";
        public static SqlConnection conn = new SqlConnection(ConnectionString);
        public static bool czy_uzytkownik_jest_zalogowany = false;
        public static bool czy_gosc = false;


        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            int currentOption = 0;
            int maxOptions = 3;

            while (true)
            {
                Console.CursorVisible = false;
                Console.Clear();
                Console.WriteLine("Wybierz opcję:");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(currentOption == 0 ? "[X] Rejestracja" : "[ ] Rejestracja");
                Console.WriteLine(currentOption == 1 ? "[X] Logowanie" : "[ ] Logowanie");
                Console.WriteLine(currentOption == 2 ? "[X] Wyjście" : "[ ] Wyjście");

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                ConsoleKey key = keyInfo.Key;

                if (key == ConsoleKey.UpArrow && currentOption > 0)
                    currentOption--;
                else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                    currentOption++;
                else if (key == ConsoleKey.Enter)
                {
                    switch (currentOption)
                    {
                        case 0:
                            MenuRejestracja();
                            break;
                        case 1:
                            MenuLogowanie();
                            break;
                        case 2:
                            Environment.Exit(0);
                            break;
                    }
                }
            }
        }




        static void MenuRejestracja()
        {
            Console.CursorVisible = false;
            int currentOption = 0;
            int maxOptions = 5;
            while (true)
            {
                Console.CursorVisible = false;
                Console.Clear();
                Console.WriteLine("Wybierz opcję:");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(currentOption == 0 ? "[X] ZAREJESTRUJ NOWEGO UZYTKOWNIKA (INSERT)" : "[ ] ZAREJESTRUJ NOWEGO UZYTKOWNIKA (INSERT)");
                Console.WriteLine(currentOption == 1 ? "[X] UAKTUALNIJ DANE UZYTKOWNIKA (UPDATE)" : "[ ] UAKTUALNIJ DANE UZYTKOWNIKA (UPDATE)");
                Console.WriteLine(currentOption == 2 ? "[X] USUN DANE UZYTKOWNIKA (DELETE)" : "[ ] USUN DANE UZYTKOWNIKA (DELETE)");
                Console.WriteLine(currentOption == 3 ? "[X] WYSWIETL DANE UZYTKOWNIKOW (SELECT)" : "[ ] WYSWIETL DANE UZYTKOWNIKOW (SELECT)");
                Console.WriteLine(currentOption == 4 ? "[X] Powrót" : "[ ] Powrót");
                ConsoleKeyInfo keyInfo = Console.ReadKey();
                ConsoleKey key = keyInfo.Key;

                if (key == ConsoleKey.UpArrow && currentOption > 0)
                    currentOption--;
                else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                    currentOption++;
                else if (key == ConsoleKey.Enter)
                {
                    switch (currentOption)
                    {
                        case 0:
                            insertRejestracja();
                            break;
                        case 1:
                            UpdateRejestracja();
                            break;
                        case 2:
                            DeleteRejestracja();
                            break;
                        case 3:
                            SelectRejestracja();
                            break;
                        case 4:
                            return;
                        default:
                            Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                            break;
                    }

                    static void insertRejestracja()
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.WriteLine("Podaj imię (max 14 znakow): ");
                        string imie = Console.ReadLine();

                        Console.WriteLine("Podaj nazwisko (max 18 znakow): ");
                        string nazwisko = Console.ReadLine();

                        DateTime data_urodzenia = DateTime.MinValue;
                        bool validDate = false;

                        while (!validDate)
                        {
                            Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
                            string dataUrodzeniaString = Console.ReadLine();

                            if (DateTime.TryParse(dataUrodzeniaString, out data_urodzenia))
                            {
                                validDate = true;
                            }
                            else
                            {
                                Console.WriteLine("Źle podana data. Spróbuj ponownie.");
                            }
                        }

                        Console.WriteLine("Podaj adres e-mail (max 24 znaki) :");
                        string email = Console.ReadLine();

                        Console.WriteLine("Podaj hasło (max 24 znaki) :");
                        string haslo = Console.ReadLine();

                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            // Sprawdź dostępność adresu e-mail
                            if (SprawdzDostepnoscEmaila(email))
                            {
                                Console.WriteLine("Ten adres e-mail jest już zarejestrowany. Proszę się zalogować!");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }
                            string sql = $"INSERT INTO REJESTRACJA (Imie, Nazwisko, Data_Urodzenia, Email, Haslo) VALUES ('{imie}', '{nazwisko}', '{data_urodzenia:yyyy-MM-dd}', '{email}', '{haslo}')";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord dodany do tabeli REJESTRACJA.");
                            Console.WriteLine();
                            Console.CursorVisible = false;
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }

                        }
                    }
                    static bool SprawdzDostepnoscEmaila(string email)
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }

                        string sql = $"SELECT COUNT(*) FROM REJESTRACJA WHERE Email = '{email}'";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            return true; // Adres e-mail jest już zarejestrowany
                        }

                        return false; // Adres e-mail jest dostępny
                    }

                }

                static void UpdateRejestracja()
                {
                    Console.Clear();
                    Console.CursorVisible = true;
                    Console.WriteLine("Podaj ID użytkownika, którego chcesz zaktualizować:");
                    int id;
                    while (!int.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                    }



                    try
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        string sql = $"SELECT * FROM REJESTRACJA WHERE ID={id}";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (!reader.HasRows)
                        {
                            Console.WriteLine($"Nie znaleziono użytkownika o ID={id}.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            return;
                        }

                        reader.Close();

                        Console.WriteLine("Podaj nowe dane użytkownika:");

                        Console.WriteLine("Podaj imię:");
                        string imie = Console.ReadLine();

                        Console.WriteLine("Podaj nazwisko:");
                        string nazwisko = Console.ReadLine();

                        Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
                        DateTime data_urodzenia = DateTime.Parse(Console.ReadLine());

                        Console.WriteLine("Podaj adres e-mail:");
                        string email = Console.ReadLine();

                        Console.WriteLine("Podaj hasło:");
                        string haslo = Console.ReadLine();

                        sql = $"UPDATE REJESTRACJA SET Imie='{imie}', Nazwisko='{nazwisko}', Data_Urodzenia='{data_urodzenia:yyyy-MM-dd}', Email='{email}', Haslo='{haslo}' WHERE ID={id}";
                        cmd = new SqlCommand(sql, conn);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} rekord zaktualizowany w tabeli REJESTRACJA.");
                        Console.WriteLine();
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }



                }

                static void DeleteRejestracja()
                {
                    Console.Clear();
                    Console.CursorVisible = true;
                    Console.WriteLine("Podaj ID użytkownika do usunięcia:");
                    int id = int.Parse(Console.ReadLine());



                    try
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        string sql = $"DELETE FROM REJESTRACJA WHERE ID = {id}";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {

                            Console.WriteLine($"{rowsAffected} rekord usunięty z tabeli rejestracja.");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("nie usunieto zadnego rekordu z tabeli");
                        }
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }


                }


                static void SelectRejestracja()
                {

                    Console.Clear();
                    try
                    {
                        if (conn.State != ConnectionState.Open)
                        {
                            conn.Open();
                        }
                        string sql = "SELECT * FROM REJESTRACJA";
                        SqlCommand cmd = new SqlCommand(sql, conn);
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (!reader.HasRows)
                        {

                            Console.WriteLine("Nie znaleziono użytkowników w tabeli REJESTRACJA.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            reader.Close();
                            return;
                        }



                        Console.WriteLine("+--------+--------------+------------------+----------------+------------------------+------------------------+");
                        Console.WriteLine("|   ID   |     Imię     |     Nazwisko     | Data urodzenia |          Email         |         Hasło          |");
                        Console.WriteLine("+--------+--------------+------------------+----------------+------------------------+------------------------+");

                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("ID"));
                            string imie = reader.GetString(reader.GetOrdinal("Imie"));
                            string nazwisko = reader.GetString(reader.GetOrdinal("Nazwisko"));
                            DateTime data_urodzenia = reader.GetDateTime(reader.GetOrdinal("Data_Urodzenia"));
                            string email = reader.GetString(reader.GetOrdinal("Email"));
                            string haslo = reader.GetString(reader.GetOrdinal("Haslo"));

                            Console.WriteLine("|{0,-8}|{1,-14}|{2,-18}|{3,-16}|{4,-24}|{5,-24}|", id, imie, nazwisko, data_urodzenia.ToString("yyyy-MM-dd"), email, haslo);
                        }
                        Console.WriteLine();
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();
                        reader.Close();

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                        Console.WriteLine();
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }



            }
        }
        static void MenuLogowanie()
        {
            Console.CursorVisible = false;
            int currentOption = 0;
            int maxOptions = 3;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Wybierz opcję:");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(currentOption == 0 ? "[X] Kontynuuj jako gość" : "[ ] Kontynuuj jako gość");
                Console.WriteLine(currentOption == 1 ? "[X] Zaloguj się na zarejestrowane konto" : "[ ] Zaloguj się na zarejestrowane konto");
                Console.WriteLine(currentOption == 2 ? "[X] Powrót" : "[ ] Powrót");

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                ConsoleKey key = keyInfo.Key;

                if (key == ConsoleKey.UpArrow && currentOption > 0)
                    currentOption--;
                else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                    currentOption++;
                else if (key == ConsoleKey.Enter)
                {
                    switch (currentOption)
                    {
                        case 0:
                            funkcje_goscia();
                            break;
                        case 1:
                            Logowanie();
                            break;
                        case 2:
                            return;
                        default:
                            Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                            break;
                    }
                }

            }
            static void funkcje_goscia()
            {
                Console.Clear();
                //Console.WriteLine("Gość akutlanie nie ma żadnych funkcji, zaloguj się aby konntynuować.");
                menuPoZalogowaniu();
                Console.ReadKey();

            }

            static void Logowanie()
            {
                Console.Clear();
                Console.WriteLine("Podaj adres e-mail:");
                string email = Console.ReadLine();

                Console.WriteLine("Podaj hasło:");
                string haslo = Console.ReadLine();

                try
                {
                    if (conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    string sql = $"SELECT Haslo FROM REJESTRACJA WHERE Email='{email}'";
                    SqlCommand cmd = new SqlCommand(sql, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                    {
                        Console.WriteLine($"Nie znaleziono użytkownika o adresie e-mail {email}.");
                        Console.WriteLine();
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();

                        return;
                    }

                    reader.Read();
                    string storedPassword = reader.GetString(0);
                    reader.Close();

                    if (haslo == storedPassword)
                    {
                        Console.WriteLine("Zalogowano.");
                        Console.WriteLine();
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();

                        czy_uzytkownik_jest_zalogowany = true;
                        menuPoZalogowaniu();
                    }
                    else
                    {
                        Console.WriteLine("Błędne hasło.");
                        Console.WriteLine();
                        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                        Console.ReadKey();

                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }
        static void menuPoZalogowaniu()
        {

            int currentOption = 0;
            int maxOptions = 4;

            while (true)
            {

                Console.Clear();
                Console.WriteLine("Wybierz opcję:");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(currentOption == 0 ? "[X] Powrót" : "[ ] Powrót");
                Console.WriteLine(currentOption == 1 ? "[X] Zarządzaj meczami" : "[ ] Zarządzaj meczami");
                Console.WriteLine(currentOption == 2 ? "[X] Zarządzaj zawodnikami" : "[ ] Zarządzaj zawodnikami");
                Console.WriteLine(currentOption == 3 ? "[X] Zarządzaj trenerami" : "[ ] Zarządzaj trenerami");
                Console.WriteLine("");
                Console.WriteLine("note: jezeli jestes w trybie 'gość' nie masz dostępu do zarządzania czymkolwiek.");

                ConsoleKeyInfo keyInfo = Console.ReadKey();
                ConsoleKey key = keyInfo.Key;

                if (key == ConsoleKey.UpArrow && currentOption > 0)
                    currentOption--;
                else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                    currentOption++;
                else if (key == ConsoleKey.Enter)
                {
                    switch (currentOption)
                    {
                        case 0:
                            return;
                        case 1:
                            Mecze();
                            break;
                        case 2:
                            Zawodnicy();
                            break;
                        case 3:
                            Trenerzy();
                            break;

                        default:
                            Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                            break;
                    }
                }
            }

            static void Mecze()
            {
                if (czy_uzytkownik_jest_zalogowany)
                {
                    int currentOption = 0;
                    int maxOptions = 5;

                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Wybierz opcję:");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(currentOption == 0 ? "[X] DODAJ MECZ (INSERT)" : "[ ] DODAJ MECZ (INSERT)");
                        Console.WriteLine(currentOption == 1 ? "[X] UAKTUALNIJ DANE O MECZU (UPDATE)" : "[ ] UAKTUALNIJ DANE O MECZU (UPDATE)");
                        Console.WriteLine(currentOption == 2 ? "[X] USUN MECZ (DELETE)" : "[ ] USUN MECZ (DELETE)");
                        Console.WriteLine(currentOption == 3 ? "[X] WYSWIETL MECZ (SELECT)" : "[ ] WYSWIETL MECZ (SELECT)");
                        Console.WriteLine(currentOption == 4 ? "[X] Powrót" : "[ ] Powrót");

                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        ConsoleKey key = keyInfo.Key;

                        if (key == ConsoleKey.UpArrow && currentOption > 0)
                            currentOption--;
                        else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                            currentOption++;
                        else if (key == ConsoleKey.Enter)
                        {
                            switch (currentOption)
                            {
                                case 0:
                                    insertMecz();
                                    break;
                                case 1:
                                    UpdateMecz();
                                    break;
                                case 2:
                                    DeleteMecz();
                                    break;
                                case 3:
                                    SelectMecz();
                                    break;
                                case 4:
                                    return;
                                default:
                                    Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                                    break;
                            }
                        }
                    }

                    static void insertMecz()
                    {
                        Console.Clear();
                        Console.WriteLine("Podaj nazwe pierwszej druzyny: (max 14 znakow)");
                        string druzyna1 = Console.ReadLine();

                        Console.WriteLine("Podaj nazwe drugiej druzyny: (max 14 znakow)");
                        string druzyna2 = Console.ReadLine();

                        Console.WriteLine("Podaj datę meczu (w formacie YYYY-MM-DD):");
                        DateTime data_meczu = DateTime.Parse(Console.ReadLine());

                        Console.WriteLine("Podaj wynik meczu: (max 5 znakow, w formacie X-Y np. 2-0)");
                        string wynik = Console.ReadLine();

                        Console.WriteLine("Podaj imię sędziego: (max 16 znakow)");
                        string sedzia = Console.ReadLine();

                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"INSERT INTO MECZ (zespol1, zespol2, DATA_MECZU, WYNIK, SEDZIA) VALUES ('{druzyna1}', '{druzyna2}', '{data_meczu:yyyy-MM-dd}', '{wynik}', '{sedzia}')";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord dodany do tabeli MECZ.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                    }


                    static void UpdateMecz()
                    {
                        Console.Clear();
                        Console.WriteLine("Podaj ID MECZU, który chcesz zaktualizować:");
                        int id;
                        while (!int.TryParse(Console.ReadLine(), out id))
                        {
                            Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                        }



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"SELECT * FROM MECZ WHERE ID={id}";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                Console.WriteLine($"Nie znaleziono użytkownika o ID={id}.");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }

                            reader.Close();

                            Console.WriteLine("Podaj nowe dane meczu.");
                            Console.CursorVisible = true;
                            Console.WriteLine("Podaj nazwe pierwszej druzyny (gospodarze): ");
                            string druzyna1 = Console.ReadLine();

                            Console.WriteLine("Podaj nazwe drugiej druzyny (goscie): ");
                            string druzyna2 = Console.ReadLine();

                            Console.WriteLine("Podaj datę meczu (w formacie YYYY-MM-DD): ");
                            DateTime data_meczu = DateTime.Parse(Console.ReadLine());

                            Console.WriteLine("Podaj wynik meczu: ");
                            string wynik = Console.ReadLine();

                            Console.WriteLine("Podaj imię sędziego: ");
                            string sedzia = Console.ReadLine();

                            sql = $"UPDATE MECZ SET zespol1='{druzyna1}', zespol2='{druzyna2}', DATA_MECZU='{data_meczu:yyyy-MM-dd}', wynik='{wynik}', sedzia='{sedzia}' WHERE ID={id}";
                            cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord zaktualizowany w tabeli MECZ.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }




                    }
                    static void DeleteMecz()
                    {
                        Console.Clear();
                        Console.WriteLine("Podaj ID meczu do usunięcia:");
                        int id = int.Parse(Console.ReadLine());



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"DELETE FROM mecz WHERE ID = {id}";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {

                                Console.WriteLine($"{rowsAffected} rekord usunięty z tabeli mecz.");
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine("nie usunieto zadnego rekordu z tabeli");
                            }
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                    }
                    static void SelectMecz()
                    {
                        Console.Clear();
                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = "SELECT * FROM mecz";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Nie znaleziono meczy");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }

                            Console.WriteLine("+--------+--------------+--------------+----------------+----------------+----------------+");
                            Console.WriteLine("|   ID   |  gospodarze  |    goscie    |     wynik      |   data meczu   |     sedzia     |");
                            Console.WriteLine("+--------+--------------+--------------+----------------+----------------+----------------+");


                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("ID"));
                                string zespol1 = reader.GetString(reader.GetOrdinal("zespol1"));
                                string zespol2 = reader.GetString(reader.GetOrdinal("zespol2"));
                                string wynik = reader.GetString(reader.GetOrdinal("wynik"));
                                DateTime data_meczu = reader.GetDateTime(reader.GetOrdinal("Data_meczu"));
                                string sedzia = reader.GetString(reader.GetOrdinal("sedzia"));

                                Console.WriteLine("|{0,-8}|{1,-14}|{2,-14}|{3,-16}|{4,-16}|{5,-16}|", id, zespol1, zespol2, wynik, data_meczu.ToString("yyyy-MM-dd"), sedzia);
                            }
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            reader.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }



                    }


                }

            }
            static void Zawodnicy()
            {
                if (czy_uzytkownik_jest_zalogowany)
                {
                    int currentOption = 0;
                    int maxOptions = 5;

                    while (true)
                    {
                        Console.Clear();
                        Console.CursorVisible = false;
                        Console.WriteLine("Wybierz opcję:");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(currentOption == 0 ? "[X] DODAJ ZAWODNIKA (INSERT)" : "[ ] DODAJ ZAWODNIKA (INSERT)");
                        Console.WriteLine(currentOption == 1 ? "[X] UAKTUALNIJ DANE O ZAWODNIKU (UPDATE)" : "[ ] UAKTUALNIJ DANE O ZAWODNIKU (UPDATE)");
                        Console.WriteLine(currentOption == 2 ? "[X] USUN ZAWODNIKA (DELETE)" : "[ ] USUN ZAWODNIKA (DELETE)");
                        Console.WriteLine(currentOption == 3 ? "[X] WYSWIETL ZAWODINKOW (SELECT)" : "[ ] WYSWIETL ZAWODNIKOW (SELECT)");
                        Console.WriteLine(currentOption == 4 ? "[X] Powrót" : "[ ] Powrót");

                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        ConsoleKey key = keyInfo.Key;

                        if (key == ConsoleKey.UpArrow && currentOption > 0)
                            currentOption--;
                        else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                            currentOption++;
                        else if (key == ConsoleKey.Enter)
                        {
                            switch (currentOption)
                            {
                                case 0:
                                    insertZAWODNIK();
                                    break;
                                case 1:
                                    UpdateZAWODNIK();
                                    break;
                                case 2:
                                    DeleteZAWODNIK();
                                    break;
                                case 3:
                                    SelectZAWODNIK();
                                    break;
                                case 4:
                                    return;
                                default:
                                    Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                                    break;
                            }
                        }
                    }

                    static void insertZAWODNIK()
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.WriteLine("Podaj imie zawodnika (max 14 znakow)");
                        string imie = Console.ReadLine();

                        Console.WriteLine("Podaj nazwisko zawodnika (max 16 znakow)");
                        string nazwisko = Console.ReadLine();

                        DateTime data_urodzenia_zawodnika = DateTime.MinValue;
                        bool validDate = false;

                        while (!validDate)
                        {
                            Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
                            string dataUrodzeniaString = Console.ReadLine();

                            if (DateTime.TryParse(dataUrodzeniaString, out data_urodzenia_zawodnika))
                            {
                                validDate = true;
                            }
                            else
                            {
                                Console.WriteLine("Źle podana data. Spróbuj ponownie.");
                            }
                        }

                        Console.WriteLine("Podaj ilosc goli w sezonie strzelonych przez zawodnika (mozna wprowadzic tylko liczby całkowite)");
                        int gole_sezon = int.Parse(Console.ReadLine());

                        Console.WriteLine("Podaj ilosc asyst w sezonie zdobytych przez zawodnika (mozna wprowadzic tylko liczby całkowite)");
                        int asysty_sezon = int.Parse(Console.ReadLine());

                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"INSERT INTO ZAWODNIK (imie, nazwisko, data_urodzenia, ilosc_goli_w_sezonie, ilosc_asyst_w_sezonie) VALUES ('{imie}', '{nazwisko}', '{data_urodzenia_zawodnika:yyyy-MM-dd}', '{gole_sezon}', '{asysty_sezon}')";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord dodany do tabeli MECZ.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                    }


                    static void UpdateZAWODNIK()
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.WriteLine("Podaj ID zawodnika, który chcesz zaktualizować:");
                        int id;
                        while (!int.TryParse(Console.ReadLine(), out id))
                        {
                            Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                        }



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"SELECT * FROM ZAWODNIK WHERE ID={id}";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                Console.WriteLine($"Nie znaleziono zawodnika o ID={id}.");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }

                            reader.Close();

                            Console.WriteLine("Podaj nowe dane zawodnika.");
                            Console.CursorVisible = true;
                            Console.WriteLine("Podaj imie zawodnika: ");
                            string imie = Console.ReadLine();

                            Console.WriteLine("Podaj nazwisko zawodnika: ");
                            string nazwisko = Console.ReadLine();

                            DateTime data_urodzenia_zawodnika = DateTime.MinValue;
                            bool validDate = false;

                            while (!validDate)
                            {
                                Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
                                string dataUrodzeniaString = Console.ReadLine();

                                if (DateTime.TryParse(dataUrodzeniaString, out data_urodzenia_zawodnika))
                                {
                                    validDate = true;
                                }
                                else
                                {
                                    Console.WriteLine("Źle podana data. Spróbuj ponownie.");
                                }
                            }

                            Console.WriteLine("Podaj ilosc goli w sezonie strzelonych przez zawodnika:  (mozna wprowadzic tylko liczby całkowite)");
                            int gole_sezon = int.Parse(Console.ReadLine());

                            Console.WriteLine("Podaj liczbe asyst zdobytych przez zawodnika w tym sezonie:  (mozna wprowadzic tylko liczby całkowite)");
                            int asysty_sezon = int.Parse(Console.ReadLine());

                            sql = $"UPDATE ZAWODNIK SET imie='{imie}', nazwisko='{nazwisko}', DATA_URODZENIA='{data_urodzenia_zawodnika:yyyy-MM-dd}', ILOSC_GOLI_W_SEZONIE='{gole_sezon}', ILOSC_ASYST_W_SEZONIE='{asysty_sezon}' WHERE ID={id}";
                            cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord zaktualizowany w tabeli ZAWODNIK.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }




                    }
                    static void DeleteZAWODNIK()
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.WriteLine("Podaj ID zawodnika do usunięcia:");
                        int id = int.Parse(Console.ReadLine());



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"DELETE FROM zawodnik WHERE ID = {id}";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {

                                Console.WriteLine($"{rowsAffected} rekord usunięty z tabeli zawodnik.");
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine("nie usunieto zadnego rekordu z tabeli");
                            }
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                    }
                    static void SelectZAWODNIK()
                    {
                        Console.CursorVisible = false;
                        Console.Clear();
                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = "SELECT * FROM zawodnik";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Nie znaleziono zawodnikow");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }

                            Console.WriteLine("+--------+--------------+----------------+----------------+----------------+----------------+");
                            Console.WriteLine("|   ID   |    imie      |    nazwisko    |      gole      |  data urodznia |     asysty     |");
                            Console.WriteLine("+--------+--------------+----------------+----------------+----------------+----------------+");


                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("ID"));
                                string imie = reader.GetString(reader.GetOrdinal("imie"));
                                string nazwisko = reader.GetString(reader.GetOrdinal("nazwisko"));
                                int gole_sezon = reader.GetInt32(reader.GetOrdinal("ILOSC_GOLI_W_SEZONIE"));
                                DateTime data_urodzenia_zawodnika = reader.GetDateTime(reader.GetOrdinal("DATA_URODZENIA"));
                                int asysty_sezon = reader.GetInt32(reader.GetOrdinal("ILOSC_ASYST_W_SEZONIE"));

                                Console.WriteLine("|{0,-8}|{1,-14}|{2,-16}|{3,-16}|{4,-16}|{5,-16}|", id, imie, nazwisko, gole_sezon, data_urodzenia_zawodnika.ToString("yyyy-MM-dd"), asysty_sezon);
                            }
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            reader.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }



                    }


                }

            }
            static void Trenerzy()
            {
                if (czy_uzytkownik_jest_zalogowany)
                {
                    int currentOption = 0;
                    int maxOptions = 5;

                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("Wybierz opcję:");
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.WriteLine(currentOption == 0 ? "[X] DODAJ trenera (INSERT)" : "[ ] DODAJ trenera (INSERT)");
                        Console.WriteLine(currentOption == 1 ? "[X] UAKTUALNIJ DANE O trenerze (UPDATE)" : "[ ] UAKTUALNIJ DANE O trenerze (UPDATE)");
                        Console.WriteLine(currentOption == 2 ? "[X] USUN trenera (DELETE)" : "[ ] USUN trenera (DELETE)");
                        Console.WriteLine(currentOption == 3 ? "[X] WYSWIETL trenerów (SELECT)" : "[ ] WYSWIETL trenerów (SELECT)");
                        Console.WriteLine(currentOption == 4 ? "[X] Powrót" : "[ ] Powrót");

                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        ConsoleKey key = keyInfo.Key;

                        if (key == ConsoleKey.UpArrow && currentOption > 0)
                            currentOption--;
                        else if (key == ConsoleKey.DownArrow && currentOption < maxOptions - 1)
                            currentOption++;
                        else if (key == ConsoleKey.Enter)
                        {
                            switch (currentOption)
                            {
                                case 0:
                                    insertTrener();
                                    break;
                                case 1:
                                    UpdateTrener();
                                    break;
                                case 2:
                                    DeleteTrener();
                                    break;
                                case 3:
                                    SelectTrener();
                                    break;
                                case 4:
                                    return;
                                default:
                                    Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                                    break;
                            }
                        }
                    }

                    static void insertTrener()
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.WriteLine("Podaj imie trenera (max 16 znakow)");
                        string imie = Console.ReadLine();

                        Console.WriteLine("Podaj nazwisko trenera (max 19 znakow)");
                        string nazwisko = Console.ReadLine();

                        DateTime data_urodzenia_trenera = DateTime.MinValue;
                        bool validDate = false;

                        while (!validDate)
                        {
                            Console.WriteLine("Podaj datę urodzenia (w formacie YYYY-MM-DD):");
                            string dataUrodzeniaString = Console.ReadLine();

                            if (DateTime.TryParse(dataUrodzeniaString, out data_urodzenia_trenera))
                            {
                                validDate = true;
                            }
                            else
                            {
                                Console.WriteLine("Źle podana data. Spróbuj ponownie.");
                            }
                        }

                        Console.WriteLine("Podaj nazwe aktualnie trenowanej druzyny (max 20 znakow)");
                        string druzyna = Console.ReadLine();



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"INSERT INTO trener (imie, nazwisko, data_urodzenia, nazwa_aktualnej_druzyny) VALUES ('{imie}', '{nazwisko}', '{data_urodzenia_trenera:yyyy-MM-dd}', '{druzyna}')";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord dodany do tabeli MECZ.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                    }


                    static void UpdateTrener()
                    {
                        Console.Clear();
                        Console.CursorVisible = true;
                        Console.WriteLine("Podaj ID Trenera, ktorego chcesz zaktualizować:");
                        int id;
                        while (!int.TryParse(Console.ReadLine(), out id))
                        {
                            Console.WriteLine("Niepoprawna wartość, spróbuj ponownie.");
                        }



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"SELECT * FROM Trener WHERE ID={id}";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                Console.WriteLine($"Nie znaleziono Trenera o ID={id}.");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }

                            reader.Close();

                            Console.WriteLine("Podaj nowe dane Trenera.");
                            Console.CursorVisible = true;
                            Console.WriteLine("Podaj imie Trenera: (max 16 znakow)");
                            string imie = Console.ReadLine();

                            Console.WriteLine("Podaj nazwisko Trenera: (max 19 znakow)");
                            string nazwisko = Console.ReadLine();

                            Console.WriteLine("Podaj datę urodzenia Trenera (w formacie YYYY-MM-DD): ");
                            DateTime data_urodzenia_zawodnika = DateTime.Parse(Console.ReadLine());

                            Console.WriteLine("Podaj akutalna druzyne trenera:  (max 20 znakow)");
                            string druzyna = Console.ReadLine();



                            sql = $"UPDATE Trener SET imie='{imie}', nazwisko='{nazwisko}', DATA_URODZENIA='{data_urodzenia_zawodnika:yyyy-MM-dd}', nazwa_aktualnej_druzyny='{druzyna}' WHERE ID={id}";
                            cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            Console.WriteLine($"{rowsAffected} rekord zaktualizowany w tabeli ZAWODNIK.");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }




                    }
                    static void DeleteTrener()
                    {
                        Console.Clear();
                        Console.WriteLine("Podaj ID Trenera do usunięcia:");
                        int id = int.Parse(Console.ReadLine());



                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = $"DELETE FROM Trener WHERE ID = {id}";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {

                                Console.WriteLine($"{rowsAffected} rekord usunięty z tabeli trener.");
                                Console.WriteLine();
                            }
                            else
                            {
                                Console.WriteLine("nie usunieto zadnego rekordu z tabeli");
                            }
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }
                    }
                    static void SelectTrener()
                    {
                        Console.Clear();
                        try
                        {
                            if (conn.State != ConnectionState.Open)
                            {
                                conn.Open();
                            }
                            string sql = "SELECT * FROM Trener";
                            SqlCommand cmd = new SqlCommand(sql, conn);
                            SqlDataReader reader = cmd.ExecuteReader();

                            if (!reader.HasRows)
                            {
                                Console.WriteLine("Nie znaleziono Trenerow");
                                Console.WriteLine();
                                Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                                Console.ReadKey();
                                return;
                            }

                            Console.WriteLine("+--------+----------------+-------------------+--------------------+--------------------+");
                            Console.WriteLine("|   ID   |      imie      |      nazwisko     |       druzyna      |    data urodzenia  |");
                            Console.WriteLine("+--------+----------------+-------------------+--------------------+--------------------+");


                            while (reader.Read())
                            {
                                int id = reader.GetInt32(reader.GetOrdinal("ID"));
                                string imie = reader.GetString(reader.GetOrdinal("imie"));
                                string nazwisko = reader.GetString(reader.GetOrdinal("nazwisko"));
                                string druzyna = reader.GetString(reader.GetOrdinal("nazwa_aktualnej_druzyny"));
                                DateTime data_urodzenia = reader.GetDateTime(reader.GetOrdinal("data_urodzenia"));


                                Console.WriteLine("|{0,-8}|{1,-16}|{2,-19}|{3,-20}|{4,-20}|", id, imie, nazwisko, druzyna, data_urodzenia.ToString("yyyy-MM-dd"));
                            }
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                            reader.Close();

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
                            Console.WriteLine();
                            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
                            Console.ReadKey();
                        }
                        finally
                        {
                            if (conn.State == ConnectionState.Open)
                            {
                                conn.Close();
                            }
                        }



                    }


                }

            }
        }


    }
}

