
# SpaRes Modernization

A full-stack spa reservation management application built with ASP.NET Core Web API, React, and TypeScript.

This project modernizes a legacy spa reservation system using contemporary web technologies, REST APIs, and a component-based frontend architecture.

## Technology Stack

### Backend
- C# / ASP.NET Core Web API
- Entity Framework Core
- SQLite
- RESTful APIs
- Swagger / OpenAPI

### Frontend
- React
- TypeScript
- Vite
- HTML / CSS
- Fetch API

### Development Tools
- Visual Studio
- Visual Studio Code
- Git and GitHub

## Architecture

The application separates frontend presentation from backend business logic and data access.

```text
React + TypeScript
        |
        | HTTP / JSON
        v
ASP.NET Core Web API
        |
        v
Entity Framework Core
        |
        v
SQLite Database
```

## Version 1 — Implemented Features

- Staff authentication
- Client data retrieval
- Service and location management APIs
- Staff schedules, breaks, and absences
- Reservation creation through the React interface
- Reservation management API endpoints
- API testing through Swagger

## Project Structure

```text
spares-modernization/
|
|-- SpaRes.Api/       # ASP.NET Core backend
|-- spares-web/       # React + TypeScript frontend
|-- README.md         # Project documentation
|-- .gitignore        # Git exclusions
```

## Running Locally

### Backend

1. Open the backend project in Visual Studio.
2. Restore NuGet dependencies.
3. Configure the local SQLite database.
4. Start the ASP.NET Core Web API.
5. Use Swagger to inspect and test API endpoints.

### Frontend

Open a terminal in `spares-web`:

```bash
npm install
npm run dev
```

Open the local URL displayed by Vite (typically `http://localhost:5173`).

The backend must be running for API-dependent functionality.

## Development Status

**Version 1:** Core reservation workflow implemented and tested locally.

**Planned Improvements:**
- Additional reservation management workflows
- Enhanced scheduling and availability validation
- UI improvements and reusable components
- Automated testing
- Deployment preparation

## Security

This repository is intended for development and portfolio demonstration.

Credentials, environment secrets, and local database files should not be committed to source control.

## Author

Tony Bakhti

Senior Software Developer | Full-Stack Engineer
