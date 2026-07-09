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

# Build the main API project. BuildConsoleUi=false: the console is built by the Node stage below,
# not by the .NET build (this SDK image has no Node).
WORKDIR "/src/BytLabs.MicroserviceTemplate.Api"
RUN dotnet build "./BytLabs.MicroserviceTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build /p:BuildConsoleUi=false

# Test stage to run all test projects
FROM build AS test
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Discover and run tests in all projects with 'Tests' in the name
RUN find . -type f -name "*.csproj" -path "*Tests*" -exec dotnet test {} -c $BUILD_CONFIGURATION --no-restore --results-directory /app/test-results --logger "trx;LogFileName=results.trx" /p:BuildConsoleUi=false \;

# Publish stage for the API
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BytLabs.MicroserviceTemplate.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false /p:BuildConsoleUi=false

# Build the bundled console (Vite client SPA -> ./dist). Its registry components are vendored under
# components/dynamic, so no registry server is needed at build time — just install + build.
FROM node:20 AS console
WORKDIR /console
COPY src/BytLabs.MicroserviceTemplate.Console/package*.json ./
RUN npm ci
COPY src/BytLabs.MicroserviceTemplate.Console/ ./
RUN npm run build

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Overlay the built console so the API serves it under /console.
COPY --from=console /console/dist /app/wwwroot/console
ENTRYPOINT ["dotnet", "BytLabs.MicroserviceTemplate.Api.dll"]
