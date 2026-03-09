# Online Shopping Backend System – C# Console Application

A robust backend simulation of an e-commerce platform built with C# and .NET 10. The system demonstrates modern software engineering practices, including Domain-Driven Design (DDD), Clean Architecture, Entity Framework Core, and SOLID principles.

---

## 🚀 Getting Started

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

## 📅 Submissions

| Submission | Focus | Due Date |
|---|---|---|
| **Submission 1** | Core backend functionality & OOP | 9 March 2026 – 12:00 PM |
| **Submission 2** | Design Patterns & Clean Architecture | 9 March 2026 – 5:00 PM |

---

## ✨ Features

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

## 🏗️ Project Architecture

The system follows **Clean Architecture** principles, ensuring a clear separation of concerns:

- **Domain:** Core entities (`User`, `Product`, `Order`, etc.), domain exceptions, and repository interfaces. No external dependencies.
- **Application:** Business logic services (`AuthService`, `OrderService`, etc.) and DTOs. Coordinates domain objects to fulfill use cases.
- **Infrastructure:** Data persistence using **Entity Framework Core**. Includes SQL Server implementations of repositories and the `ShoppingDbContext`.
- **ConsoleApp:** The presentation layer providing a menu-driven CLI for user interaction.

---

## 🛠️ Technical Stack
- **Framework:** .NET 10
- **Persistence:** Entity Framework Core (SQL Server)
- **Patterns:** Repository, Unit of Work, Dependency Injection, Data Transfer Objects (DTOs)
- **Querying:** LINQ for efficient data processing
- **Testing:** xUnit with Moq and FluentAssertions

---

## 📜 Documentation & Guidelines
- [RULES.md](RULES.md) – Coding standards and naming conventions.
- [TESTING.md](TESTING.md) – Testing strategy and guidelines.
- [CLAUDE.md](CLAUDE.md) – Build, test, and development workflow.

---
*Developed for the Online Shopping Backend System Project.*
