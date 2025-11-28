#!/bin/bash

# Product Catalog API - Azure Setup Script
# Automatiza la creación de recursos en Azure para el backend y frontend

set -e

# Colors for output
GREEN='\033[0;32m'
BLUE='\033[0;34m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}=== Product Catalog API - Azure Setup ===${NC}\n"

# Variables
RESOURCE_GROUP="rg-productcatalog"
LOCATION="eastus2"
POSTGRES_LOCATION="canadacentral"  # PostgreSQL Flexible Server requiere región específica
RANDOM_SUFFIX=$(date +%s | tail -c 6)
POSTGRES_SERVER_NAME="productcatalog-db-${RANDOM_SUFFIX}"
POSTGRES_DB_NAME="ProductCatalogDb"
POSTGRES_ADMIN_USER="adminuser"
BACKEND_APP_SERVICE="app-productcatalog-api"
FRONTEND_APP_SERVICE="app-productcatalog-web"
APP_SERVICE_PLAN_BACKEND="plan-productcatalog-api"
APP_SERVICE_PLAN_FRONTEND="plan-productcatalog-web"
ACR_NAME="acr$(date +%s | tail -c 6)"
KEY_VAULT_NAME="kvpc$(date +%s | tail -c 6)"

# Read from user or use defaults
read -p "Ingresa el nombre del resource group (default: $RESOURCE_GROUP): " input
RESOURCE_GROUP=${input:-$RESOURCE_GROUP}

read -p "Ingresa la región de Azure para recursos generales (default: eastus2): " input
LOCATION=${input:-eastus2}

read -p "Ingresa la región para PostgreSQL Flexible Server (default: canadacentral): " input
POSTGRES_LOCATION=${input:-canadacentral}

