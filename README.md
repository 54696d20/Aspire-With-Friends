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
- Authentication via **Keycloak**

### Observability & Monitoring
- **OpenTelemetry** for metrics and tracing
- **Prometheus** for metrics collection and storage
- **Grafana** for metrics visualization and dashboards
- **Real-time monitoring** of all .NET services

### External Integrations
- **WeatherAPI.com** for real-time weather data
- **Handlebars** templating (planned)
- **Serilog** logging (planned)
- **Elsa Workflows** (planned)

---

## 👤 Project Structure

```text
Aspire-With-Friends/
├── AspireApp.AppHost/               # Entry point that orchestrates services
├── AspireApp.MasterDataService/     # API with SQL Server + Wolverine
├── AspireApp.NotificationHubService/# Publishes SignalR notifications
├── AspireApp.WeatherAPI/            # Real-time weather service with WeatherAPI.com
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

## 🌐 YARP Gateway

The YARP gateway provides:

- **API Routing**: Routes requests to appropriate microservices
  - `/masterdata-api/*` → MasterDataService (CQRS with validation)
  - `/myweather-api/*` → WeatherAPI (real-time weather data)
- **CORS Support**: Allows Blazor WebAssembly to make cross-origin requests
- **Request/Response Logging**: Comprehensive logging for debugging
- **Health Checks**: `/health` endpoint for monitoring (basic health check)
- **Error Handling**: Proper error handling and logging

**Gateway Endpoints:**
- `GET http://localhost:5211/` - Gateway status
- `GET http://localhost:5211/health` - Health check
- `POST http://localhost:5211/masterdata-api/api/locations` - Create location (with validation)
- `GET http://localhost:5211/masterdata-api/api/locations` - Get all locations
- `PUT http://localhost:5211/masterdata-api/api/locations/{id}` - Update location (with validation)
- `DELETE http://localhost:5211/masterdata-api/api/locations/{id}` - Delete location
- `GET http://localhost:5211/myweather-api/api/weather/current/{query}` - Current weather
- `GET http://localhost:5211/myweather-api/api/weather/forecast/{query}?days=7` - 7-day forecast
- `GET http://localhost:5211/myweather-api/api/weather/search/{query}` - Location search

---

## ✅ Features

- Modular services with asynchronous messaging
- Real-time client updates through SignalR
- Redis caching and SQL Server storage
- **Enhanced YARP gateway** with CORS, logging, and health checks
- Docker-based orchestration via .NET Aspire AppHost
- **CQRS pattern** with Wolverine and FluentValidation
- **Domain events** for event-driven architecture
- **Comprehensive observability** with OpenTelemetry, Prometheus, and Grafana
- **Real-time weather data** integration with WeatherAPI.com
- **Interactive weather dashboard** with current conditions, forecasts, and location search
- **Secure API key management** using .NET user secrets and environment variables

---

## 🌤️ Weather API Integration

This project now includes a comprehensive weather service that provides real-time weather data:

### Features
- **Current Weather**: Real-time temperature, conditions, wind, humidity, and visibility
- **7-Day Forecast**: Extended weather predictions
- **Location Search**: Find weather for any city, zip code, or location
- **Responsive Dashboard**: Beautiful MudBlazor-based weather interface
- **Temperature Units**: Displays in both Fahrenheit and Celsius
- **Error Handling**: Graceful fallbacks and user-friendly error messages

### Setup Required
To use the weather functionality, you'll need to:
1. Get a free API key from [WeatherAPI.com](https://www.weatherapi.com/)
2. Configure it using .NET user secrets or environment variables
3. See [WEATHER_API_SETUP.md](WEATHER_API_SETUP.md) for detailed setup instructions

### Weather Dashboard
The weather page includes:
- Search functionality for any location
- Current weather display with detailed metrics
- Responsive grid layout for weather information
- Loading states and error handling
- Real-time data from WeatherAPI.com

---

## 💻 Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Aspire-With-Friends
   ```

2. **Prerequisites**
   - **.NET 9 SDK** installed
   - **Docker Desktop** running
   - **Git** for version control

3. **Optional Setup**
   - **WeatherAPI.com API key** for weather functionality (see [WEATHER_API_SETUP.md](WEATHER_API_SETUP.md))
   - **Keycloak** for authentication (see Authentication section below)

---

## 🚀 Running the Project

This project now uses a **modular infrastructure approach** that separates infrastructure services from application code, providing enterprise-level flexibility.

### 🏗️ Infrastructure-First Architecture

The project has been restructured to use external infrastructure services that can be:
- **Started independently** of the application
- **Scaled separately** from the application
- **Used across multiple environments** (dev, staging, production)
- **Managed independently** by DevOps teams

### 📁 Project Structure

```
Aspire-With-Friends/
├── infrastructure/                    # 🆕 Infrastructure services (Docker)
│   ├── start-aspire.sh              # Start infrastructure for Aspire development
│   ├── start-full-docker.sh         # Start everything in Docker
│   ├── start-infrastructure.sh      # Start just infrastructure
│   └── stop-all.sh                  # Stop all services
├── AspireApp.AppHost/               # .NET Aspire orchestration
├── AspireApp.MasterDataService/     # API with SQL Server + Wolverine
├── AspireApp.NotificationHubService/# SignalR notifications
├── AspireApp.WeatherAPI/            # Real-time weather service
├── AspireApp.WebWasm/               # Blazor WebAssembly frontend
├── YarpGateway/                     # Reverse proxy
└── AspireApp.Shared/                # Shared contracts
```

---

## 🚨 Why This New Approach?

### Benefits
- **Separation of Concerns**: Infrastructure separate from application code
- **Flexible Deployment**: Choose your deployment scenario
- **Environment Configuration**: Easy to customize per environment
- **Independent Scaling**: Monitoring can scale independently
- **Easier Maintenance**: Infrastructure changes don't require app rebuilds
- **Production Ready**: Same setup can be used in production
- **Security**: Infrastructure services can be secured independently

### Migration from Old Setup
If you were using the old docker-compose files:
1. **Stop old services**: `docker-compose down` (in project root)
2. **Start new infrastructure**: `cd infrastructure && ./start-aspire.sh`
3. **Start Aspire AppHost**: `dotnet run --project AspireApp.AppHost`

---

## 🔐 Authentication & Keycloak Setup

This project uses **Keycloak** for authentication and role-based access control in the Blazor WebAssembly app.

### Starting Keycloak

**Option 1: Use Full Docker Deployment**
```bash
cd infrastructure
./start-full-docker.sh
```
This includes Keycloak automatically.

**Option 2: Start Keycloak Separately**
```bash
# Start just Keycloak and PostgreSQL
docker-compose -f docker-compose.docker.yml up -d keycloak postgres
```

### Keycloak Access
- **Keycloak Admin Console**: http://localhost:8080/admin
- **Default admin credentials**: `admin` / `admin`

---

## 🎯 Two Ways to Run

### 1. **Aspire Development** (Recommended for .NET Developers)

**Best for**: .NET development, debugging, using Aspire dashboard

1. **Start Infrastructure Services**
   ```bash
   cd infrastructure
   ./start-aspire.sh
   ```
   This starts: SQL Server, Redis, RabbitMQ, Prometheus, Grafana

2. **Start Aspire AppHost** (in a new terminal)
   ```bash
   # From project root
   dotnet run --project AspireApp.AppHost
   ```

3. **Access Services**
   - **Aspire Dashboard**: http://localhost:15262
   - **Blazor Web App**: http://localhost:5071
   - **Infrastructure Services**: See URLs displayed by start-aspire.sh

### 2. **Full Docker Deployment**

**Best for**: Full-stack demos, production-like environment, "one command to run it all"

1. **Start Everything**
   ```bash
   cd infrastructure
   ./start-full-docker.sh
   ```
   This starts: All infrastructure + all .NET applications

2. **Access Services**
   - **Blazor Web App**: http://localhost:5071
   - **Gateway**: http://localhost:5211
   - **MasterDataService**: http://localhost:5316
   - **WeatherAPI**: http://localhost:5062
   - **Infrastructure Services**: See URLs displayed by start-full-docker.sh

---

## 🔧 Infrastructure Configuration

### Environment Setup

1. **Copy Environment Template**
   ```bash
   cd infrastructure
   cp env.example .env
   ```

2. **Customize .env** (optional - defaults work for development)
   ```bash
   # Edit .env with your preferred values
   nano .env
   ```

### Default Credentials

| Service | Username | Password | URL |
|---------|----------|----------|-----|
| SQL Server | `sa` | `P@ssword123!` | localhost:1433 |
| Redis | - | `devpassword` | localhost:6379 |
| RabbitMQ | `devuser` | `devpassword` | localhost:5672 |
| Grafana | `admin` | `admin` | http://localhost:3000 |

---

## 🛑 Stopping Services

### Stop Infrastructure Only
```bash
cd infrastructure
docker-compose -f docker-compose.base.yml -f docker-compose.monitoring.yml down
```

### Stop Everything
```bash
cd infrastructure
./stop-all.sh
```

### Stop Aspire AppHost
- Use `Ctrl+C` in the terminal running the AppHost
- Or use the cleanup script: `./scripts/cleanup-aspire.sh`

---

## 🔍 Troubleshooting

### Port Conflicts
If you encounter port conflicts:
```bash
# Clean up all Aspire containers
./scripts/cleanup-aspire.sh

# Or manually stop and remove
docker stop $(docker ps -q)
docker rm $(docker ps -aq)
```

### Infrastructure Not Starting
1. **Check Docker**: Ensure Docker Desktop is running
2. **Check Ports**: Ensure required ports are available
3. **Check .env**: Verify environment configuration
4. **View Logs**: `docker-compose logs [service-name]`

### Aspire Can't Connect
1. **Verify Infrastructure**: Check if infrastructure services are running
2. **Check URLs**: Ensure service URLs match configuration
3. **Check Credentials**: Verify database and Redis credentials
4. **View AppHost Logs**: Check the terminal running the AppHost

---

## 🏗 CQRS & Wolverine Architecture

This project uses the **CQRS (Command Query Responsibility Segregation)** pattern for all API endpoints, powered by [Wolverine](https://wolverine.netlify.app/):

- **Commands** (e.g., `CreateLocationCommand`) change state and are handled by dedicated handler classes.
- **Queries** (e.g., `GetAllLocationsQuery`) read state and are handled by their own handlers.
- **Wolverine** is used as the mediator to dispatch commands/queries to their handlers, and to publish events for real-time updates.

### Why CQRS?
- Clear separation of read and write logic
- Easier to test and maintain
- Handlers encapsulate all business logic and side effects (like event publishing)

### How it Works
- **Controller** receives an HTTP request and sends a command/query via Wolverine's bus:
  ```csharp
  var result = await _bus.InvokeAsync<TResult>(commandOrQuery);
  ```
- **Wolverine** automatically finds and invokes the correct handler based on the message type.
- **Handler** performs the business logic and (if needed) publishes events for other services or the UI.

### Example

**Command:**
```csharp
public record CreateLocationCommand(string Name, string Type, int? ParentId);
```
**Handler:**
```csharp
public class CreateLocationHandler
{
    // ... constructor with DI

    public async Task<int> Handle(CreateLocationCommand command)
    {
        // Insert into DB, publish event, return new ID
    }
}
```
**Controller:**
```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateLocationCommand command)
{
    var id = await _bus.InvokeAsync<int>(command);
    return CreatedAtAction(nameof(GetById), new { id }, command);
}
```

### Adding a New Command or Query

1. Create a record for your command/query in `Messages/Commands` or `Messages/Queries`.
2. Create a handler class with a `Handle` method for your command/query.
3. In your controller, use `_bus.InvokeAsync<TResult>(commandOrQuery)` to dispatch.

---

## 📊 Observability & Monitoring

This application includes comprehensive observability with **OpenTelemetry**, **Prometheus**, and **Grafana**:

### What's Monitored
- **HTTP Metrics**: Request rates, response times, error rates
- **Process Metrics**: CPU, memory usage, garbage collection
- **Runtime Metrics**: Thread pool, exception counts
- **Custom Metrics**: Business-specific metrics (when added)

### Accessing Monitoring Tools

**When using Aspire Development mode:**
- **Prometheus**: http://localhost:9090 - Raw metrics and query interface
- **Grafana**: http://localhost:3000 - Dashboards and visualizations
- **Aspire Dashboard**: http://localhost:15262 - Service health and logs

**When using Full Docker mode:**
- **Prometheus**: http://localhost:9090 - Raw metrics and query interface
- **Grafana**: http://localhost:3000 - Dashboards and visualizations
- **All services**: Accessible via the URLs displayed by start-full-docker.sh

### Quick Start

1. **Start Infrastructure** (includes monitoring):
   ```bash
   cd infrastructure
   ./start-aspire.sh
   ```

2. **Start Aspire AppHost**:
   ```bash
   dotnet run --project AspireApp.AppHost
   ```

3. **Access Monitoring**:
   - Grafana: http://localhost:3000 (admin/admin)
   - Prometheus: http://localhost:9090
   - Aspire Dashboard: http://localhost:15262

### Monitoring Setup (Automatic)

The infrastructure scripts automatically:
- ✅ Start Prometheus and Grafana
- ✅ Configure service discovery
- ✅ Set up basic dashboards
- ✅ Configure health checks

**No manual configuration required!** The monitoring services are pre-configured and ready to use.

For detailed observability documentation, see [OpenTelemetry-README.md](OpenTelemetry-README.md).

**💡 Tip**: For detailed infrastructure information, see [infrastructure/README.md](infrastructure/README.md) - it contains comprehensive details about all infrastructure services, deployment scenarios, and enterprise benefits.

---

## 🧪 Testing with Postman

This project includes comprehensive Postman collections for testing the API endpoints and validation:

### Available Collections

**1. AspireApp YARP Collection - CQRS with Validation** *(Recommended)*
- **Location**: `AspireApp.MasterDataService/Data/Postman/AspireApp_YARP_Collection.json`
- **Purpose**: Test all endpoints through the YARP gateway with CQRS and validation
- **Features**:
  - Gateway health checks
  - All CRUD operations for locations
  - Validation test cases (valid and invalid data)
  - Weather API endpoints
- **Port**: `5211` (YARP Gateway)

**2. AspireApp MasterDataService - Direct API**
- **Location**: `AspireApp.MasterDataService/Data/Postman/AspireApp_Locations_Postman_Collection.json`
- **Purpose**: Test MasterDataService directly (bypassing YARP)
- **Features**:
  - Basic CRUD operations for locations
  - Direct service testing
- **Port**: `5001` (MasterDataService directly)
- **Note**: This collection doesn't include validation test cases and uses the old API structure

### Import Instructions

1. **Open Postman**
2. **Import Collection**: File → Import → Select the JSON file
3. **Set Environment**: The collection uses `localhost:5211` for the YARP gateway
4. **Test Validation**: Use the "Invalid" test cases to verify FluentValidation works

### Test Cases Included

**Gateway Tests:**
- ✅ Gateway Status (`GET /`)
- ✅ Health Check (`GET /health`)

**Master Data API Tests:**
- ✅ Get All Locations
- ✅ Get Location by ID
- ✅ Create Location (Valid & Invalid)
- ✅ Update Location (Valid & Invalid)
- ✅ Delete Location

**Weather API Tests:**
- ✅ Current Weather (`GET /myweather-api/api/weather/current/{query}`)
- ✅ Weather Forecast (`GET /myweather-api/api/weather/forecast/{query}?days=7`)
- ✅ Location Search (`GET /myweather-api/api/weather/search/{query}`)

**Validation Test Cases:**
- ❌ Empty name validation
- ❌ Invalid location type validation
- ✅ Valid data acceptance

### Expected Responses

**Valid Request:**
```json
POST /masterdata-api/api/locations
{
  "name": "Main Building",
  "type": "Building",
  "parentId": null
}
```
**Response:** `201 Created` with location ID

**Invalid Request:**
```json
POST /masterdata-api/api/locations
{
  "name": "",
  "type": "InvalidType",
  "parentId": null
}
```
**Response:** `400 Bad Request` with validation errors

---