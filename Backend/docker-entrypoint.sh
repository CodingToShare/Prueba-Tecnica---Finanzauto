#!/bin/sh
set -e

echo "🔄 Waiting for database to be ready..."

# Wait for PostgreSQL to be ready
until PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -c '\q' 2>/dev/null; do
  echo "⏳ PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "✅ PostgreSQL is ready!"

# Apply EF Core migrations
echo "🔄 Applying database migrations..."
export ConnectionStrings__DefaultConnection="Host=db;Port=5432;Database=ProductCatalogDb;Username=postgres;Password=postgres"
dotnet ef database update --project /src/ProductCatalog.Infrastructure/ProductCatalog.Infrastructure.csproj --startup-project /src/ProductCatalog.Api/ProductCatalog.Api.csproj --no-build || echo "⚠️  Migrations may already be applied"

# Execute seed data script
echo "🌱 Applying seed data..."
PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -f /app/seed-data.sql || echo "⚠️  Seed data may already exist"

echo "🚀 Starting API..."
cd /app
exec dotnet ProductCatalog.Api.dll
