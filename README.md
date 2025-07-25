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

### Planned Integrations

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

---

## Running the Project: Two Approaches

This project supports two main ways to run the full stack for local development:

### 1. AppHost (Aspire) Way
- Uses .NET Aspire’s AppHost to orchestrate .NET services and infrastructure.
- Best for rapid development, debugging, and using the Aspire dashboard.
- External dependencies like Keycloak must be started separately (e.g., with a dedicated Docker Compose file).

### 2. Docker Compose Way
- Uses Docker Compose to start all services (including .NET apps, Keycloak, databases, etc.) in containers.
- Best for simulating a production-like environment or running everything with a single command.
- May start more services than you need for development.

**Choose the approach that best fits your workflow!**

---

## 🚀 Running with AppHost (Aspire)

1. Start only the required infrastructure (e.g., Keycloak) with:
   ```bash
   docker compose -f docker-compose.Aspire.yml up -d
   ```
2. In a separate terminal, run:
   ```bash
   dotnet run --project AspireApp.AppHost
   ```
3. Access the Aspire dashboard and Blazor app as described above.

---

## 🐳 Running with Docker Compose

1. Start all services (including .NET apps, Keycloak, databases, etc.):
   ```bash
   docker compose -f docker-compose.Docker.yml up -d
   ```
2. Access the Blazor app at [http://localhost:5071](http://localhost:5071).

---

## Why Two Ways?

- **AppHost (Aspire) Way:**  
  Great for .NET-centric development, debugging, and using Aspire’s orchestration features.  
  Lets you control which infra services are running. AND... Keycloak at this time doesn't in within Aspire (07/2025)
- **Docker Compose Way:**  
  Great for full-stack integration testing, demos, or when you want to run everything in containers.  
  Easiest for onboarding or “one command to run it all.”

---

## 🔐 Authentication & Keycloak Setup

This project uses **Keycloak** for authentication and role-based access control in the Blazor WebAssembly app.

### 1. Start Keycloak (via Docker Compose)

Keycloak is included in the `docker-compose.docker.yml`. To start Keycloak:

```bash
docker compose up -d keycloak postgres
```
- Keycloak will be available at [http://localhost:8080](http://localhost:8080)
- Default admin credentials: `admin` / `admin`

### 2. Create Realm, Client, and Roles

1. **Login to Keycloak Admin Console** at [http://localhost:8080/admin](http://localhost:8080/admin)
2. **Create a new Realm** (e.g., `AspireRealm`)
3. **Create a new Client**:
   - Client ID: `aspire-blazor-client`
   - Protocol: `openid-connect`
   - Next
   - Authentication flow - only have Standard flow selected
   - Next 
   - Root URL: `http://localhost:5071`
   - Valid Redirect URIs: `http://localhost:5071/*`
   - Valid Post Logout Redirect URIs: `http://localhost:5071/`
   - Web Origins: `http://localhost:5071`
   - Save
   - Client authentication: **OFF** (public client)
   - Standard flow: **ON**
   - Save
4. **Create Realm Roles** (`godmode`) (Is casesensitive)
   - Go to Roles > Create Role
   - Name: `godmode` (repeat for other roles as needed)
   
5. **Create a Test User**
   - Go to Users > Create user
   - Set username, email, etc. and save
   - Go to Credentials tab, set a password, and disable 'Temporary'
   - Go to Role mapping tab, assign `godmode` (or other roles) to the user

6. **(If Using Realm Roles) Add a Protocol Mapper**
   - If you assign roles under the Realm (not as client roles), you must add a protocol mapper to include them in the token:
   1. Go to **Clients** > select your client (`aspire-blazor-client`).
   2. Go to the **Client scopes** tab.
   3. Clink on aspire-blazor-client-dedicated
   3. Click **Create Mapper**, by configuration
   4. Select User Real Role
   4. Set:
      - **Name:** admin roles
      - **Mapper Type:** User Client Role
      - **Token Claim Name:** `role`
      - **Claim JSON Type:** Array
      - **Add to ID token:** ON
      - **Add to access token:** ON
      - **Add to userinfo:** ON
   5. Save.

   Now, roles assigned under the client will appear in the token as a `role` claim.

At this point you should be able to login with that user

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
- **Controller** receives an HTTP request and sends a command/query via Wolverine’s bus:
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