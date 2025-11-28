# Multi-stage build para Product Catalog API

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files from Backend directory
# Build context is repo root, so we copy from Backend/
COPY Backend/ProductCatalog.Api/*.csproj ./ProductCatalog.Api/
COPY Backend/ProductCatalog.Application/*.csproj ./ProductCatalog.Application/
COPY Backend/ProductCatalog.Domain/*.csproj ./ProductCatalog.Domain/
COPY Backend/ProductCatalog.Infrastructure/*.csproj ./ProductCatalog.Infrastructure/

# Restore dependencies (just for API project, will restore dependencies)
RUN dotnet restore "ProductCatalog.Api/ProductCatalog.Api.csproj"

# Copy remaining source code
COPY Backend/ProductCatalog.Api/ ./ProductCatalog.Api/
COPY Backend/ProductCatalog.Application/ ./ProductCatalog.Application/
COPY Backend/ProductCatalog.Domain/ ./ProductCatalog.Domain/
COPY Backend/ProductCatalog.Infrastructure/ ./ProductCatalog.Infrastructure/

# Build the application
RUN dotnet build "ProductCatalog.Api/ProductCatalog.Api.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "ProductCatalog.Api/ProductCatalog.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app

# Install curl and postgresql-client for health checks and migrations
RUN apt-get update && apt-get install -y curl postgresql-client && rm -rf /var/lib/apt/lists/*

# Install dotnet-ef tool globally
RUN dotnet tool install --global dotnet-ef --version 10.0.0
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy project files needed for migrations to /src directory
COPY --from=build /app/ProductCatalog.Infrastructure /src/ProductCatalog.Infrastructure
COPY --from=build /app/ProductCatalog.Api /src/ProductCatalog.Api
COPY --from=build /app/ProductCatalog.Application /src/ProductCatalog.Application
COPY --from=build /app/ProductCatalog.Domain /src/ProductCatalog.Domain

# Copy published application to /app
COPY --from=build /app/publish .

# Copy entrypoint script and seed data
COPY Backend/docker-entrypoint.sh /app/docker-entrypoint.sh
COPY Backend/seed-data.sql /app/seed-data.sql

# Convert line endings to Unix format and make executable
RUN sed -i 's/\r$//' /app/docker-entrypoint.sh && chmod +x /app/docker-entrypoint.sh

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Entry point
ENTRYPOINT ["/app/docker-entrypoint.sh"]
