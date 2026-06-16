# Transit-X

A full-stack ASP.NET Core web application for transit and transportation management.

## Overview

Transit-X is a comprehensive transit management system built with ASP.NET Core, designed to handle transportation operations efficiently.

## Features

- **Transit Service Management**: Core transit operations and scheduling
- **Database Integration**: Entity Framework Core with SQL Server support
- **Web Interface**: Razor Views for user interaction
- **Responsive Design**: Modern CSS and JavaScript for frontend

## Technology Stack

### Backend
- **Framework**: ASP.NET Core (.NET 10.0)
- **Database**: SQL Server with Entity Framework Core
- **Architecture**: MVC Pattern

### Frontend
- **Views**: Razor Pages
- **Styling**: CSS
- **Scripting**: JavaScript

## Project Structure

```
TransitX-fullstack/
├── Controllers/          # Application controllers
├── Models/              # Data models
├── Views/               # Razor view templates
├── Data/                # Database context and migrations
├── Services/            # Business logic services
├── wwwroot/             # Static assets (CSS, JS, Images, Videos)
├── Properties/          # Project properties
├── Program.cs           # Application entry point
├── TransitX.csproj      # Project configuration
└── appsettings.json     # Configuration settings
```

## Prerequisites

- .NET 10.0 SDK or later
- SQL Server (or compatible)
- Visual Studio 2022 or VS Code

## Getting Started

### 1. Clone the Repository
```bash
git clone https://github.com/Zohaib2003pk/Transit-X.git
cd Transit-X
```

### 2. Restore Dependencies
```bash
dotnet restore
```

### 3. Configure Database
Update `appsettings.json` with your database connection string:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  }
}
```

### 4. Apply Migrations
```bash
dotnet ef database update
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `http://localhost:5000`

## Key Components

### Controllers
- **HomeController**: Handles main application routes and navigation

### Models
- Core data models for transit operations

### Services
- **ITransitService**: Interface for transit operations
- **TransitService**: Implementation of transit service logic

### Database
- **TransitXDbContext**: Entity Framework Core database context
- Migrations for schema management

## Configuration

Application settings are configured in `appsettings.json`:
- Database connection strings
- Logging levels
- Application-specific settings

## Development

### Build the Project
```bash
dotnet build
```

### Run Tests (if available)
```bash
dotnet test
```

### Clean Build
```bash
dotnet clean
```

## Deployment

The application is configured to run on .NET 10.0 and includes runtime configurations for multiple platforms:
- Windows (x64, x86, ARM)
- Linux (x64, ARM, ARM64)
- macOS (x64, ARM64)

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Author

**Zohaib**

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, please open an issue on the GitHub repository.

## Changelog

### Version 1.0.0 (Initial Release)
- Initial project setup
- Database schema with migrations
- Basic transit service implementation
- Web UI with Razor views
