KSeFClientService - Integracja z Krajowym Systemem e-Faktur (KSeF)
Opis projektu

KSeFClientService to implementacja serwisu oraz kontrolera REST API umożliwiającego integrację z Krajowym Systemem e-Faktur (KSeF). Klasa obsługuje proces uwierzytelniania, przesyłania faktur oraz zarządzania sesją w systemie KSeF.

Projekt został zaprojektowany zgodnie z zasadami architektury warstwowej oraz wykorzystuje ASP.NET Core jako podstawę aplikacji.
Funkcjonalności

Serwis obsługuje następujące operacje:

    Pobieranie wyzwania (Challenge) i czasu (Timestamp):
        Umożliwia pobranie informacji wymaganych do uwierzytelnienia w KSeF.

    Szyfrowanie tokenu:
        Realizuje szyfrowanie tokenu przy użyciu klucza publicznego w formacie PEM.

    Uzyskanie tokenu sesji:
        Generuje token sesji niezbędny do przesyłania faktur.

    Przesyłanie faktury:
        Wysyła fakturę w formacie XML do systemu KSeF.

    Zamykanie sesji:
        Kończy aktywną sesję w KSeF.

Wymagania systemowe

    .NET 6 lub nowszy
    ASP.NET Core
    Klucz publiczny w formacie PEM
    Faktury w formacie XML

Struktura projektu

    IKSeFClientService:
        Interfejs definiujący metody serwisu, m.in. szyfrowanie tokenu, uzyskanie tokenu sesji, wysyłanie faktur.
    KSeFClientService:
        Implementacja serwisu komunikującego się z KSeF.
    KSeFController:
        Kontroler REST API umożliwiający dostęp do funkcji serwisu przez HTTP.

Instalacja i konfiguracja

    Klonowanie repozytorium:

git clone https://github.com/your-username/KSeFClientService.git
cd KSeFClientService

Dodanie klucza publicznego:

    Umieść plik klucza publicznego w formacie PEM w katalogu projektu i zaktualizuj jego ścieżkę w konfiguracji serwisu.

Dodanie pliku faktury:

    Upewnij się, że faktura jest dostępna w formacie XML i zaktualizuj jej ścieżkę w żądaniach API.

Uruchomienie aplikacji:

    dotnet run

Przykłady użycia API
1. Pobieranie wyzwania (Challenge) i czasu (Timestamp)

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

Konfiguracja w kodzie

Zarejestruj serwis w Program.cs:

builder.Services.AddScoped<IKSeFClientService, KSeFClientService>();

Zmapuj kontroler:

app.MapControllers();

Uruchom aplikację:

dotnet run

Testowanie API

Możesz użyć narzędzi takich jak Postman, Curl lub Insomnia do testowania punktów końcowych API.
Licencja

Projekt jest udostępniany na licencji MIT.
Wkład

Zapraszamy do zgłaszania problemów i składania próśb o nowe funkcje poprzez system zgłoszeń GitHub. Pull requesty są mile widziane!
