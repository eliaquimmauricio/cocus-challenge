# Cocus Flight Management System

A comprehensive flight management system built with ASP.NET Core MVC, featuring flight scheduling, aircraft management, and airport tracking capabilities with Google OAuth authentication.

## ?? Features

- **Flight Management**: Schedule, track, and manage flights with real-time status updates
- **Aircraft Management**: Maintain aircraft inventory with fuel consumption and performance metrics
- **Airport Management**: Manage airport information including location coordinates
- **Google Authentication**: Secure login using Google OAuth 2.0
- **Database Seeding**: Automated test data generation using Bogus library
- **Entity Framework Core**: Code-first database approach with SQL Server

## ?? Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express or higher)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- Google Cloud account for OAuth configuration

## ??? Project Structure

```
cocus-challenge/
??? 0 - Tests/              # Unit and integration tests
?   ??? Cocus.Tests.csproj
??? 1 - Application/        # ASP.NET Core MVC application
?   ??? Controllers/        # MVC Controllers
?   ??? Views/             # Razor views
?   ??? Cocus.Mvc.csproj
??? 2 - Domain/            # Domain entities and interfaces
?   ??? Entities/          # Domain models
?   ??? Interfaces/        # Repository and service interfaces
?   ??? Services/          # Business logic
?   ??? Cocus.Domain.csproj
??? 3 - Infra/             # Infrastructure layer
    ??? Data/              # DbContext and repositories
    ??? Seeders/           # Database seeding
    ??? Cocus.Infra.Data.csproj
```

## ?? Installation

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

The application will be available at `https://localhost:5001`

## ??? Database Schema

### Entities

- **Airport**: Airport information (code, name, city, country, coordinates)
- **Aircraft**: Aircraft details (manufacturer, model, registration, fuel consumption, range)
- **Flight**: Flight information (flight number, departure/arrival, status, fuel requirements)

### Flight Status

- Scheduled
- Boarding
- Departed
- InFlight
- Landed
- Cancelled

## ?? Key Technologies

- **ASP.NET Core 10.0**: Web framework
- **Entity Framework Core 10.0**: ORM
- **SQL Server**: Database
- **Bogus**: Fake data generation
- **Google OAuth 2.0**: Authentication
- **Bootstrap**: UI framework

## ?? Database Seeding

The application includes an automatic seeding feature that generates sample data:

- 5 Airports with realistic location data
- 5 Aircraft with various manufacturers (Boeing, Airbus, Embraer, Bombardier, ATR)
- 5 Flights with calculated fuel requirements and flight times

To enable/disable seeding, modify `appsettings.json`:

```json
{
  "DbSeeder": {
    "Enabled": true
  }
}
```

## ?? Authentication

The application uses Google OAuth 2.0 for user authentication. Users must sign in with their Google account to access the flight management features.

## ?? Testing

Run tests using:

```bash
dotnet test
```

## ?? Configuration Options

### appsettings.json

- `ConnectionStrings:DefaultConnection`: Database connection string
- `DbSeeder:Enabled`: Enable/disable automatic database seeding
- `Logging:LogLevel`: Configure logging levels

### User Secrets (Development)

- `Authentication:Google:ClientId`: Google OAuth Client ID
- `Authentication:Google:ClientSecret`: Google OAuth Client Secret

## ?? Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## ?? License

This project is part of the Cocus Challenge.

## ?? Author

**Eliaquim Mauricio**
- GitHub: [@eliaquimmauricio](https://github.com/eliaquimmauricio)

## ?? Acknowledgments

- Built for the Cocus Challenge
- Uses Bogus library for realistic test data generation
- Implements clean architecture principles with separated layers
