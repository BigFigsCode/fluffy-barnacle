# Insurance Policy Management System

A full-stack web application for managing insurance products, policies and claims. Customers can browse insurance products, request coverage, and submit claims against their active policies. Administrators review and activate policies, process claims, and manage the product catalog.

The project has two parts:

| Folder | What it is | Built with |
| --- | --- | --- |
| [`InsuranceAPI/`](InsuranceAPI) | REST API and business logic | ASP.NET Core (.NET 10), Entity Framework Core, SQL Server, JWT authentication |
| [`insurance-frontend/`](insurance-frontend) | Single-page web app | Angular 22, TypeScript |

## What the application does

### For customers
- **Register and log in.** New accounts are created as customers. Logging in returns a JWT that the app stores for the browser session and sends with every API request.
- **Browse products.** View the available insurance products with their descriptions and base premiums.
- **Request coverage.** Choosing a product creates a one-year policy that starts out as *Pending* until an administrator approves it.
- **View my policies.** See each policy's product, status, and start and end dates.
- **Submit and track claims.** File a claim (up to 1,000 characters) against any of your *Active* policies and follow its status as it moves through review.

### For administrators
- **Dashboard overview.** Totals for all policies, pending policies and claims.
- **Activate policies.** Approve pending policy requests so they become *Active*.
- **Process claims.** Move a claim from *Submitted* to *Under Review*, then *Approve* or *Deny* it. Approved and denied claims are final and can't be changed.
- **Manage products.** Create and update insurance products, and deactivate or reactivate them. Customers can't request coverage for an inactive product.

### Policy and claim lifecycle

```
Policy:  Pending ──(admin activates)──▶ Active          (Expired / Cancelled also exist in the model)

Claim:   Submitted ──▶ Under Review ──▶ Approved
                                   └──▶ Denied
```

## Data model

- **User**: first and last name, email (unique), hashed password, and role (`Admin` or `Customer`).
- **InsuranceProduct**: name, description, base premium, and whether it's active.
- **Policy**: links a user to a product, with start and end dates and a status.
- **Claim**: belongs to a policy, with a description, submission date and status.

Passwords are hashed with ASP.NET Core Identity's `PasswordHasher`. They are never stored or returned in plain text.

## API endpoints

All routes are prefixed with `/api`. Endpoints marked 🔒 need a valid JWT (`Authorization: Bearer <token>`).

| Method | Route | Access | Description |
| --- | --- | --- | --- |
| POST | `/Auth/Register` | Public | Create a customer account |
| POST | `/Auth/Login` | Public | Log in and receive a JWT |
| GET | `/Auth/Profile` | 🔒 Any user | Return the current user's ID and email |
| GET | `/InsuranceProduct` | Public | List all products |
| GET | `/InsuranceProduct/{id}` | Public | Get one product |
| POST | `/InsuranceProduct` | 🔒 Admin | Create a product |
| PUT | `/InsuranceProduct/{id}` | 🔒 Admin | Update a product |
| PATCH | `/InsuranceProduct/{id}/deactivate` | 🔒 Admin | Deactivate a product |
| PATCH | `/InsuranceProduct/{id}/activate` | 🔒 Admin | Reactivate a product |
| POST | `/Policy` | 🔒 Any user | Request a policy for a product |
| GET | `/Policy/MyPolicies` | 🔒 Any user | List your own policies |
| GET | `/Policy/AllPolicies` | 🔒 Admin | List every policy |
| PATCH | `/Policy/{id}/activate` | 🔒 Admin | Activate a pending policy |
| POST | `/Claim` | 🔒 Customer | Submit a claim on an active policy |
| GET | `/Claim/MyClaims` | 🔒 Customer | List your own claims |
| GET | `/Claim/AllClaims` | 🔒 Admin | List every claim |
| PATCH | `/Claim/{id}/status` | 🔒 Admin | Update a claim's status |

When the API runs in Development mode, Swagger UI is available at `/swagger` for exploring and testing these endpoints.

## Running locally

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB, Express or full)
- [Node.js](https://nodejs.org/) and npm

### 1. Start the API

1. In `InsuranceAPI/appsettings.json`, set `ConnectionStrings:InsuranceConnection` to point at your SQL Server instance.
2. Set a JWT signing key. The key is a secret, so it isn't kept in `appsettings.json`. Store it with .NET user secrets, which keeps it outside the repo:

   ```bash
   cd InsuranceAPI
   dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)"
   ```

   The key has to be at least 32 bytes long, and the API won't start without one.
3. Create the database and run the API:

   ```bash
   dotnet tool install --global dotnet-ef   # first time only
   dotnet ef database update
   dotnet run --launch-profile https
   ```

   The API listens on `https://localhost:7292` and `http://localhost:5085`.

> **Note:** The `Claim` table is configured with a database trigger, `trg_ClaimStatusAudit`, for auditing status changes. The migrations don't create this trigger, so add it to your database separately if you want the audit trail.

### 2. Start the frontend

```bash
cd insurance-frontend
npm install
npm start
```

Then open `http://localhost:4200`.

The frontend's API address is set in `src/app/services/insurance.ts` (`apiUrl`), and it currently points at the hosted Azure API. To use your local API, change it to `http://localhost:5085/api`. The API's CORS policy already allows `http://localhost:4200`.

### Creating an admin account

Everyone who registers through the app becomes a `Customer`. To get an admin, register a user and then change their `Role` to `0` (Admin) in the `Users` table.

## Deployment

The project is set up for Azure:
- **API**: Azure App Service.
- **Frontend**: Azure Static Web Apps. `public/staticwebapp.config.json` sends all routes to `index.html` so Angular's client-side routing works.

In the App Service, set the JWT signing key as an app setting named `Jwt__Key` (Settings → Environment variables), or keep it in Azure Key Vault. Use a long random value and don't commit it. If someone knows the signing key, they can forge tokens, including admin tokens.
