# Soda

## About the Project

Soda is a distributed application built using .NET Aspire, demonstrating modern cloud-native application development
practices with .NET 9.0.

## Technologies Used

- .NET 9.0
- ASP.NET Core
- .NET Aspire
- C# 13.0

## .NET Aspire Implementation

This project leverages .NET Aspire, a new stack for building cloud-native applications with .NET. Key features include:

- Distributed application orchestration
- Built-in service discovery
- Centralized logging and metrics
- Development-time container orchestration
- Cloud-ready configuration

## Project Structure

The solution consists of the following projects:

- `Soda.AppHost`: Orchestration project managing the distributed application
- `Soda.Api`: Web API project providing backend services

## Getting Started

1. Prerequisites:
    - .NET 9.0 SDK
    - Docker Desktop (for containerized services)

2. Build and Run:
   ```bash
   dotnet build
   dotnet run --project Soda.AppHost
   ```

3. Access the application:
    - API endpoint: `http://localhost:<port>/api`
    - Aspire dashboard: `http://localhost:<port>`

## Architecture

The application follows a distributed architecture pattern where services are orchestrated using .NET Aspire's
`DistributedApplication` model, enabling seamless local development and cloud deployment.