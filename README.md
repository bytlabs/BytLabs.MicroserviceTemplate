# BytLabs Microservice Template

A modern .NET microservice template with GraphQL, MongoDB, Docker support, and comprehensive testing infrastructure.

## Features

- 🚀 GraphQL API using HotChocolate
- 📦 MongoDB Integration
- 🐳 BytLabs Core Packages Integration
- 🐳 Docker & Docker Compose support
- 🧪 Comprehensive testing setup (Unit & Acceptance Tests)
- 📊 OpenTelemetry integration
- 🔄 CI/CD pipeline with GitHub Actions
- 🎯 Domain-Driven Design (DDD) architecture
- 🔍 Health checks
- 👥 Multi-tenant support
- 📝 Logging with Serilog

## BytLabs Package Integration

This template leverages BytLabs [core packages](https://github.com/bytlabs/BytLabs.BackendPackages) to ensure consistency and standardization across all microservices:

| Package | Description |
|---------|------------|
| [`BytLabs.Domain`](https://github.com/bytlabs/BytLabs.BackendPackages) | Domain model building blocks |
| [`BytLabs.Application`](https://github.com/bytlabs/BytLabs.BackendPackages) | CQRS, validation, and application services |
| [`BytLabs.DataAccess`](https://github.com/bytlabs/BytLabs.BackendPackages) | Data persistence and MongoDB integration |
| [`BytLabs.DataAccess.MongoDB`](https://github.com/bytlabs/BytLabs.BackendPackages) | Data persistence and MongoDB integration |
| [`BytLabs.Multitenancy`](https://github.com/bytlabs/BytLabs.BackendPackages) | Multi-tenant infrastructure |
| [`BytLabs.Observability`](https://github.com/bytlabs/BytLabs.BackendPackages) | Monitoring and logging tools |

Using these packages ensures:
- Uniform architecture across all microservices
- Consistent error handling and logging
- Standardized testing approaches
- Common security practices
- Shared domain patterns and building blocks

## Prerequisites

- .NET 8.0 SDK
- Docker & Docker Compose
- MongoDB (or use the provided Docker container)

## Getting Started

To get started with the project, follow these steps:

1. **Create a New Repository from the Template**

   Instead of cloning the repository directly, use the "Use this template" button on GitHub to create your own repository based on the **microservice-template**. This ensures that you have a copy of the template repository to work with.

   - Click the **"Use this template"** button (usually found near the top-right corner of the page).
   - Create your new repository by following the instructions on GitHub.

2. **Clone Your New Repository**

   Once your new repository is created, clone it to your local machine:

   ```bash
   git clone https://github.com/your-username/your-new-repository.git
   cd your-new-repository
   ```

3. **Run the Project Setup Script**

   To rename the project, run the `set-project-name.sh` script. It will prompt you for a new project name:

   ```bash
   bash set-project-name.sh
   ```

   **Example Input:**
   When prompted, enter the new project name. For example:
   - `Order Management`
   - `Orders`

   The script will automatically update the project name in all relevant files.

4. **Run the application using Docker Compose**

   ```bash
   docker-compose up
   ```

   The service will be available at:
   - HTTP: http://localhost:8080
   - HTTPS: https://localhost:8081
   - GraphQL Endpoint: http://localhost:8080/graphql/


## Tutorial

This blog series will guide you step by step, from setting up the template to deploying your services in the cloud.

1. **[Getting Started with BytLabs.MicroserviceTemplate: API Project Setup](https://bytlabs.co/blog/getting-started-with-bytlabsmicroservicetemplate-api-project-setup)**  
2. **[Getting Started with BytLabs.MicroserviceTemplate: Domain and Application Setup](https://bytlabs.co/blog/getting-started-with-bytlabsmicroservicetemplate-domain-and-application-setup)**  


## Development

### Configuration

The application can be configured through various settings files:

1. Main Configuration (`appsettings.json`):

   ```json
   {
     "ObservabilityConfiguration": {
       "CollectorUrl": "http://localhost:4317",
       "ServiceName": "microservice-template",
       "Timeout": 1000
     },
     "MongoDatabaseConfiguration": {
       "DatabaseName": "microserviceTemplate",
       "ConnectionString": "mongodb://localhost:27017?retryWrites=false",
       "UseTransactions": false
     }
   }
   ```

### Running Tests

Tests are plain **xUnit** (no SpecFlow/Gherkin). Acceptance tests boot the API in-process with
`WebApplicationFactory` and drive it through the generated StrawberryShake client, so they need a
running MongoDB.

Unit tests (no infrastructure required):

```bash
dotnet test src/BytLabs.MicroserviceTemplate.Tests.Unit
```

Acceptance tests (start MongoDB first):

```bash
docker-compose up -d bytlabs-mongo
dotnet test src/BytLabs.MicroserviceTemplate.Tests.Accpetance
```

Or run the full suite in Docker:

```bash
docker-compose up bytlabs-microservice-template-tests
```

### GraphQL Client Generation

The template uses StrawberryShake for GraphQL client generation. To update the client:

1. Install the StrawberryShake CLI:

   ```bash
   dotnet tool restore
   ```

2. Generate the client code:

   ```bash
   dotnet graphql generate
   ```

## CI/CD Pipeline

The template includes a GitHub Actions workflow that:

1. Runs all tests
2. Builds the application
3. Publishes NuGet packages
4. Supports different versioning for:
   - Feature branches (alpha versions)
   - Release tags (stable versions)

## Architecture

The solution follows Clean Architecture and DDD principles:

- **API Layer**: GraphQL endpoints, configuration, and middleware
- **Application Layer**: Commands, queries, and application logic
- **Domain Layer**: Aggregates, entities, and domain logic
- **Infrastructure Layer**: Database access, external services integration

## Recipes

This template doubles as a **recipe catalog**: `Order` is the minimal example and `Product`
demonstrates the advanced patterns. Each recipe is documented with links to the real sample code.

See the full index: [docs/recipes/README.md](docs/recipes/README.md).

Highlights:

- Domain: [aggregate root & events](docs/recipes/aggregate-root.md) ·
  [sub-entity](docs/recipes/sub-entity.md) ·
  [dynamic data](docs/recipes/dynamic-data.md) ·
  [soft delete](docs/recipes/soft-delete.md) ·
  [schema value objects](docs/recipes/schema-value-objects.md)
- Application: [command + handler](docs/recipes/cqrs-command-handler.md) ·
  [sub-entity commands](docs/recipes/sub-entity-commands.md) ·
  [DTOs](docs/recipes/dtos.md) ·
  [AutoMapper](docs/recipes/automapper-profiles.md) ·
  [domain event handler](docs/recipes/domain-event-handler.md)
- API: [GraphQL query](docs/recipes/graphql-query.md) ·
  [dynamic-data query](docs/recipes/graphql-dynamic-data-query.md) ·
  [mutation](docs/recipes/graphql-mutation.md) ·
  [authorization](docs/recipes/authorization.md) ·
  [type registration](docs/recipes/graphql-type-registration.md)
- Infrastructure: [service registration](docs/recipes/service-registration.md) ·
  [BSON class maps](docs/recipes/bson-class-maps.md) ·
  [custom serializer](docs/recipes/custom-bson-serializer.md)
- Cross-cutting: [multitenancy](docs/recipes/multitenancy.md)
- Testing: [unit](docs/recipes/unit-testing.md) ·
  [acceptance (xUnit)](docs/recipes/acceptance-testing-xunit.md) ·
  [typed client](docs/recipes/strawberry-shake-client.md)

## Roadmap

We are planning to introduce the following enhancements to the BytLabs Microservice Template:

1. **BytLabs MessageBus Package Integration**:
   - Leverages Dapr's message bus to handle both event-based pub/sub and command-driven point-to-point communication.
   - Facilitates seamless communication for integration events and commands between microservices.
   - Supports the outbox pattern to ensure reliable message delivery and consistency.

2. **BytLabs FileSystem Service**:
   - The FileSystem Service will provide an abstraction for managing object storage across multiple cloud providers (e.g., AWS S3, Azure Blob Storage, Google Cloud Storage).
   - It will be implemented as a standalone microservice, built using the BytLabs Microservice Template, ensuring consistency in architecture and design principles.
    - **Key Features:**
      - **Multi-Cloud Compatibility**: The service will support major cloud providers, enabling seamless storage operations regardless of the underlying cloud infrastructure.
      - **Unified API**: Developers can interact with a single API, abstracting the complexities of different cloud provider implementations.
      - **Core Operations**: Includes file upload, download, metadata retrieval, and secure file sharing.

This service aims to simplify object storage management and provide a consistent developer experience across varied cloud environments.

Stay tuned for updates as we continue enhancing the BytLabs ecosystem!

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [HotChocolate](https://chillicream.com/docs/hotchocolate) for GraphQL implementation
- [MongoDB.Driver](https://www.mongodb.com/docs/drivers/csharp/) for MongoDB integration
- [OpenTelemetry](https://opentelemetry.io/) for observability
- [Docker](https://www.docker.com/) for containerization

