# Aspire-With-Friends

This is a distributed microservice solution built using .NET Aspire, with the following components:

- **AspireApp.AppHost**: Entry point orchestrator using Aspire hosting.
- **AspireApp.MasterDataService**: Handles location data using a SQL Server backend and Wolverine messaging.
- **AspireApp.Web**: Frontend Blazor WebAssembly UI.
- **AspireApp.YARP**: YARP reverse proxy to route API traffic.
- **RabbitMQ**: Event-based messaging using Wolverine.
- **SQL Server**: Dockerized SQL instance for persistent data.

---

## 🧱 Project Structure

```
Aspire-With-Friends/
├── AspireApp.AppHost/
├── AspireApp.MasterDataService/
├── AspireApp.Web/
├── AspireApp.YARP/
└── docker-compose.yml (via Aspire AppHost)
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Optional: [JetBrains Rider](https://www.jetbrains.com/rider/) or [Visual Studio 2022+]

---

### 🏁 Running the App

1. **Set Docker Runtime**  
   If you're on macOS, ensure Docker is used:
   ```bash
   export DOTNET_HOSTING_CONTAINER_RUNTIME=docker
   ```

2. **Run AppHost**  
   Start everything from the AppHost project:
   ```bash
   dotnet run --project AspireApp.AppHost
   ```

3. **Access the Web App**  
   Navigate to: [http://localhost:5273](http://localhost:5273)

---

## 📦 Services & Endpoints

### Reverse Proxy (YARP)

#### Weather API
```
GET http://localhost:5273/myweather-api/weatherforecast
```

#### Master Data API
```
GET    http://localhost:5273/masterdata-api/api/locations
POST   http://localhost:5273/masterdata-api/api/locations
PUT    http://localhost:5273/masterdata-api/api/locations/{id}
DELETE http://localhost:5273/masterdata-api/api/locations/{id}
```

> The routes are configured via `appsettings.json` in the YARP project.

---

## 🐇 RabbitMQ

- Management UI: [http://localhost:15672](http://localhost:15672)  
- Default credentials:
  - **User**: `guest`
  - **Password**: `guest`

---

## 📬 Messaging (Wolverine)

- Wolverine is set up in MasterDataService to support message-based communication (future Sagas or async event publishing).
- The service uses `.AutoProvision()` to configure necessary queues.

---

## 🧪 Testing with Postman

Use the provided Postman collection:

1. [Download AspireApp_YARP_Collection.zip](./AspireApp_YARP_Collection.zip)
2. Import into Postman.
3. Run the requests for Weather and MasterData through the proxy.

---

## ⚠️ Troubleshooting

- **Docker containers not running**:
  - Ensure Docker Desktop is started.
  - Clean build: `docker system prune -af`
- **RabbitMQ errors**:
  - Check if ports `5672` and `15672` are available.
  - Restart the container via Docker Desktop if necessary.
- **SQL Connection Errors**:
  - Confirm that `localhost` is used in connection strings during development (inside the container, use `sqlserver` hostname).

---

## 📝 License

MIT License
