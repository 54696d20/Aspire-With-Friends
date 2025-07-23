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
  Lets you control which infra services are running.
- **Docker Compose Way:**  
  Great for full-stack integration testing, demos, or when you want to run everything in containers.  
  Easiest for onboarding or “one command to run it all.”

---

## 🔐 Authentication & Keycloak Setup

This project uses **Keycloak** for authentication and role-based access control in the Blazor WebAssembly app.

### 1. Start Keycloak (via Docker Compose)

Keycloak is included in the `docker-compose.yml`. To start Keycloak:

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
   - Root URL: `http://localhost:5071`
   - Valid Redirect URIs: `http://localhost:5071/*`
   - Valid Post Logout Redirect URIs: `http://localhost:5071/`
   - Web Origins: `http://localhost:5071`
   - Client authentication: **OFF** (public client)
   - Standard flow: **ON**
   - Save
4. **Create Realm Roles** (e.g., `godmode`, `user`)
   - Go to Roles > Create Role
   - Name: `godmode` (repeat for other roles as needed)
5. **Create a Test User**
   - Go to Users > Add user
   - Set username, email, etc. and save
   - Go to Credentials tab, set a password, and disable 'Temporary'
   - Go to Role mapping tab, assign `godmode` (or other roles) to the user

6. **(If Using Client Roles) Add a Protocol Mapper**
   - If you assign roles under the client (not as realm roles), you must add a protocol mapper to include them in the token:
   1. Go to **Clients** > select your client (`aspire-blazor-client`).
   2. Go to the **Client scopes** or **Client scopes > Mappers** tab.
   3. Click **Create** (or **Add built-in**).
   4. Set:
      - **Name:** client roles
      - **Mapper Type:** User Client Role
      - **Client ID:** `aspire-blazor-client`
      - **Token Claim Name:** `role`
      - **Claim JSON Type:** String or Array
      - **Add to ID token:** ON
      - **Add to access token:** ON
      - **Add to userinfo:** ON
   5. Save.

   Now, roles assigned under the client will appear in the token as a `role` claim.

### 3. Configure the Blazor App

The Blazor WebAssembly app is preconfigured to use Keycloak at `http://localhost:8080/realms/AspireRealm` and expects the client ID `aspire-blazor-client`.

If you change the realm or client name, update `AspireApp.WebWasm/Program.cs` accordingly:

```csharp
options.ProviderOptions.Authority = "http://localhost:8080/realms/AspireRealm";
options.ProviderOptions.ClientId = "aspire-blazor-client";
options.ProviderOptions.RedirectUri = "http://localhost:5071/authentication/login-callback";
options.ProviderOptions.PostLogoutRedirectUri = "http://localhost:5071/";
```

### 4. Assigning Roles for Role-Based UI

- Assign realm roles (e.g., `