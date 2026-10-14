# Clinic Management System - .NET 8 Migration

## Overview
This is a complete migration of the Clinic Management System from ASP.NET Web Forms 4.5.2 to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
ClinicManagement/
├── src/
│   ├── ClinicManagement.Domain/          # Domain entities, interfaces, enums
│   ├── ClinicManagement.Application/     # Business logic, services, DTOs
│   ├── ClinicManagement.Infrastructure/  # EF Core, repositories, data access
│   └── ClinicManagement.Web/             # Razor Pages, UI layer
└── tests/
    └── ClinicManagement.UnitTests/       # Unit tests
```

## Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Visual Studio 2022 or VS Code

## Setup Instructions

### 1. Database Setup
Run the SQL scripts in the `Database Files` folder to create the database:
```sql
-- Run in order:
-- 1. Schema.sql
-- 2. Admin.sql
-- 3. Doctor.sql
-- 4. Patient.sql
-- 5. SignUp.sql
-- 6. Insertions.sql
```

### 2. Configuration
Update the connection string in `src/ClinicManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=.\\SQLEXPRESS;Initial Catalog=DBProject;Integrated Security=True;TrustServerCertificate=True"
  }
}
```

### 3. Build and Run
```bash
cd src/ClinicManagement.Web
dotnet restore
dotnet build
dotnet run
```

## User Roles
- **Admin** (Type=3): Manage doctors, staff, view statistics
- **Doctor** (Type=2): Manage appointments, update prescriptions, generate bills
- **Patient** (Type=1): Book appointments, view history, provide feedback

## Key Features
- Patient registration and login
- Doctor registration by admin
- Appointment booking and management
- Treatment history tracking
- Bill generation and management
- Patient feedback system

## Migration Notes
- Replaced ADO.NET with Entity Framework Core 8.0
- Replaced Web Forms pages with Razor Pages
- Replaced Web.config with appsettings.json
- Replaced System.Web with ASP.NET Core equivalents
- Added proper dependency injection
- Added async/await patterns throughout
- Added structured logging with Serilog
