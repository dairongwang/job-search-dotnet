# Job Search Dashboard (.NET + Angular)

Converted from the original Python/Streamlit job search dashboard.

## Structure

- `backend/` — ASP.NET Core 8 Web API
  - Entity Framework Core + SQLite (`job_search.db`)
  - Controllers: Jobs, Search, Config, Dashboard, Contacts, Outreach, Organizations
  - Job search service with adapters for Remotive, Jobicy, Arbeitnow, Remote OK
  - Configuration stored in `backend/config/job_search_config.json`
- `frontend/` — Angular 18 app
  - Dashboard, Search, Jobs tracker, Contacts, Outreach, Configuration

## Prerequisites

- .NET 8 SDK
- Node.js 18+ and npm

## Run the Backend

```bash
cd backend
dotnet run
```

API listens on `http://localhost:5000` (or the port shown in the console).  
Swagger UI: `http://localhost:5000/swagger`

## Run the Frontend

```bash
cd frontend
npm install   # if not already done
npm start
```

Opens at `http://localhost:4200`. The dev server proxies `/api` to the backend.

## Features

- Search listings from enabled job sites (Remotive, Jobicy, Arbeitnow, Remote OK)
- Rank results by configured titles and skills
- Save matches into the job tracker
- Track pipeline status (Saved → Applied → Interview → Offer → …)
- Contacts and outreach log
- Dashboard with counts and overdue follow-ups
- Editable configuration (sites, preferred titles, skills)

## Configuration

Edit sites and preferences in the Config page, or directly edit  
`backend/config/job_search_config.json`.

## Notes

- This is a functional port of the core Python application.
- Additional providers (Adzuna, Reed, The Muse, Himalayas, USAJOBS, etc.) can be added by extending `JobSearchService.FetchSiteAsync`.
- Database is created automatically on first run via `EnsureCreated()`.
