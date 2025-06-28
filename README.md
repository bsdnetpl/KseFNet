KSeFClientService – Integracja z Krajowym Systemem e-Faktur (KSeF)
📌 Opis projektu

KSeFClientService to implementacja serwisu oraz kontrolera REST API umożliwiająca integrację z Krajowym Systemem e-Faktur (KSeF). Klasa obsługuje proces uwierzytelniania w trybie FA(3), przesyłania faktur oraz zarządzania sesją.

Projekt został zaprojektowany zgodnie z zasadami architektury warstwowej i oparty na technologii ASP.NET Core.
✅ Funkcjonalności

Serwis obsługuje następujące operacje:

    Pobieranie wyzwania (Challenge) i czasu (Timestamp)
    Uzyskuje dane wymagane do uwierzytelnienia w KSeF.

    Szyfrowanie tokenu
    Realizowane przy użyciu klucza publicznego w formacie PEM i algorytmu RSA.

    Uzyskanie tokenu sesji (FA(3))
    Generuje token sesji, niezbędny do przesyłania faktur.

    Przesyłanie faktury
    Wysyła fakturę w formacie XML do systemu KSeF.

    Zamykanie sesji
    Kończy aktywną sesję w KSeF.

🖥️ Wymagania systemowe

    .NET 6 lub nowszy

    ASP.NET Core

    Klucz publiczny Ministerstwa Finansów w formacie PEM

    Faktury w formacie XML (zgodne z KSeF)

🧱 Struktura projektu

    IKSeFClientService
    Interfejs definiujący metody serwisu, takie jak: szyfrowanie tokenu, uzyskanie tokenu sesji, przesyłanie faktur, itp.

    KSeFClientService
    Implementacja interfejsu, odpowiedzialna za komunikację z API KSeF.

    KSeFController
    Kontroler REST API umożliwiający dostęp do metod serwisu przez HTTP.

⚙️ Instalacja i konfiguracja
1. Klonowanie repozytorium

git clone https://github.com/bsdnetpl/KSeFClientService.git
cd KSeFClientService

2. Dodanie klucza publicznego

Umieść plik klucza publicznego (publicKey.pem) w katalogu projektu i zaktualizuj jego ścieżkę w konfiguracji serwisu (np. przez DI lub appsettings.json).
3. Dodanie pliku faktury

Upewnij się, że plik faktury w formacie XML znajduje się w systemie plików i podaj jego ścieżkę w żądaniu.
4. Uruchomienie aplikacji

dotnet run

📡 Przykłady użycia API
1. Pobieranie Challenge i Timestamp

    Endpoint: GET /api/ksef/challenge

Przykład odpowiedzi:

{
  "challenge": "exampleChallenge",
  "challengeTime": "1630456789000"
}

2. Szyfrowanie tokenu

    Endpoint: POST /api/ksef/encrypt-token

Przykład żądania:

{
  "token": "yourApiToken",
  "challengeTimeMillis": "1630456789000"
}

Przykład odpowiedzi:

{
  "encryptedToken": "base64EncryptedToken"
}

3. Uzyskanie tokenu sesji

    Endpoint: POST /api/ksef/session-token

Przykład żądania:

{
  "encryptedToken": "base64EncryptedToken",
  "challenge": "exampleChallenge"
}

Przykład odpowiedzi:

{
  "sessionToken": "sessionTokenValue"
}

4. Wysyłanie faktury

    Endpoint: POST /api/ksef/send-invoice

Przykład żądania:

{
  "invoiceFilePath": "path/to/invoice.xml"
}

Przykład odpowiedzi:

{
  "message": "Invoice sent successfully",
  "response": { ... }
}

5. Zamykanie sesji

    Endpoint: POST /api/ksef/terminate-session

Przykład odpowiedzi:

{
  "message": "Session terminated successfully."
}

🧩 Konfiguracja w kodzie
1. Rejestracja serwisu w Program.cs

builder.Services.AddScoped<IKSeFClientService, KSeFClientService>();

2. Mapowanie kontrolera

app.MapControllers();

🧪 Testowanie API

Do testowania możesz użyć narzędzi takich jak:

    Postman

    curl

    Insomnia

📝 Licencja

Projekt dostępny na licencji MIT. Szczegóły znajdziesz w pliku LICENSE.
🤝 Wkład

Chcesz pomóc? Super!

    Otwórz zgłoszenie (Issue) w GitHubie

    Dodaj Pull Request

    Zgłoś propozycję nowych funkcji lub poprawek
