# DeveloperStore API

Sales API built with .NET 8, layered architecture, DDD principles, and RabbitMQ-based event publishing.

## Recommended Reading Order

For faster onboarding, this order is the most effective:
1. Architecture and definitions
2. How to run
3. Access addresses and credentials
4. Endpoints and request/response examples

This flow helps the reader understand the system first, then execute it with fewer mistakes.

## Architecture and Definitions

### Layers
- `WebApi`: HTTP contracts, controllers, middleware.
- `Application`: use cases, handlers, application services, validation.
- `Domain`: entities, business rules, repository contracts.
- `ORM`: persistence implementation (EF Core, mappings, migrations).
- `IoC`: dependency registration/composition root.

### Main communication flow
`API Controller -> MediatR Command/Query -> Handler -> Application Service -> Domain -> Repository (ORM/EF Core) -> PostgreSQL`

### Event flow
`Application Service -> ISalesEventPublisher -> Rebus -> RabbitMQ queue (developerstore.sales.events)`

### Patterns used
- DDD
- CQRS with MediatR
- Repository Pattern
- Dependency Injection
- Global exception/validation middleware

## Technology Stack

### Language and platform
- C# 12
- .NET 8 (`net8.0`)
- ASP.NET Core Web API

### Data and messaging
- PostgreSQL 13
- RabbitMQ 3 (`rabbitmq:3-management`)

### ORM
- Entity Framework Core 8
- Npgsql provider (`Npgsql.EntityFrameworkCore.PostgreSQL`)

### Main libraries
- MediatR
- AutoMapper
- FluentValidation
- Swashbuckle (Swagger/OpenAPI)
- Rebus + Rebus.RabbitMq
- AspNetCore.HealthChecks.Rabbitmq

### Tests and quality
- xUnit
- FluentAssertions
- NSubstitute
- Bogus
- Testcontainers.PostgreSql

Test suites:
- Unit tests (`tests/Ambev.DeveloperEvaluation.Unit`)
- Integration tests (`tests/Ambev.DeveloperEvaluation.Integration`)
- Functional tests (`tests/Ambev.DeveloperEvaluation.Functional`)

## Running the Project

### Run with Docker

Prerequisite: Docker Desktop running.

Start:
```bash
docker compose up --build -d
```

Stop:
```bash
docker compose down
```

### Run with IDE (Rider / Visual Studio)

1. Open `Ambev.DeveloperEvaluation.sln`.
2. Set `src/Ambev.DeveloperEvaluation.WebApi/Ambev.DeveloperEvaluation.WebApi.csproj` as startup project.
3. Select `https` (or `http`) profile in `Properties/launchSettings.json`.
4. Run with Play or Debug.

Startup behavior:
- Swagger opens automatically (`launchBrowser=true`, `launchUrl=swagger`).
- `https` profile: `https://localhost:8081/swagger` and `http://localhost:8080/swagger`
- `http` profile: `http://localhost:5119/swagger`

### Required local configuration for IDE run

RabbitMQ and PostgreSQL must be up and correctly configured in:
`src/Ambev.DeveloperEvaluation.WebApi/appsettings.json`

Current project config:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n;"
  },
  "RabbitMQ": {
    "ConnectionString": "amqp://guest:guest@127.0.0.1:5672",
    "SalesEventsQueue": "developerstore.sales.events"
  }
}
```

## Addresses and Credentials

After `docker compose up`:
- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- RabbitMQ UI: `http://localhost:15672` (`user: guest`, `password: guest`)

PostgreSQL (container):
- host: `localhost`
- port: `5432`
- database: `developer_evaluation`
- user: `developer`
- password: `ev@luAt10n`

## Exposed Endpoints

Base route: `/api/sales`

1. `GET /api/sales` (paginated listing)
2. `GET /api/sales/{id}` (get by id)
3. `POST /api/sales` (create)
4. `PUT /api/sales/{id}` (update)
5. `DELETE /api/sales/{id}` (logical delete)

## Request and Response Examples

You can test all requests using the Postman collection available in the repository:
- `Sale Postman Collection.json`

### Create sale
`POST /api/sales`
```json
{
  "saleNumber": "SALE-001",
  "saleDate": "2026-05-10T12:00:00Z",
  "customerId": "1f8f2c70-1111-4a60-8f2f-bb6c2f0a4c22",
  "customerName": "Andre Customer",
  "branchId": "2f8f2c70-2222-4a60-8f2f-bb6c2f0a4c22",
  "branchName": "Main Branch",
  "items": [
    {
      "productId": "3f8f2c70-3333-4a60-8f2f-bb6c2f0a4c22",
      "productDescription": "Product A",
      "quantity": 4,
      "unitPrice": 100.00
    }
  ]
}
```

### Update sale
`PUT /api/sales/{id}`
```json
{
  "saleNumber": "SALE-001-UPDATED",
  "saleDate": "2026-05-10T13:00:00Z",
  "customerId": "1f8f2c70-1111-4a60-8f2f-bb6c2f0a4c22",
  "customerName": "Andre Customer Updated",
  "branchId": "2f8f2c70-2222-4a60-8f2f-bb6c2f0a4c22",
  "branchName": "Main Branch",
  "active": true,
  "items": [
    {
      "id": "11111111-1111-1111-1111-111111111111",
      "productId": "3f8f2c70-3333-4a60-8f2f-bb6c2f0a4c22",
      "productDescription": "Product A Updated",
      "quantity": 5,
      "unitPrice": 100.00,
      "active": true
    }
  ]
}
```

### List sales with paging/filter
`GET /api/sales?_page=1&_size=10&_order=saleDate desc&customerName=Andre`

### Success response example
```json
{
  "success": true,
  "message": "Sale created successfully",
  "data": {
    "id": "a6a9f4d9-87fc-4a96-9462-86edb4b36850",
    "saleNumber": "SALE-001",
    "saleDate": "2026-05-10T12:00:00Z",
    "customerId": "1f8f2c70-1111-4a60-8f2f-bb6c2f0a4c22",
    "customerName": "Andre Customer",
    "branchId": "2f8f2c70-2222-4a60-8f2f-bb6c2f0a4c22",
    "branchName": "Main Branch",
    "totalSaleAmount": 380.00,
    "active": true,
    "items": []
  }
}
```

### Validation failure example (400)
```json
{
  "success": false,
  "message": "Validation Failed",
  "errors": [
    {
      "error": "SaleNumber",
      "detail": "'Sale Number' must not be empty."
    }
  ]
}
```

### Domain rule violation example (400)
```json
{
  "type": "DomainRuleViolation",
  "error": "Sale domain rule violated",
  "detail": "Quantity cannot be greater than 20."
}
```

### Not found example (404)
```json
{
  "type": "ResourceNotFound",
  "error": "Sale not found",
  "detail": "The sale with ID 00000000-0000-0000-0000-000000000001 does not exist."
}
```
