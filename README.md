# 🛠️ Product API - .NET Web API with Authentication & Authorization

This project is a clean and layered .NET 8 Web API for product management, featuring JWT-based authentication and role-based authorization.

## 🚀 Features
- User registration (Sign Up)
- Login with JWT + Refresh Token
- Token renewal (Refresh Token Rotation)
- Password hashing (SHA256)
- Role-based authorization (`[Authorize(Roles = "admin")]`)
- Centralized token and response management
- Layered architecture: Repository, Service, API
- Swagger UI support for testing

## 🗂️ Project Layers
- **App.API**: Controllers and `CustomBaseController` for unified response handling
- **App.Repositories**: DbContext, entities, Generic & custom repositories
- **App.Services**: DTO usage, business logic, token operations
- **App.Services.Auth.Jwt**: JWT settings and token generation logic
- **App.Services.ServiceResults**: Generic wrapper for service responses

## 🔐 Authentication & Authorization

### Password Hashing
- Secure password hashing using SHA256

### JWT Access Token
- Includes UserId, Username, Role claims
- Valid for 1 hour (configurable)

### Refresh Token
- Secure 64-byte Base64 random string
- Valid for 7 days
- Refreshed on each login or token renewal

## 📦 API Endpoints

### Users
|Method          |Route                             |Access                       |
|----------------|----------------------------------|-----------------------------|
|POST            |`/api/users`                      |Public                       |
|GET             |`/api/users/{username}`           |Public                       |

### Authentication
|Method          |Route                             |Access                       |
|----------------|----------------------------------|-----------------------------|
|POST            |`/api/auth/login`                 |Public                       |
|POST            |`/api/auth/refresh-token`         |Public                       |
|GET             |`/api/auth/admin-area`            |Only `admin` role            |

### Products
|Method          |Route                             |Access                       |
|----------------|-------------------------------   |-----------------------------|
|GET             |`/api/products`                   |All users                    |
|GET             |`/api/products/top-price/{count}` |All users                    |
|GET             |`/api/products/{id}`              |All users                    |
|POST            |`/api/products`                   |Only `admin` role            |
|PUT             |`/api/products/{id}`              |Only `admin` role            |
|PATCH           |`/api/products/stock`             |Only `admin` role            |
|DELETE          |`/api/products/{id}`              |Only `admin` role            |

## ⚙️ Getting Started

```
git clone https://github.com/mrantikadev/AuthenticationAndAuthorization
dotnet restore
dotnet ef database update
dotnet run
```

## 🔑 Testing via Swagger

1. Use `POST /api/auth/login` to log in
2. Copy the returned **Access Token** and click the **Authorize** button in Swagger
   ```
   Bearer eyJhbGciOiJIUzI1NiIs...
   ```
3. Test protected endpoints like `/products` and `/auth/admin-area`

## 📌 Notes
- Access tokens are short-lived — refresh using the refresh token
- `CustomBaseController` provides consistent and clean controller responses
- `ServiceResult<T>` ensures unified success/error response handling

## ✨ Contributions & Extensions

This project is perfect for learning and extending authentication/authorization concepts.
> Easily extendable with: refresh token blacklist, password reset, profile updates, email verification, etc.

## ⚠️ Keep in Mind
- `appsettings.Development.json` includes a local SQL Server connection string.
- **Please update `"ConnectionStrings"` based on your environment**.

- The JWT signing key in `appsettings.json` is for demonstration only.
- **In production, always move it to environment variables or a secrets manager**.
