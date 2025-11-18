# Expense Tracker Backend

A robust RESTful API built with ASP.NET Core 9.0. This backend provides comprehensive expense management capabilities with JWT-based authentication and PostgreSQL database integration.

## Features

- **User Authentication & Authorization**
  - JWT-based authentication with access and refresh tokens
  - Secure password hashing
  - Token refresh mechanism
  - Protected endpoints with role-based access

- **Expense Management**
  - Create, read, update, and delete expenses
  - Track expense details (amount, currency, date, payment method)
  - Support for recurring expenses with customizable frequencies
  - Receipt URL storage
  - Category-based expense organization
  - User-specific expense isolation

- **Budget Tracking**
  - Create and manage budgets
  - Track budget amounts and names
  - Update budget allocations

- **User Management**
  - User profile management
  - Track total income
  - Update user information

- **Category System**
  - Predefined expense categories
  - Category-based expense filtering

## Tech Stack

- **Framework**: ASP.NET Core 9.0
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 9.0
- **Authentication**: JWT Bearer Authentication
- **API Documentation**: Scalar API Reference
- **Security**: BCrypt password hashing

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/download/) (version 12 or higher)
- A code editor (VS Code)

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/natenguyen100/expense-tracker-backend-2.0
cd expense-tracker-backend-2.0
```

### 2. Database Setup

Create a PostgreSQL database:

```sql
CREATE DATABASE AppDb;
```

### 3. Configuration

Update the `appsettings.json` file with your database credentials and JWT settings:

```json
{
  "AppSettings": {
    "Token": "your-very-long-secret-key-at-least-64-characters-for-production",
    "Issuer": "YourAppName",
    "Audience": "https://your-domain.com"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AppDb;Username=your_username;Password=your_password"
  }
}
```

### 4. Apply Database Migrations

```bash
dotnet ef database update
```

If you need to create a new migration:

```bash
dotnet ef migrations add MigrationName
```

### 5. Run the Application

```bash
dotnet run
```

The API will start at `https://localhost:5208` (or the port specified in your configuration).
