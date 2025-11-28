#!/bin/sh
set -e

echo "🔄 Waiting for database to be ready..."

# Wait for PostgreSQL to be ready
until PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -c '\q' 2>/dev/null; do
  echo "⏳ PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "✅ PostgreSQL is ready!"

# Execute seed data script
echo "🌱 Applying seed data..."
PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -f /app/seed-data.sql || echo "⚠️  Seed data may already exist"

echo "🚀 Starting API..."
exec dotnet ProductCatalog.Api.dll
