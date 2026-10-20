# Student Portal API

A .NET 10 ASP.NET Core Web API for managing students, their addresses, genders, and profile images. The API uses SQL Server through Entity Framework Core and is intended to be consumed by a local Angular application.

## Contents

- [Technology](#technology)
- [Repository layout](#repository-layout)
- [Prerequisites](#prerequisites)
- [Configuration](#configuration)
- [Run the API](#run-the-api)
- [API endpoints](#api-endpoints)
- [Database and migrations](#database-and-migrations)
- [Profile image storage](#profile-image-storage)
- [Tests](#tests)
- [Current implementation notes](#current-implementation-notes)

## Technology

| Component | Details |
| --- | --- |
| Runtime target | .NET 10 (`net10.0`) |
| Web framework | ASP.NET Core Web API, controller-based |
| Data access | Entity Framework Core 10 with SQL Server provider |
| Validation | FluentValidation ASP.NET Core integration |
| Object mapping | AutoMapper |
| API documentation | Swagger / Swashbuckle, enabled in Development |
| Tests | xUnit, Moq, and Microsoft.NET.Test.Sdk; targets .NET 10 |

## Repository layout

```text
StudentAdminPortal.API/
  StudentAdminPortal.API.sln
  StudentAdminPortal.API/
    Controllers/        HTTP endpoints
    DataModels/         EF Core entities and DbContext
    DomainModels/       API request and response models
    Migrations/         EF Core database migrations
    Profiles/           AutoMapper mappings
    Repository/         SQL Server and image-storage implementations
    Validator/          FluentValidation request validators
    Program.cs          Dependency registration and HTTP pipeline
  StudentAdminPortal.API.Tests/
    Controllers/        Controller tests
    Validator/          Request validator tests
```

## Prerequisites

- .NET 10 SDK (the project and tests target `net10.0`).
- SQL Server or SQL Server Express, with a database the configured connection string can access.
- For database migration commands, the `dotnet-ef` CLI tool matching EF Core 10. The project references EF Core tooling, but that package does not itself install the global CLI.

Check your SDK installation with:

```powershell
dotnet --list-sdks
```

## Configuration

The API reads the connection string named `StudentAdminPortalDb` from ASP.NET Core configuration. The checked-in development settings currently contain a machine-specific SQL Server instance name; replace it for your machine or override it through the environment. Avoid committing passwords or other secrets.

For a PowerShell session, set the environment variable before running the API:

```powershell
$env:ConnectionStrings__StudentAdminPortalDb = "Server=(localdb)\MSSQLLocalDB;Database=StudentAdminPortalDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

The double underscore (`__`) maps to the configuration key `ConnectionStrings:StudentAdminPortalDb`. Use the connection syntax and authentication method appropriate for your SQL Server instance. For production, provide the connection string through a secure deployment secret/configuration provider rather than source control.

The configured CORS policy allows requests from `http://localhost:4200`, with any header and method. Change the allowed origin in `Program.cs` when the frontend runs at a different address.

## Run the API

From the repository root:

```powershell
dotnet restore .\StudentAdminPortal.API\StudentAdminPortal.API.sln
dotnet run --project .\StudentAdminPortal.API\StudentAdminPortal.API\StudentAdminPortal.API.csproj
```

Apply the database migrations before using endpoints that access SQL Server. In Development, Swagger UI is available at `/swagger` on the local API address printed by `dotnet run`.

## API endpoints

Student and gender routes use the `api` prefix. The sample weather endpoint uses `/WeatherForecast`. Controller routes are case-insensitive in ASP.NET Core, but the paths below use the controller/action spelling from the source.

### Students (`api/Students`)

| Method | Route | Behavior |
| --- | --- | --- |
| `GET` | `/api/Students/GetAllStudent` | Returns all students, including mapped gender and address data. |
| `GET` | `/api/Students/GetStudentById/{studentId}` | Returns a student by GUID, or `404` when no student is found. |
| `POST` | `/api/Students/AddStudent` | Creates a student and returns `201 Created` with the new student and a `GetStudentById` location. |
| `PUT` | `/api/Students/UpdateStudent/{studentId}` | Updates an existing student; returns the mapped student or `404`. |
| `DELETE` | `/api/Students/DeleteStudent/{studentId}` | Deletes an existing student; returns `200` on success or `404` otherwise. |
| `POST` | `/api/Students/UploadProfile/{studentId}` | Uploads a profile image as multipart form data using the `profileImage` field. |

Student IDs in routes must be GUIDs. Add/update JSON uses the request model fields `studentFirstName`, `studentLastname`, `dob`, `email`, `mobile`, `genderId`, `streetAdress`, and `zipCode` (JSON property casing is handled by ASP.NET Core's JSON defaults). The validators require these values to be populated, require a mobile number greater than 999,999,999 and less than 10,000,000,000, and check that the gender exists.

Example request to create a student:

```json
{
  "studentFirstName": "Ava",
  "studentLastname": "Patel",
  "dob": "2000-01-01",
  "email": "ava@example.com",
  "mobile": 1234567890,
  "genderId": "00000000-0000-0000-0000-000000000000",
  "streetAdress": "1 Main Street",
  "zipCode": "12345"
}
```

Use a real gender ID returned by the gender endpoint; the ID above is only a placeholder.

### Genders (`api/Gender`)

| Method | Route | Behavior |
| --- | --- | --- |
| `GET` | `/api/Gender/GetGenders` | Returns the available genders. |

### Sample weather endpoint

| Method | Route | Behavior |
| --- | --- | --- |
| `GET` | `/WeatherForecast` | Returns five sample forecasts with generated temperatures and summaries. This is template/demo data, not student-portal functionality. |

## Database and migrations

The API registers `StudentAdminContext` with the SQL Server provider. EF Core entities are stored in the `Students`, `Genders`, and `Address` tables. The repository includes migrations under `StudentAdminPortal.API/Migrations`.

If `dotnet-ef` is not installed, install a version compatible with the project's EF Core 10 packages:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

Apply migrations from the repository root:

```powershell
dotnet ef database update `
  --project .\StudentAdminPortal.API\StudentAdminPortal.API\StudentAdminPortal.API.csproj `
  --startup-project .\StudentAdminPortal.API\StudentAdminPortal.API\StudentAdminPortal.API.csproj
```

The database must be reachable using the configured `StudentAdminPortalDb` connection string. Review generated or pending migrations before applying them to a production database.

## Profile image storage

The upload endpoint accepts `.jpeg`, `.jpg`, `.png`, or `.gif` file extensions. Files are written under `Resources/Images` relative to the process working directory. The static-file middleware serves content from the API content root's `Resources` directory under the `/Resources` URL prefix. For local development, ensure these directories exist under the API project directory:

```powershell
New-Item -ItemType Directory -Force .\StudentAdminPortal.API\StudentAdminPortal.API\Resources\Images
```

An uploaded file is returned as a relative path, such as `Resources/Images/<filename>.jpg`, and is served at `/Resources/Images/<filename>.jpg`. Files served this way are publicly accessible; do not put private content in this directory. The current storage implementation is local disk storage, not cloud or database storage.

## Tests

Run the test project:

```powershell
dotnet test .\StudentAdminPortal.API\StudentAdminPortal.API.Tests\StudentAdminPortal.API.Tests.csproj
```

Or run all tests in the solution:

```powershell
dotnet test .\StudentAdminPortal.API\StudentAdminPortal.API.sln
```

The test project contains controller tests using mocked repository and mapper dependencies, plus request-validator tests. These tests do not require a live SQL Server. They do not constitute full end-to-end tests of HTTP routing, serialization, authentication, or database integration.

## Current implementation notes

- Swagger is registered for the API and exposed only when the ASP.NET Core environment is `Development`.
- CORS is currently restricted to `http://localhost:4200`.
- The application calls authorization middleware but does not currently configure an authentication scheme or authorization policies; the API should not be treated as protected by authentication.
- Profile-image static files are served without per-file authorization.
- `LocalStorageRepository` writes relative to the process working directory, while static-file serving resolves `Resources` relative to the application content root. Run the app with the API project as its working directory, or align these paths if hosting with a different working directory.