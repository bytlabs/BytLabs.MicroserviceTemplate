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

Learn how to build scalable and maintainable microservices using BytLabs.MicroserviceTemplate through our comprehensive blog series. Each post guides you step by step, from setting up the template to deploying your services in the cloud.

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

Execute all tests using Docker:

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

## Roadmap

We are planning to introduce the following enhancements to the BytLabs Microservice Template:

1. **BytLabs MessageBus Package Integration**:
   - Leverages Dapr's message bus to handle both event-based pub/sub and command-driven point-to-point communication.
   - Facilitates seamless communication for integration events and commands between microservices.
   - Supports the outbox pattern to ensure reliable message delivery and consistency.

2. **Grafana and Prometheus Integration**:
   - Add Docker Compose setup for Grafana and Prometheus to enable monitoring and visualization.

3. **Kubernetes Templates**:
   - Provide Kubernetes manifests for observability, including configurations for metrics and distributed tracing.

4. **REST API and gRPC Interface Examples**:
   - Include examples demonstrating REST API and gRPC interfaces within the template.

5. **BytLabs IdentityService using Keycloak**:
   - Integration with an IdentityService implementation using Keycloak, designed to work seamlessly with this microservice template.

6. **BytLabs IdentityService using IdentityServer**:
   - Integration with an IdentityService implementation using IdentityServer, designed to work seamlessly with this microservice template.

7. **BytLabs Gateway Service**:
   - Integration with a new gateway service using Ocelot, supporting interfaces defined in the microservice template.

8. **BytLabs Aggregator Service**:
   - Integration with a new aggregator service that consolidates data from multiple microservices built with this template.

9. **FileSystem Service**:
   - Integration with a new FileSystem service managing object storage across all cloud providers, implemented as a separate microservice using this template.

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

