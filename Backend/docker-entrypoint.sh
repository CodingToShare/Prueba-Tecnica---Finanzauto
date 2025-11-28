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

# Wait for API to apply migrations (check if Users table exists)
echo "⏳ Waiting for migrations to be applied..."
for i in $(seq 1 30); do
  if PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -c '\d "Users"' >/dev/null 2>&1; then
    echo "✅ Migrations applied successfully!"
    break
  fi
  echo "⏳ Waiting for migrations... ($i/30)"
  sleep 2
done

# Verify table exists before seeding
if ! PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -c '\d "Users"' >/dev/null 2>&1; then
  echo "❌ ERROR: Migrations failed to apply. Users table does not exist."
  echo "Check API logs for migration errors."
  wait $API_PID
  exit 1
fi

# Execute seed data script
echo "🌱 Applying seed data..."
PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -f /app/seed-data.sql 2>&1 | grep -v "ERROR" || true

echo "✅ Setup complete! API is running."

# Keep API running in foreground
wait $API_PID
