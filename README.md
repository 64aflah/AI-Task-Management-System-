# AI Task Management System - Setup Guide

## Prerequisites
- .NET 8 SDK installed
- SQL Server LocalDB installed
- Redis Server running (or Docker container)
- Visual Studio 2022+ or VS Code

## Quick Setup Instructions

### 1. Database Setup

#### Option A: Using Entity Framework Core Migrations (Recommended)

```powershell
# Navigate to the Infrastructure project directory
cd AI.TaskManagement.Infrastructure

# Apply migrations (automatically runs InitialCreate)
Update-Database -Context ApplicationDbContext

# This will:
# - Create the AITaskManagementDB database
# - Create all tables (Users, Roles, Tasks, Comments, Notifications, RefreshTokens)
# - Seed Admin role, Manager role, and Member role
# - Create default Admin user with credentials:
#   Email: admin@taskmanagement.com
#   Password: Admin@123456
```

#### Option B: Using .NET CLI

```powershell
# From solution root
dotnet ef database update --project AI.TaskManagement.Infrastructure
```

### 2. Redis Setup

#### Option A: Using Docker (Recommended)

```powershell
docker run -d -p 6379:6379 --name redis redis:latest
```

#### Option B: Local Redis Installation

- Download and install Redis from https://redis.io/download
- Ensure Redis is running on localhost:6379

### 3. Environment Configuration

Update `appsettings.json` in `AI.TaskManagement.API`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=AITaskManagementDB;...",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "Key": "your_super_secret_key_that_should_be_at_least_32_characters_long_for_security",
    "Issuer": "AITaskManagementAPI",
    "Audience": "AITaskManagementClient",
    "ExpirationMinutes": 15
  }
}
```

### 4. Run the Application

```powershell
# From solution root
dotnet run --project AI.TaskManagement.API

# Or using Visual Studio:
# F5 (Debug) or Ctrl+F5 (Release)
```

The API will be available at: `https://localhost:5001`

### 5. Access Swagger Documentation

Navigate to: `https://localhost:5001/swagger/index.html`

**Credentials for testing:**
- Email: `admin@taskmanagement.com`
- Password: `Admin@123456`

## Project Structure

```
AI.TaskManagement.Domain/
├── Entities/              # Core domain models
├── Enums/                 # Task priority, status, role enums
├── Interfaces/            # Repository, UnitOfWork contracts
└── Specifications/        # Query specifications for filtering

AI.TaskManagement.Application/
├── Features/              # CQRS Commands, Queries, Handlers
├── DTOs/                  # Data transfer objects
├── Mappings/              # AutoMapper profiles
├── Validators/            # FluentValidation validators
└── ApplicationServiceRegistration.cs  # DI setup

AI.TaskManagement.Infrastructure/
├── Data/                  # DbContext and configurations
├── Repositories/          # Repository and UnitOfWork implementations
├── Services/              # JWT, Cache, Notification services
├── Migrations/            # EF Core migrations
└── InfrastructureServiceRegistration.cs  # DI setup

AI.TaskManagement.API/
├── Controllers/           # API endpoints
├── Middleware/            # Exception, Logging middleware
├── Program.cs             # Main entry point with DI configuration
└── appsettings.json       # Configuration

AI.TaskManagement.Shared/
├── Responses/             # ApiResponse wrapper
├── Exceptions/            # Custom exceptions
├── DTOs/                  # Common DTOs (Pagination)
├── Constants/             # API routes
└── Utilities/             # PasswordHasher

AI.TaskManagement.Tests/
└── Unit tests with xUnit and Moq
```

## API Endpoints

### Authentication
- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `POST /api/v1/auth/refresh-token` - Refresh access token

### Tasks
- `GET /api/v1/tasks` - Get all tasks (paginated)
- `GET /api/v1/tasks/{id}` - Get task by ID
- `GET /api/v1/tasks/user/{userId}` - Get user's tasks
- `POST /api/v1/tasks` - Create task
- `PUT /api/v1/tasks/{id}` - Update task
- `DELETE /api/v1/tasks/{id}` - Delete task (soft delete)
- `POST /api/v1/tasks/{id}/assign` - Assign task to user

### Comments
- `GET /api/v1/comments/task/{taskId}` - Get task comments
- `GET /api/v1/comments/{id}` - Get comment by ID
- `POST /api/v1/comments` - Create comment
- `PUT /api/v1/comments/{id}` - Update comment
- `DELETE /api/v1/comments/{id}` - Delete comment

### Users
- `GET /api/v1/users` - Get all users (Admin only)
- `GET /api/v1/users/{id}` - Get user by ID
- `GET /api/v1/users/profile/current` - Get current user profile

### Notifications
- `GET /api/v1/notifications` - Get user notifications
- `WS /hubs/notifications` - SignalR hub for real-time notifications

## Features

✅ Clean Architecture with CQRS pattern
✅ JWT Authentication with Refresh Tokens
✅ Role-based Access Control (Admin, Manager, Member)
✅ Soft Delete support
✅ Redis Caching
✅ SignalR Real-time Notifications
✅ Pagination and Filtering
✅ FluentValidation
✅ AutoMapper
✅ Comprehensive Error Handling
✅ Serilog Logging
✅ Swagger/OpenAPI Documentation

## Testing

```powershell
# Run all unit tests
dotnet test

# Run specific test project
dotnet test AI.TaskManagement.Tests
```

## NuGet Packages Used

See NUGET_PACKAGES.md for complete list of dependencies.

## Docker Deployment

See DOCKER_DEPLOYMENT.md for containerization instructions.

## Troubleshooting

### Database Connection Issues
- Ensure SQL Server LocalDB is installed and running
- Check connection string in appsettings.json
- Verify MSSQLLocalDB instance: `sqllocaldb info`

### Redis Connection Issues
- Ensure Redis is running: `redis-cli ping` should return "PONG"
- Check Redis connection string in appsettings.json

### JWT Token Issues
- Ensure JWT key length is at least 32 characters
- Check token expiration time in appsettings.json
- Verify issued claims match validation parameters

## Production Considerations

1. **Change Default Admin Password** - Change immediately in production
2. **Update JWT Secret** - Use a strong, environment-specific secret
3. **Enable HTTPS** - Configure SSL certificates
4. **Add Rate Limiting** - Implement API rate limiting
5. **Database Backups** - Set up automated backups
6. **Monitoring** - Implement application performance monitoring
7. **Security** - Implement additional security headers, CORS restrictions
8. **Logging** - Configure Serilog for production environments

## Support

For issues or questions, please create an issue in the repository.
