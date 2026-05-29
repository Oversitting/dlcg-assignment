# Video Game Catalogue

This repository contains an interview assignment implementation for a simple two-page video game catalogue.

The application includes:

- An ASP.NET Core Web API with EF Core and SQL Server Code First
- An Angular frontend with Bootstrap and ng-bootstrap
- Backend unit and API-level tests
- A root PowerShell runner script for building and launching the full stack

## Repository Structure

- `Server/VideoGameCatalogue.Api` - ASP.NET Core API, EF Core data access, validation, startup, migrations
- `Frontend` - Angular application for browsing and editing catalogue entries
- `Tests/VideoGameCatalogue.Tests` - xUnit test project for backend behavior
- `run-app.ps1` - one-command local runner for the full app
- `requirements.txt` - original assignment brief

## Tech Stack

- .NET 8
- ASP.NET Core Web API
- EF Core 8 with SQL Server
- Angular 21
- Bootstrap 5 and ng-bootstrap
- xUnit and EF Core InMemory for backend tests

## Features

- Browse catalogue entries
- Create and edit entries
- Delete entries
- Filtering by search term, genre, and platform
- Ordering by id, title, release year, critic score, and last updated
- Server-driven pagination
- Global API error handling and request validation

## Prerequisites

- .NET SDK 8+
- Node.js and npm
- SQL Server LocalDB available on Windows for the default local configuration

## Run The App

From the repository root:

```powershell
.\run-app.ps1
```

The script:

- installs frontend dependencies when needed
- builds the backend and frontend
- starts both apps
- waits for both services to be reachable
- prints the URLs, PIDs, and log file locations

Default local URLs:

- Frontend: `http://127.0.0.1:4200`
- Backend Swagger: `http://127.0.0.1:5201/swagger`

Useful options:

```powershell
.\run-app.ps1 -SkipBuild
.\run-app.ps1 -SkipInstall
```

To stop the app, use the process IDs printed by the script:

```powershell
Stop-Process -Id <backendPid>, <frontendPid>
```

## Run The Projects Separately

Backend:

```powershell
dotnet run --project .\Server\VideoGameCatalogue.Api\VideoGameCatalogue.Api.csproj --launch-profile http
```

Frontend:

```powershell
Set-Location .\Frontend
npm start
```

## Validation

Backend tests:

```powershell
dotnet test .\Tests\VideoGameCatalogue.Tests\VideoGameCatalogue.Tests.csproj
```

Frontend tests:

```powershell
Set-Location .\Frontend
npm test -- --watch=false
```

Frontend production build:

```powershell
Set-Location .\Frontend
npm run build
```

## API Notes

The main browse endpoint supports filtering, ordering, and pagination through query string parameters:

- `searchTerm`
- `genre`
- `platform`
- `orderBy` - `id`, `title`, `releaseYear`, `criticScore`, `updatedUtc`
- `orderDirection` - `asc`, `desc`
- `pageNumber`
- `pageSize`

Example:

```text
/api/video-games?orderBy=criticScore&orderDirection=desc&pageNumber=1&pageSize=12
```

## Notes

- The backend connection string and CORS settings are defined in `Server/VideoGameCatalogue.Api/appsettings.json`.
- The frontend dev server proxies `/api` requests to the backend on port `5201`.
- A sample data migration is included so a new local database starts with catalogue entries.