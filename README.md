# RaceDay — Part 2 RESTful API

This ASP.NET Core Web API is a starter implementation for the PROG6212 Programming 2B Part 2 requirements. It provides an API for South African running, walking, and cycling events.

## Features
- ASP.NET Core Web API in C# and Entity Framework Core (SQL Server, Code First).
- Registration and login with hashed passwords.
- Server-side session authentication and Organiser/Participant role checks.
- Event CRUD, event categories, participant enrolments, and race results.
- Swagger UI in Development.
- Basic API smoke tests and GitHub Actions build/test workflow.

## Data model used
`User`, `RaceEvent`, `EventCategory`, `Enrolment`, and `RaceResult`. Compare these entities and endpoint routes with your approved Part 1 ERD, SQL script, and endpoint plan. Change the implementation to match your approved plan before submitting.

## Run locally
1. Install Visual Studio 2022 with the ASP.NET and web development workload and the .NET 8 SDK.
2. Install/launch SQL Server or SQL Server LocalDB.
3. Open `RaceDay.sln` in Visual Studio.
4. Check `RaceDay.Api/appsettings.json` and update the `RaceDay` connection string for your SQL Server.
5. In Package Manager Console, set the default project to `RaceDay.Api`, then run:
   ```powershell
   Add-Migration InitialCreate
   Update-Database
   ```
   Alternatively use the CLI:
   ```bash
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate --project RaceDay.Api
   dotnet ef database update --project RaceDay.Api
   ```
6. Run the API. Open `/swagger` on the local URL shown in the browser.

## Session usage in Swagger
Use `POST /api/auth/register` to create a user, then `POST /api/auth/login`. The API uses an HTTP session cookie. In Swagger, cookie handling can vary by browser; if session cookies are not retained, test with a browser or an API client that preserves cookies.

Example register JSON:
```json
{
  "fullName": "Alex Runner",
  "email": "alex@example.com",
  "password": "StrongPass123!",
  "role": "Participant",
  "phone": "0712345678"
}
```
Valid roles are `Organiser` and `Participant`. Do not use real personal information in test data.

## Main endpoints
- `POST /api/auth/register`, `POST /api/auth/login`, `POST /api/auth/logout`
- `GET /api/users/me`, `PUT /api/users/me`
- `GET /api/events`, `GET /api/events/{id}`, organiser-only event create/update/delete
- `GET/POST /api/events/{eventId}/categories`, organiser-only category create/delete
- `POST /api/enrolments`, `GET /api/enrolments/mine`, organiser-only `GET /api/enrolments/event/{eventId}`
- `POST /api/results`, `GET /api/results/mine`

## Tests and CI
Run:
```bash
dotnet test RaceDay.sln
```
GitHub Actions builds and tests the solution on each push and pull request. Check the Actions tab and add a real screenshot of a successful green build here before submission.

## Before submission
- Compare the entities/routes with your approved Part 1 documents and make corrections.
- Run migrations and test the API against SQL Server.
- Add tests for successful login, registration, role restrictions, event management, and enrolment workflows; ensure all tests pass.
- Make at least 20 meaningful commits reflecting your own work.
- Record your own video voice-over demonstrating the running API, Swagger, authentication, endpoints, and tests. Add the unlisted YouTube link here.
- Submit your GitHub repository link on ARC. The POE instructions say ZIP files are not accepted as the final submission.
- Follow your institution's academic integrity rules. Understand and adapt the code yourself; disclose AI assistance if required.
