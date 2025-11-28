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

# Wait for API to apply migrations (check if any table exists)
echo "⏳ Waiting for migrations to be applied..."
for i in $(seq 1 30); do
  TABLE_COUNT=$(PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';" 2>/dev/null | tr -d ' ')
  if [ "$TABLE_COUNT" -gt "0" ]; then
    echo "✅ Migrations applied successfully! Found $TABLE_COUNT tables."
    break
  fi
  echo "⏳ Waiting for migrations... ($i/30)"
  sleep 2
done

# Verify tables exist before seeding
TABLE_COUNT=$(PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -t -c "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';" 2>/dev/null | tr -d ' ')
if [ "$TABLE_COUNT" -eq "0" ]; then
  echo "❌ ERROR: Migrations failed to apply. No tables found in database."
  echo "Check API logs for migration errors."
  wait $API_PID
  exit 1
fi

# Execute seed data script
echo "🌱 Applying seed data..."
PGPASSWORD=postgres psql -h "db" -U "postgres" -d "ProductCatalogDb" -f /app/seed-data.sql 2>&1 | grep -v "already exists" | grep -v "does not exist" || true

echo "✅ Setup complete! API is running with $TABLE_COUNT tables and seed data."

# Keep API running in foreground
wait $API_PID
