# Soda

## About the Project

Soda is a distributed application built using .NET Aspire, demonstrating modern cloud-native application development
practices with .NET 9.0.

## Technologies Used

- .NET 9.0
- ASP.NET Core
- .NET Aspire
- C# 13.0
- xUnit for Testing
- Auth0 for Authentication/Authorization
- Swagger/OpenAPI
- PostgreSQL

## .NET Aspire Implementation

This project leverages .NET Aspire, a new stack for building cloud-native applications with .NET. Key features include:

- Distributed application orchestration
- Built-in service discovery
- Centralized logging and metrics
- Development-time container orchestration
- Cloud-ready configuration

## Testing

The solution uses xUnit as the testing framework with:

- Unit tests for business logic and services
- Integration tests for API endpoints
- Mock frameworks for dependencies
- Test data builders and fixtures
- Code coverage reporting

## Authentication & Authorization

Authentication and authorization are handled via Auth0:

- JWT token-based authentication
- Role-based access control (RBAC)
- Auth0 Management API integration
- Secure token validation
- User management through Auth0 dashboard

## API Documentation

The API is documented using Swagger/OpenAPI:

- Interactive API documentation
- Request/response examples
- Authentication flows
- Schema definitions
- API versioning support

## Project Structure

The solution consists of the following projects:

- `Soda.AppHost`: Orchestration project managing the distributed application
- `Soda.Api`: Web API project providing backend services

## Getting Started

1. Prerequisites:
    - .NET 9.0 SDK
    - Docker Desktop (for containerized services)
    - Auth0 account and configured application

2. Build and Run:
   ```bash
   dotnet build
   dotnet run --project Soda.AppHost
   ```

3. Access the application:
    - API endpoint: `http://localhost:<port>/api`
    - Swagger UI: `http://localhost:<port>/swagger`
    - Aspire dashboard: `http://localhost:<port>`

## Architecture

The application follows a distributed architecture pattern where services are orchestrated using .NET Aspire's
`DistributedApplication` model, enabling seamless local development and cloud deployment.