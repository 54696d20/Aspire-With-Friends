# Aspire With Friends

**Aspire With Friends** is a sample distributed application built with the .NET Aspire stack. It showcases event‑driven communication and real‑time updates for a Blazor WebAssembly client.

---

## 🛠 Tech Stack

### Core Technologies
- **.NET Aspire** on **.NET 9**
- **Blazor WebAssembly** UI with **MudBlazor**
- **ASP.NET Web API** services (MasterDataService & WeatherAPI)
- **WolverineFx** messaging
- **RabbitMQ** for asynchronous events
- **SignalR** for client notifications
- **Redis** cache
- **SQL Server**
- **YARP** reverse proxy
- **Docker** container orchestration
- **Aspire Dashboard**

### Planned Integrations
- Authentication via **Keycloak**
- **Handlebars** templating
- **Prometheus** + **Grafana** monitoring
- **Serilog** logging
- **Elsa Workflows**

---

## 👤 Project Structure

```text
Aspire-With-Friends/
├── AspireApp.AppHost/               # Entry point that orchestrates services
├── AspireApp.MasterDataService/     # API with SQL Server + Wolverine
├── AspireApp.NotificationHubService/# Publishes SignalR notifications
├── AspireApp.WeatherAPI/            # Example weather service
├── AspireApp.ServiceDefaults/       # Shared infrastructure helpers
├── AspireApp.WebWasm/               # Blazor WebAssembly frontend
├── YarpGateway/                     # Reverse proxy using YARP
└── AspireApp.Shared/                # Shared contracts and utilities
```

---

## 📮 Message Flow

```text
MasterDataService --(RabbitMQ)--> NotificationHubService --(SignalR)--> WebWasm
```

---

## ✅ Features

- Modular services with asynchronous messaging
- Real-time client updates through SignalR
- Redis caching and SQL Server storage
- YARP gateway routing
- Docker-based orchestration via .NET Aspire AppHost

---

## 💻 Getting Started

1. Clone the repository
2. Ensure the **.NET 9 SDK** and **Docker Desktop** are installed
3. Run `dotnet run --project AspireApp.AppHost` to start all services
4. Use Postman collections under `MasterDataService/Data/Postman` to test APIs
5. View live updates in the Blazor WebAssembly app
6. Monitor services using the Aspire Dashboard

---

## 📝 Notes

- RabbitMQ uses default `guest/guest` credentials for local development
- SignalR endpoints must be reachable through the YARP gateway
- The Aspire Dashboard starts automatically when running the AppHost

---

## 🤔 Author

This project explores microservice messaging and real-time notifications using the Aspire ecosystem.

---
