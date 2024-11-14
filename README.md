## Wymagania

- [.NET Core SDK (minimum wersja 3.1)](https://dotnet.microsoft.com/download/dotnet)
- [Node.js i npm](https://nodejs.org/en/)

## Klonowanie Repozytorium

```bash
git clone https://github.com/m4rc1nn/software-studio-task.git
cd software-studio-task
```
## Uruchomienie backendu:
```bash
cd backend
dotnet restore
dotnet build
dotnet run
```
## Uruchamianie frontendu
```bash
cd frontend
```
Utwórz plik .env i przekopiuj zawartość z .env.example
```bash
VUE_APP_API_URL=TUTAJ_DODAJ_LINK_Z_SERWERA_DOTNET_Z_KONSOLI
```
```bash
npm install
npm run serve
```
## Opcjonalnie
Sprawdź czy CorsOrigin z appsettings.json (folder backend) posiada poprawny link do aplikacji Vue.js (localhost:8080).
