# 🧱 Microservices Core Package

This repository contains the **core packages** for building robust, scalable, and maintainable microservices using .NET 9. It is structured in a modular way to promote separation of concerns, reusability, and clean architectural practices.

> ✅ Each folder represents a **NuGet-ready package** with its own responsibility and internal `README.md` for detailed usage.

---

## 📦 Package Overview

| Package                              | Description                                                             |
|--------------------------------------|-------------------------------------------------------------------------|
| **Core.Abstractions**                | Base interfaces and contracts used across all layers                    |
| **Core.Application**                 | Pipelines (e.g. validation, caching, logging, IP control) using MediatR |
| **Core.CrossCuttingConcerns**        | Common concerns like exceptions, guards, enums                          |
| **Core.ElasticSearch**               | Integration and abstraction for Elasticsearch                          |
| **Core.Mailing**                     | Email infrastructure with SMTP support                                 |
| **Core.Messaging.InMemory**          | In-memory messaging infrastructure (useful for testing)                |
| **Core.Messaging.Postgres**          | Messaging implementation using PostgreSQL (Outbox Pattern)             |
| **Core.Messaging.SqlServer**         | Messaging with SQL Server support (Outbox Pattern)                     |
| **Core.Messaging.Transport.InMemory**| In-memory message transport abstraction                                |
| **Core.Messaging.Transport.Kafka**   | Kafka transport for event-driven systems                               |
| **Core.Messaging.Transport.RabbitMQ**| RabbitMQ transport infrastructure                                      |
| **Core.Monitoring**                  | Health checks, metrics, logging extensions                             |
| **Core.Persistence**                 | Persistence abstractions with UnitOfWork, Transaction support          |
| **Core.Resiliency**                  | Retry, Circuit Breaker, Timeout policies (Polly)                       |
| **Core.Scheduling.Hangfire**         | Delayed and recurring job scheduling using Hangfire                    |
| **Core.Scheduling.Postgres.Internal**| Scheduled job store using PostgreSQL                                   |
| **Core.Scheduling.SqlServer.Internal**| Scheduled job store using SQL Server                                   |
| **Core.Security**                    | JWT, claims, and permission management                                 |
| **Core.ServiceDiscovery**            | Service discovery abstractions (e.g. Eureka, Consul)                   |
| **Core.Tracing**                     | Distributed tracing (OpenTelemetry, etc.)                              |
| **Core.Test**                        | Common test utilities and base test setup                              |

---

## 🚀 Getting Started

Each package is designed to be independent and reusable across microservices.

You can install them via `NuGet`:

```bash
dotnet add package Core.Abstractions
dotnet add package Core.Messaging.Transport.Kafka
```

> 📌 Make sure to check each package’s own `README.md` for detailed setup, configuration, and usage instructions.

---

## 🧩 Design Principles

- **Clean Architecture** - Ports & Adapters, Separation of Concerns  
- **CQRS & Event-Driven** - Commands, Queries, Integration Events  
- **Resilient by Design** - Retry, Circuit Breaker, Message Outbox  
- **Open for Extension** - Plugin-like modularity  
- **Cloud-Native Ready** - Service Discovery, Tracing, Health Checks

---

## 💡 Example Use Cases

- ✅ Schedule background jobs with Hangfire & PostgreSQL
- ✅ Publish & consume integration events via Kafka or RabbitMQ
- ✅ Enforce IP allow/deny rules with IPControlBehavior
- ✅ Track request flow using distributed tracing (OpenTelemetry)

---

## 💠 Testing

Some packages like `Core.Messaging.InMemory` and `Core.Test` are optimized for unit/integration testing.

---

## 🛠 Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/)
- Docker (for Kafka/RabbitMQ/Postgres if needed)
- MediatR, Polly, Newtonsoft.Json, etc.

---

## 👍 Contributing

Feel free to contribute!

1. Fork the repo
2. Create a new branch
3. Add or improve a package
4. Submit a Pull Request

---

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file.

---

> 📄 For internal use or enterprise support, you can contact the maintainers.

