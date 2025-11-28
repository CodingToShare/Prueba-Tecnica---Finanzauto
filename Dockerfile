# Multi-stage build para Product Catalog API

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files from Backend directory
# Build context is repo root, so we copy from Backend/
COPY Backend/ProductCatalog.sln ./
COPY Backend/ProductCatalog.Api/ ./ProductCatalog.Api/
COPY Backend/ProductCatalog.Application/ ./ProductCatalog.Application/
COPY Backend/ProductCatalog.Domain/ ./ProductCatalog.Domain/
COPY Backend/ProductCatalog.Infrastructure/ ./ProductCatalog.Infrastructure/
COPY Backend/ProductCatalog.Tests/ ./ProductCatalog.Tests/

# Restore dependencies
RUN dotnet restore "ProductCatalog.sln"

# Build the application
RUN dotnet build "ProductCatalog.sln" -c Release -o /app/build

# Publish
RUN dotnet publish "ProductCatalog.Api/ProductCatalog.Api.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published application from build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "ProductCatalog.Api.dll"]
