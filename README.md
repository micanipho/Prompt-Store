# Online Shopping Backend System – C# Console Application

A backend simulation of a real-world e-commerce platform built as a C# Console Application. The system demonstrates object-oriented programming, Domain-Driven Design, LINQ querying, exception handling, and clean architecture.

---

## Submissions

| Submission | Focus | Due |
|---|---|---|
| Submission 1 | Core backend functionality | 9 March 2026 – 12:00 PM |
| Submission 2 | Software Design Patterns + architecture improvements | 9 March 2026 – 5:00 PM |

---

## Features

**Customer**
- Register and log in
- Browse and search the product catalog
- Manage shopping cart (add, update, remove)
- Checkout and pay via simulated wallet
- View wallet balance and add funds
- View order history and track orders
- Leave product reviews and ratings

**Administrator**
- Add, update, delete, and restock products
- View and manage all orders
- Update order statuses
- Monitor low stock products
- Generate sales reports

---

## Project Structure

```
src/
├── OnlineShopping.Domain/          # Core domain — entities, interfaces, enums
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Customer.cs
│   │   ├── Administrator.cs
│   │   ├── Product.cs
│   │   ├── Cart.cs
│   │   ├── CartItem.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   ├── Payment.cs
│   │   └── Review.cs
│   ├── Enums/
│   │   ├── OrderStatus.cs
│   │   └── UserRole.cs
│   └── Interfaces/
│       ├── IUserRepository.cs
│       ├── IProductRepository.cs
│       └── IOrderRepository.cs
│
├── OnlineShopping.Application/     # Use cases and business logic
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── ProductService.cs
│   │   ├── CartService.cs
│   │   ├── OrderService.cs
│   │   ├── PaymentService.cs
│   │   ├── InventoryService.cs
│   │   ├── ReviewService.cs
│   │   └── ReportService.cs
│   └── Dtos/
│       ├── RegisterUserRequest.cs
│       ├── LoginRequest.cs
│       ├── CreateProductRequest.cs
│       ├── UpdateProductRequest.cs
│       ├── PlaceOrderRequest.cs
│       └── AddFundsRequest.cs
│
├── OnlineShopping.Infrastructure/  # In-memory data storage
│   └── Repositories/
│       ├── InMemoryUserRepository.cs
│       ├── InMemoryProductRepository.cs
│       └── InMemoryOrderRepository.cs
│
└── OnlineShopping.ConsoleApp/      # Entry point and console menus
    ├── Program.cs
    └── Menus/
        ├── MainMenu.cs
        ├── CustomerMenu.cs
        └── AdminMenu.cs
```

---

## Architecture

This project follows **Domain-Driven Design (DDD)** with a **Clean Architecture** layer separation:

| Layer | Project | Responsibility |
|---|---|---|
| Domain | `OnlineShopping.Domain` | Entities, interfaces, enums — no external dependencies |
| Application | `OnlineShopping.Application` | Business logic, services, DTOs — depends only on Domain |
| Infrastructure | `OnlineShopping.Infrastructure` | In-memory repositories implementing Domain interfaces |
| Presentation | `OnlineShopping.ConsoleApp` | Console menus and user interaction — entry point |

**Dependency rule:** outer layers depend on inner layers. The Domain layer has zero dependencies.

---

## Technical Stack

- **Language:** C# (.NET)
- **Data storage:** In-memory collections (`List<T>`)
- **Querying:** LINQ
- **Design:** OOP — inheritance, polymorphism, interfaces
- **Patterns (Submission 2):** Repository, Factory, Singleton, Observer, Strategy (TBD)

---

## Coding Standards

See [RULES.md](RULES.md) for the full C# coding standards followed in this project.
