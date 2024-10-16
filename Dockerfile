# Base image for the runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image with SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

# Copy and restore main API project
COPY ["src/BytLabs.MicroserviceTemplate.Api/BytLabs.MicroserviceTemplate.Api.csproj", "src/BytLabs.MicroserviceTemplate.Api/"]
RUN dotnet restore "./src/BytLabs.MicroserviceTemplate.Api/BytLabs.MicroserviceTemplate.Api.csproj"

# Copy all source files
COPY . .

# Build the main API project
WORKDIR "/src/BytLabs.MicroserviceTemplate.Api"
RUN dotnet build "./BytLabs.MicroserviceTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Test stage to run all test projects
FROM build AS test
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Discover and run tests in all projects with 'Tests' in the name
RUN find . -type f -name "*.csproj" -path "*Tests*" -exec dotnet test {} -c $BUILD_CONFIGURATION --no-restore --results-directory /app/test-results --logger "trx;LogFileName=results.trx" \;

# Publish stage for the API
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BytLabs.MicroserviceTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BytLabs.MicroserviceTemplate.Api.dll"]
