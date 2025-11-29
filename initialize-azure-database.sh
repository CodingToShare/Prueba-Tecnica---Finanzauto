#!/bin/bash
# Script para inicializar la base de datos de Azure PostgreSQL con las migraciones y seed data

set -e

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}   Azure PostgreSQL Database Initialization${NC}"
echo -e "${BLUE}═══════════════════════════════════════════════════════════${NC}"
echo ""

# Verificar que estamos en el directorio correcto
if [ ! -f "Backend/ProductCatalog.Api/ProductCatalog.Api.csproj" ]; then
    echo -e "${RED}❌ Error: Debes ejecutar este script desde la raíz del repositorio${NC}"
    exit 1
fi

# Pedir información de conexión
echo -e "${YELLOW}Por favor ingresa la información de tu Azure PostgreSQL:${NC}"
echo ""

read -p "Servidor (ej: productcatalog-postgres.postgres.database.azure.com): " POSTGRES_HOST
read -p "Usuario admin (ej: adminuser): " POSTGRES_USER
read -sp "Contraseña: " POSTGRES_PASSWORD
echo ""
read -p "Nombre de la base de datos (ej: ProductCatalogDb): " POSTGRES_DB

# Construir connection string
CONNECTION_STRING="Host=${POSTGRES_HOST};Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD};SSL Mode=Require;Trust Server Certificate=true"

echo ""
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Paso 1: Verificar conexión a la base de datos${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

# Verificar conexión usando psql si está disponible
if command -v psql &> /dev/null; then
    echo "Probando conexión..."
    PGPASSWORD=$POSTGRES_PASSWORD psql -h "$POSTGRES_HOST" -U "$POSTGRES_USER" -d "$POSTGRES_DB" -c '\q' 2>/dev/null
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Conexión exitosa${NC}"
    else
        echo -e "${RED}❌ No se pudo conectar a la base de datos${NC}"
        echo "Verifica:"
        echo "  - Las credenciales son correctas"
        echo "  - El firewall de Azure PostgreSQL permite tu IP"
        echo "  - El servidor existe y está activo"
        exit 1
    fi
else
    echo -e "${YELLOW}⚠️  psql no disponible, saltando verificación de conexión${NC}"
fi

echo ""
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Paso 2: Instalar dotnet-ef tool${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

dotnet tool install --global dotnet-ef || dotnet tool update --global dotnet-ef
echo -e "${GREEN}✅ dotnet-ef instalado/actualizado${NC}"

echo ""
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Paso 3: Aplicar migraciones de Entity Framework${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

cd Backend

dotnet ef database update \
    --project ProductCatalog.Infrastructure \
    --startup-project ProductCatalog.Api \
    --connection "$CONNECTION_STRING" \
    --verbose

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Migraciones aplicadas exitosamente${NC}"
else
    echo -e "${RED}❌ Error aplicando migraciones${NC}"
    cd ..
    exit 1
fi

cd ..

echo ""
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Paso 4: Renombrar tablas y columnas a lowercase${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

if command -v psql &> /dev/null; then
    echo "Ejecutando script de renombre..."
    PGPASSWORD=$POSTGRES_PASSWORD psql \
        -h "$POSTGRES_HOST" \
        -U "$POSTGRES_USER" \
        -d "$POSTGRES_DB" \
        -f Backend/rename-tables-to-lowercase.sql \
        2>&1 | grep -v "already exists" | grep -v "does not exist" || true
    
    echo -e "${GREEN}✅ Tablas renombradas a lowercase${NC}"
else
    echo -e "${YELLOW}⚠️  psql no disponible${NC}"
    echo "Debes ejecutar manualmente el script: Backend/rename-tables-to-lowercase.sql"
fi

echo ""
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Paso 5: Aplicar seed data${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

if command -v psql &> /dev/null; then
    echo "Insertando datos iniciales..."
    PGPASSWORD=$POSTGRES_PASSWORD psql \
        -h "$POSTGRES_HOST" \
        -U "$POSTGRES_USER" \
        -d "$POSTGRES_DB" \
        -f Backend/seed-data-lowercase.sql \
        2>&1 | grep -v "already exists" | grep -v "duplicate key" || true
    
    echo -e "${GREEN}✅ Seed data aplicado${NC}"
else
    echo -e "${YELLOW}⚠️  psql no disponible${NC}"
    echo "Debes ejecutar manualmente el script: Backend/seed-data-lowercase.sql"
fi

echo ""
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"
echo -e "${YELLOW}Paso 6: Verificar instalación${NC}"
echo -e "${BLUE}━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━${NC}"

if command -v psql &> /dev/null; then
    echo ""
    echo "Conteo de registros por tabla:"
    echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
    PGPASSWORD=$POSTGRES_PASSWORD psql \
        -h "$POSTGRES_HOST" \
        -U "$POSTGRES_USER" \
        -d "$POSTGRES_DB" \
        -c "
        SELECT 
            'categories' as tabla, COUNT(*) as registros FROM categories
        UNION ALL
        SELECT 'products', COUNT(*) FROM products
        UNION ALL
        SELECT 'suppliers', COUNT(*) FROM suppliers
        UNION ALL
        SELECT 'shippers', COUNT(*) FROM shippers
        UNION ALL
        SELECT 'users', COUNT(*) FROM users
        ORDER BY tabla;
        "
fi

echo ""
echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo -e "${GREEN}   ✅ ¡Inicialización completada exitosamente!${NC}"
echo -e "${GREEN}═══════════════════════════════════════════════════════════${NC}"
echo ""
echo "Próximos pasos:"
echo "1. Configura los secrets en GitHub (ver AZURE_SETUP_GITHUB_SECRETS.md)"
echo "2. Haz push de tus cambios para disparar el deployment"
echo "3. El App Service usará la base de datos que acabas de inicializar"
echo ""
echo "Credenciales de prueba:"
echo "  Admin: admin / Admin123!"
echo "  User:  user  / User123!"
echo ""
