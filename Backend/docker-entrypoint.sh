#!/bin/sh
set -e

echo "🔄 Waiting for database to be ready..."

# Wait for PostgreSQL to be ready
until PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -c '\q' 2>/dev/null; do
  echo "⏳ PostgreSQL is unavailable - sleeping"
  sleep 2
done

echo "✅ PostgreSQL is ready!"

# Start API in background
echo "🚀 Starting API..."
cd /app
dotnet ProductCatalog.Api.dll &
API_PID=$!

# Wait for API to start and apply migrations
echo "⏳ Waiting for API to start and apply migrations..."
sleep 10

# Execute seed data script
echo "🌱 Applying seed data..."
PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -f /app/seed-data.sql || echo "⚠️  Seed data may already exist"

echo "✅ Setup complete! API is running."

# Keep API running in foreground
wait $API_PID
