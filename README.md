# Aspire-With-Friends

**Aspire-With-Friends** is a microservices-based, event-driven application built using .NET Aspire and a modern cloud-native stack. It demonstrates modular service design with asynchronous messaging, real-time client notifications, and distributed application composition using the Aspire framework.

---

## 🛠️ Tech Stack

### Core Technologies:
- **.NET Aspire** (.NET 9 distributed application orchestration)
- **.NET 9** for all projects
- **Blazor WebAssembly** (Client UI)
- **Web API with Controllers** (MasterDataService)
- **WolverineFx** (Messaging and Middleware)
- **RabbitMQ** (Message broker for asynchronous communication)
- **SignalR** (Real-time messaging between server and client)
- **Redis** (Caching)
- **SQL Server** (Data persistence)
- **YARP** (Reverse proxy to route traffic to services)
- **Docker** (Containerized environment)
- **Aspire Dashboard** (Distributed service visibility)

### Additional/Planned Integrations:
- **Keycloak** (Authentication/Authorization)
- **Handlebars** (Templating)
- **Prometheus + Grafana** (Metrics and monitoring)
- **Serilog** (Structured logging)
- **Elsa Workflows** (Workflow engine)

---

## 📐 Project Structure

```
Aspire-With-Friends/
│
├── AspireApp.AppHost/           # Aspire AppHost (entrypoint and container orchestration)
├── AspireApp.MasterDataService/ # API with SQL Server + Wolverine
├── AspireApp.WebWasm/           # Blazor WebAssembly frontend with SignalR integration
├── AspireApp.Yarp/              # Reverse proxy using YARP
├── AspireApp.Cache/             # Redis integration
└── AspireApp.RabbitMQ/          # RabbitMQ container configuration
```

---

## 📤 Message Flow Diagram

```
+---------------------+           RabbitMQ            +----------------------+
| MasterDataService   |  -------------------------->  |   Web App (Blazor)   |
| (Publishes Events)  |         Wolverine            | (Subscribes to Events)|
+---------------------+                              +----------------------+
     |                                                          |
     |              SignalR                                     |
     |             Notification  ---------------------------->  |
     |                                                          |
     |                    Blazor UI Updates                     |
     |                                                          |
```

---

## ✅ Features Implemented

- [x] MasterDataService migrated to use Controllers, Services, and Interfaces
- [x] Asynchronous publishing using Wolverine + RabbitMQ
- [x] Real-time client updates via SignalR
- [x] Blazor WebAssembly frontend
- [x] Docker-based container orchestration
- [x] YARP reverse proxy configured
- [x] Redis caching connected
- [x] Aspire Dashboard integration
- [x] Working Publish/Subscribe pattern between services

---

## 🧪 Testing

- Postman collections included for testing the API.
- Logs and events verified in `MasterDataService`, `RabbitMQ`, and `WebWasm`.
- Web clients receive updates upon database changes.

---

## 🧭 Future Enhancements

- Authentication via **Keycloak**
- Monitoring dashboards via **Prometheus/Grafana**
- Logging improvements via **Serilog**
- Custom workflow support with **Elsa**
- Shared contracts and message schemas between services
- Environment-specific configurations
- Unit + Integration tests

---

## 🚀 Getting Started

1. Clone the repository
2. Restore NuGet packages and build with `.NET 9 SDK`
3. Start the app via `AspireApp.AppHost`
4. You need Docker Desktop to run Aspire
4. Use Postman to test API (Json files are in the MasterDataService/Data/Postman)
5. Observe real-time notifications in the Blazor WebAssembly app
6. View service health and metrics in the Aspire Dashboard

---

## 🧩 Notes

- RabbitMQ must be healthy and use correct credentials (default `guest/guest` works only with localhost).
- Blazor WebAssembly requires signalR endpoints exposed in reverse proxy or direct.
- Aspire Dashboard runs automatically when using AppHost.

---

## 🧠 Author

This project was assembled step-by-step to understand different tech, Wolverine messaging, and real-time event propagation using the Aspire ecosystem.

---