# Online Shopping Backend System – C# Console Application

A robust backend simulation of an e-commerce platform built with C# and .NET 10. The system demonstrates modern software engineering practices, including Domain-Driven Design (DDD), Clean Architecture, Entity Framework Core, and SOLID principles.

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)

### Database Configuration
1. Update the connection string in `src/ConsoleApp/appsettings.json` if necessary.
2. The application automatically applies migrations and seeds initial data on startup.

### Build and Run
```bash
# Build the solution
dotnet build

# Run the console application
dotnet run --project src/ConsoleApp/ConsoleApp.csproj
```

### Run Tests
```bash
# Run all tests
dotnet test

# Run specific test projects
dotnet test tests/Application.Tests/Application.Tests.csproj
dotnet test tests/Infrastructure.Tests/Infrastructure.Tests.csproj
```

---

## Running with Docker

Docker Compose spins up a SQL Server container and the console application together, with no local SQL Server installation required.

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (or Docker Engine + Compose plugin on Linux)

### Quick Start

```bash
# 1. Build the app image (only needs to run when source code changes)
docker compose build app

# 2. Start SQL Server in the background and wait for it to become healthy
docker compose up db -d

# 3. Launch the interactive console application
#    (--rm removes the container automatically when you exit)
docker compose run --rm app
```

> **Why two steps?**  
> `docker compose up` multiplexes log output from all services and never
> forwards stdin to any container, so the console menus would not be
> interactive. Running the app with `docker compose run` attaches your
> terminal's stdin/stdout directly to the container, making the menus
> fully usable.

### Stopping the Environment

```bash
# Stop SQL Server (data is preserved in the mssql_data Docker volume)
docker compose down

# Stop and delete all persisted data
docker compose down -v
```

### Environment Variables

| Variable | Default | Description |
|---|---|---|
| `DOTNET_ENVIRONMENT` | `Production` | Controls which `appsettings.*.json` overlay is loaded |
| `ConnectionStrings__DefaultConnection` | *(set in docker-compose.yml)* | Full SQL Server connection string. Overrides `appsettings.json`. |
| `SA_PASSWORD` | `YourStrong@Passw0rd` | SQL Server SA password (change before production use) |

> **Security note:** The SA password in `docker-compose.yml` is for local development only. For any shared or production deployment, inject secrets via Docker secrets or a secrets manager rather than plain environment variables.

### How It Works

1. The `db` service starts SQL Server and waits until it is healthy.
2. The `app` service starts only after the database is healthy (`depends_on: condition: service_healthy`).
3. On first run, EF Core migrations are applied automatically (`context.Database.Migrate()` in `DatabaseSeeder`), and sample data is seeded.
4. Subsequent runs reuse the data stored in the `mssql_data` Docker volume.

---

## Features

### Customer Features
- **Account Management:** Secure registration and login.
- **Product Discovery:** Browse and search the product catalog.
- **Shopping Cart:** Add, update, and remove items with real-time calculations.
- **Checkout:** Process orders with automated inventory updates.
- **Wallet System:** Manage balance and add funds for simulated payments.
- **Order Tracking:** View order history and current status.
- **Social:** Submit and view product reviews and ratings.

### Administrator Features
- **Catalog Management:** Create, update, and delete products.
- **Inventory Control:** Restock products and monitor low-stock items.
- **Order Fulfillment:** View all customer orders and update tracking statuses.
- **Analytics:** Generate daily sales and product performance reports.

---

## Project Architecture

The system follows **Clean Architecture** principles, ensuring a clear separation of concerns:

- **Domain:** Core entities (`User`, `Product`, `Order`, etc.), domain exceptions, and repository interfaces. No external dependencies.
- **Application:** Business logic services (`AuthService`, `OrderService`, etc.) and DTOs. Coordinates domain objects to fulfill use cases.
- **Infrastructure:** Data persistence using **Entity Framework Core**. Includes SQL Server implementations of repositories and the `ShoppingDbContext`.
- **ConsoleApp:** The presentation layer providing a menu-driven CLI for user interaction.

---

## Technical Stack
- **Framework:** .NET 10
- **Persistence:** Entity Framework Core (SQL Server)
- **Patterns:** Repository, Unit of Work, Dependency Injection, Data Transfer Objects (DTOs)
- **Querying:** LINQ for efficient data processing
- **Testing:** xUnit with Moq and FluentAssertions

---
*Developed for the Online Shopping Backend System Project.*
