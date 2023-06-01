# Laboratorium 11: ASP.NET core MVC oraz REST API - projekt aplikacji internetowej
## Programowanie zaawansowane 2

- Maksymalna liczba punktów: 10

- Skala ocen za punkty:
    - 9-10 ~ bardzo dobry (5.0)
    - 8 ~ plus dobry (4.5)
    - 7 ~ dobry (4.0)
    - 6 ~ plus dostateczny (3.5)
    - 5 ~ dostateczny (3.0)
    - 0-4 ~ niedostateczny (2.0)

Celem laboratorium jest wykonanie projektu aplikacji internetowej napisanej w technologii ASP.NET core MVC oraz REST API. Aplikacja będzie wykorzystywała mechanizm sesji do zapamiętania faktu zalogowania przez użytkownika. Dane wprowadzane do aplikacji będą zapamiętywane w bazie danych SQLite. Hasło w bazie będzie przechowywane w postaci skrótu (hash-u). Dostęp do bazy powinien odbywać się przy pomocy modelu danych w ramach MVC (nie poprzez natywne kwerendy do bazy).

- Projekt należy wykonać w grupach co najwyżej dwuosobowych.
- Projekt należy oddać na kolejnych zajęciach laboratoryjnych. Jeżeli projekt zostanie oddany po tym terminie, można otrzymać z niego co najwyżej ocenę dostateczną (3.0).
- Projekt musi spełniać następujące kryteria formalne:
    - Aplikacja internetowa musi korzystać z co najmniej 4 tabel bazy danych.
    - Zarządzanie danymi w tabelach musi być wykonane w całości przez interfejs webowy.
    - Pierwsze uruchomienie aplikacji musi dodawać wszystkie ewentualnie potrzebne do działania dane do bazy.
    - Aplikacja musi "robić coś więcej" niż tylko wyświetlać zawartości poszczególnych tabel - proszę przygotować jakieś ciekawe ich zestawienia. Na przykład jeżeli aplikacja gromadzi informacje o rozgrywkach ligowych drużyn piłkarskich mogłaby mieć możliwość wyświetlenia najskuteczniejszych zawodników, kalendarz nadchodzących rozgrywek, ranking drużyn itp.
    - Do poszczególnych widoków i funkcjonalności aplikacji powinno być możliwe dostanie się z poziomu menu / dostępnych na stronie linków itp., to znaczy że jeśli użytkownik zna domenę pod którą jest dostępna aplikacja, to czytając zawartość strony może po niej swobodnie nawigować bez wpisywania "ręcznie" jakiś adresów www.
    - Do aplikacji musi być dołączona dokumentacja (może być w formie strony internetowej, pliku w formacie md lub pdf) na której znajdzie się tytuł projektu, informacje o autorach, opis, do czego aplikacja służy oraz opis jej funkcjonalności / sposobu użycia. 

Wszystkie powyższe kryteria są obowiązkowe, aby aplikacja mogła zostać oceniona. Tematykę projektu proszę uzgodnić z prowadzącymi zajęcia!

Kryteria oceny:
- [5 punktów] Stworzenie aplikacji która implementuje zaproponowany temat w architekturze ASP.NET MCV core. Zamodeluj powiązania relacyjne pomiędzy tabelami w modelach!
- [2 punkty] Aby zrobić ten podpunkt należy wykonać wcześniejszy podpunkt. Należy wykonać zabezpieczenie aplikacji przed nieautoryzowanym dostępem poprzez dodanie logowania poprzez login i hasło. Jedynie zalogowani użytkownicy mają mieć możliwość dostępu do poszczególnych zasobów (widoków) projektu. Tabele potrzebne do obsługi logowania nie wliczają się do limitu co najmniej 4 tabel projektu. Podczas pierwszego uruchomienia aplikacji ma być stworzony pierwszy użytkownik, który będzie pełnił rolę administratora. Tylko administrator ma prawo dodawać nowych użytkowników oraz wyświetlać informacje o istniejących w systemie użytkownikach. Hasło w bazie proszę przechowywać w postaci skrótu (hashu).
- [3 punkty] Aby zrobić ten podpunkt należy wykonać również oba wcześniejsze podpunkty. Dodaj do aplikacji możliwość dodawania, usuwania, modyfikacji oraz wyświetlania danych przy użyciu REST API. Autentykacja oraz autoryzacja żądań REST powinna się odbywać w oparciu o dodatkowo przesyłane w żądaniach nazwę użytkownika oraz klucz/token, który jest niepustym ciągiem znaków. Każdy użytkownik ma przypisany do swojego konta taki klucz. Przygotuj programy konsolowe demonstrujące prawidłowość działania REST API.

Powodzenia!
(҂◡_◡) ᕤ