# Cocus Flight Management System

A comprehensive flight management system built with ASP.NET Core MVC, featuring flight scheduling, aircraft management, and airport tracking capabilities with Google OAuth authentication.

## Features

- **Flight Management**: Schedule, track, and manage flights with real-time status updates
- **Aircraft Management**: Maintain aircraft inventory with fuel consumption and performance metrics
- **Airport Management**: Manage airport information including location coordinates
- **Google Authentication**: Secure login using Google OAuth 2.0
- **Database Seeding**: Automated test data generation using Bogus library
- **Entity Framework Core**: Code-first database approach with SQL Server

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or higher)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Google Cloud account for OAuth configuration

## Installation

### 1. Clone the repository

```bash
git clone https://github.com/eliaquimmauricio/cocus-challenge.git
cd cocus-challenge
```

### 2. Configure Database Connection

Update the connection string in `1 - Application/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=cocus;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 3. Configure Google Authentication

1. Create a project in [Google Cloud Console](https://console.cloud.google.com/)
2. Enable Google+ API
3. Create OAuth 2.0 credentials
4. Add authorized redirect URI: `https://localhost:5001/signin-google`
5. Configure user secrets:

```bash
cd "1 - Application"
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_CLIENT_SECRET"
```

### 4. Apply Database Migrations

```bash
cd "1 - Application"
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet run
```

## Author

**Eliaquim Mauricio**
- GitHub: [@eliaquimmauricio](https://github.com/eliaquimmauricio)