read -p "Ingresa la contraseña PostgreSQL (min 12 chars, mayús, minús, números, símbolos): " POSTGRES_ADMIN_PASSWORD
if [ ${#POSTGRES_ADMIN_PASSWORD} -lt 12 ]; then
    echo -e "${RED}❌ La contraseña debe tener al menos 12 caracteres${NC}"
    exit 1
fi

read -p "¿Deseas crear Frontend App Service? (s/n, default: s): " CREATE_FRONTEND
CREATE_FRONTEND=${CREATE_FRONTEND:-s}

echo -e "\n${BLUE}Autenticando en Azure...${NC}"
az login

SUBSCRIPTION_ID=$(az account show --query id -o tsv)
echo -e "${GREEN}✅ Suscripción: $SUBSCRIPTION_ID${NC}\n"

# 1. Create Resource Group
echo -e "${BLUE}1. Creando Resource Group: $RESOURCE_GROUP${NC}"
az group create \
  --name "$RESOURCE_GROUP" \
  --location "$LOCATION" \
  --output none

echo -e "${GREEN}✅ Resource Group creado${NC}"

# 2. Create Key Vault for Secrets
echo -e "\n${BLUE}2. Creando Key Vault para secretos${NC}"
az keyvault create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$KEY_VAULT_NAME" \
  --location "$LOCATION" \
  --enabled-for-deployment true \
  --enabled-for-disk-encryption false \
  --enabled-for-template-deployment true \
  --enable-rbac-authorization false \
  --output none

echo -e "${GREEN}✅ Key Vault creado${NC}"

# Get current user's object ID for access policy
CURRENT_USER_ID=$(az ad signed-in-user show --query id -o tsv)

echo "   → Configurando acceso al Key Vault para usuario actual..."
az keyvault set-policy \
  --name "$KEY_VAULT_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --object-id "$CURRENT_USER_ID" \
  --secret-permissions get list set delete \
  --key-permissions get list create delete \
  --output none

echo -e "${GREEN}✅ Permisos configurados${NC}"

# 3. Store secrets in Key Vault
echo -e "\n${BLUE}3. Guardando secretos en Key Vault${NC}"
az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "PostgresAdminPassword" \
  --value "$POSTGRES_ADMIN_PASSWORD" \
  --output none

echo -e "${GREEN}✅ Secretos guardados${NC}"

# 4. Create PostgreSQL Server
echo -e "\n${BLUE}4. Creando PostgreSQL Server${NC}"

echo "   → PostgreSQL Server: $POSTGRES_SERVER_NAME..."
if ! az postgres flexible-server create \
  --name "$POSTGRES_SERVER_NAME" \
  --resource-group "$RESOURCE_GROUP" \
  --location "$POSTGRES_LOCATION" \
  --admin-user "$POSTGRES_ADMIN_USER" \
  --admin-password "$POSTGRES_ADMIN_PASSWORD" \
  --sku-name "Standard_B1ms" \
  --tier "Burstable" \
  --storage-size 32 \
  --version 14 \
  --yes \
  --output json > /dev/null; then
    echo -e "${RED}❌ Error creando PostgreSQL Server. Verifica:${NC}"
    echo -e "   - Contraseña: debe tener mayús, minús, números, símbolos"
    echo -e "   - El nombre del servidor debe ser único globalmente"
    echo -e "   - La región '$POSTGRES_LOCATION' está disponible para PostgreSQL Flexible Server"
    echo -e "   - Cuota de recursos disponible"
    exit 1
fi
echo -e "${GREEN}✅ PostgreSQL Server creado${NC}"

echo "   → Configurando Firewall (permitir servicios de Azure)..."
az postgres flexible-server firewall-rule create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$POSTGRES_SERVER_NAME" \
  --rule-name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 255.255.255.255 \
  --output none

echo -e "${GREEN}✅ Firewall configurado${NC}"

echo "   → Base de Datos: $POSTGRES_DB_NAME..."
az postgres flexible-server db create \
  --resource-group "$RESOURCE_GROUP" \
  --server-name "$POSTGRES_SERVER_NAME" \
  --database-name "$POSTGRES_DB_NAME" \
  --charset UTF8 \
  --collation en_US.utf8 \
  --output none

POSTGRES_FQDN="${POSTGRES_SERVER_NAME}.postgres.database.azure.com"

# 5. Create Container Registry (for Docker images)
echo -e "\n${BLUE}5. Creando Azure Container Registry${NC}"
az acr create \
  --resource-group "$RESOURCE_GROUP" \
  --name "$ACR_NAME" \
  --sku Basic \
  --admin-enabled true \
  --output none

echo -e "${GREEN}✅ Container Registry creado${NC}"

ACR_URL="${ACR_NAME}.azurecr.io"
ACR_USERNAME=$(az acr credential show --name "$ACR_NAME" --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name "$ACR_NAME" --query "passwords[0].value" -o tsv)

# 6. Create App Service Plans
echo -e "\n${BLUE}6. Creando App Service Plans${NC}"

echo "   → Plan para Backend (Standard S1)..."
az appservice plan create \
  --name "$APP_SERVICE_PLAN_BACKEND" \
  --resource-group "$RESOURCE_GROUP" \
  --sku S1 \
  --is-linux \
  --output none

echo -e "${GREEN}✅ Backend App Service Plan creado${NC}"

if [ "$CREATE_FRONTEND" == "s" ] || [ "$CREATE_FRONTEND" == "S" ]; then
    echo "   → Plan para Frontend (Standard S1)..."
    az appservice plan create \
      --name "$APP_SERVICE_PLAN_FRONTEND" \
      --resource-group "$RESOURCE_GROUP" \
      --sku S1 \
      --is-linux \
      --output none

    echo -e "${GREEN}✅ Frontend App Service Plan creado${NC}"
fi

# 7. Create Backend App Service
echo -e "\n${BLUE}7. Creando Backend App Service${NC}"
az webapp create \
  --name "$BACKEND_APP_SERVICE" \
  --resource-group "$RESOURCE_GROUP" \
  --plan "$APP_SERVICE_PLAN_BACKEND" \
  --runtime "DOTNETCORE:8.0" \
  --output none

echo -e "${GREEN}✅ Backend App Service creado${NC}"
BACKEND_URL="https://${BACKEND_APP_SERVICE}.azurewebsites.net"

# 8. Create Frontend App Service (optional)
if [ "$CREATE_FRONTEND" == "s" ] || [ "$CREATE_FRONTEND" == "S" ]; then
    echo -e "\n${BLUE}8. Creando Frontend App Service${NC}"
    az webapp create \
      --name "$FRONTEND_APP_SERVICE" \
      --resource-group "$RESOURCE_GROUP" \
      --plan "$APP_SERVICE_PLAN_FRONTEND" \
      --runtime "NODE:20-lts" \
      --output none

    echo -e "${GREEN}✅ Frontend App Service creado${NC}"
    FRONTEND_URL="https://${FRONTEND_APP_SERVICE}.azurewebsites.net"
else
    FRONTEND_URL="N/A"
fi

# 9. Configure Backend App Settings
echo -e "\n${BLUE}9. Configurando App Settings del Backend${NC}"

CONNECTION_STRING="Host=${POSTGRES_FQDN};Port=5432;Database=${POSTGRES_DB_NAME};Username=${POSTGRES_ADMIN_USER}@${POSTGRES_SERVER_NAME};Password=${POSTGRES_ADMIN_PASSWORD};SSL Mode=Require;"

az webapp config appsettings set \
  --name "$BACKEND_APP_SERVICE" \
  --resource-group "$RESOURCE_GROUP" \
  --settings \
    ConnectionStrings__DefaultConnection="$CONNECTION_STRING" \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS="http://+:80" \
    Cors__AllowedOrigins="${FRONTEND_URL}" \
  --output none

echo -e "${GREEN}✅ App Settings configurados${NC}"

# 10. Configure Container Registry Access
echo -e "\n${BLUE}10. Configurando acceso a Container Registry${NC}"
az webapp config container set \
  --name "$BACKEND_APP_SERVICE" \
  --resource-group "$RESOURCE_GROUP" \
  --docker-custom-image-name "$ACR_URL/productcatalog-api:latest" \
  --docker-registry-server-url "https://$ACR_URL" \
  --docker-registry-server-user "$ACR_USERNAME" \
  --docker-registry-server-password "$ACR_PASSWORD" \
  --output none

echo -e "${GREEN}✅ Container Registry configurado${NC}"

# 11. Store ACR credentials in Key Vault
echo -e "\n${BLUE}11. Guardando credenciales de ACR en Key Vault${NC}"
az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "AcrUsername" \
  --value "$ACR_USERNAME" \
  --output none

az keyvault secret set \
  --vault-name "$KEY_VAULT_NAME" \
  --name "AcrPassword" \
  --value "$ACR_PASSWORD" \
  --output none

echo -e "${GREEN}✅ Credenciales de ACR guardadas${NC}"

# 12. Create Service Principal for CI/CD
echo -e "\n${BLUE}12. Creando Service Principal para GitHub Actions${NC}"

SERVICE_PRINCIPAL_JSON=$(az ad sp create-for-rbac \
  --name "ProductCatalogDeployment" \
  --role Contributor \
  --scopes "/subscriptions/${SUBSCRIPTION_ID}" \
  --sdk-auth)

echo -e "${GREEN}✅ Service Principal creado${NC}"

# Output Summary
echo -e "\n${GREEN}=== RESUMEN DE CONFIGURACIÓN ===${NC}"
echo -e "${BLUE}Resource Group:${NC} $RESOURCE_GROUP"
echo -e "${BLUE}Región:${NC} $LOCATION"
echo -e "${BLUE}PostgreSQL Server:${NC} $POSTGRES_FQDN"
echo -e "${BLUE}Base de Datos:${NC} $POSTGRES_DB_NAME"
echo -e "${BLUE}Container Registry:${NC} $ACR_URL"
echo -e "${BLUE}Key Vault:${NC} $KEY_VAULT_NAME"
echo -e "${BLUE}Backend URL:${NC} $BACKEND_URL"
if [ "$CREATE_FRONTEND" == "s" ] || [ "$CREATE_FRONTEND" == "S" ]; then
    echo -e "${BLUE}Frontend URL:${NC} $FRONTEND_URL"
fi

echo -e "\n${BLUE}=== SECRETOS PARA GITHUB ===${NC}"
echo -e "Agrega los siguientes secrets en GitHub (Repo > Settings > Secrets > Actions):\n"

echo -e "${BLUE}AZURE_CREDENTIALS:${NC}"
echo "$SERVICE_PRINCIPAL_JSON"

echo -e "\n${BLUE}AZURE_SUBSCRIPTION_ID:${NC}"
echo "$SUBSCRIPTION_ID"

echo -e "\n${BLUE}AZURE_RESOURCE_GROUP:${NC}"
echo "$RESOURCE_GROUP"

echo -e "\n${BLUE}AZURE_BACKEND_APP_NAME:${NC}"
echo "$BACKEND_APP_SERVICE"

if [ "$CREATE_FRONTEND" == "s" ] || [ "$CREATE_FRONTEND" == "S" ]; then
    echo -e "\n${BLUE}AZURE_FRONTEND_APP_NAME:${NC}"
    echo "$FRONTEND_APP_SERVICE"
fi

echo -e "\n${BLUE}ACR_NAME:${NC}"
echo "$ACR_NAME"

echo -e "\n${BLUE}ACR_URL:${NC}"
echo "$ACR_URL"

echo -e "\n${BLUE}ACR_USERNAME:${NC}"
echo "$ACR_USERNAME"

echo -e "\n${BLUE}ACR_PASSWORD:${NC}"
echo "$ACR_PASSWORD"

echo -e "\n${BLUE}POSTGRES_CONNECTION_STRING:${NC}"
echo "$CONNECTION_STRING"

echo -e "\n${BLUE}KEY_VAULT_NAME:${NC}"
echo "$KEY_VAULT_NAME"

echo -e "\n${GREEN}✅ Setup completado${NC}"
echo -e "${YELLOW}⚠️  Próximos pasos:${NC}"
echo -e "   1. Clona el repositorio en tu máquina local"
echo -e "   2. Build y push de la imagen Docker: docker build . -t $ACR_URL/productcatalog-api:latest"
echo -e "   3. Push a ACR: docker push $ACR_URL/productcatalog-api:latest"
echo -e "   4. Ejecuta migraciones de BD en Azure:"
echo -e "      dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.Api"
echo -e "   5. Configura los GitHub Secrets y CI/CD workflows"
echo -e "\n"
