# Guía de Deployment - Product Catalog

Esta guía proporciona instrucciones detalladas para desplegar la aplicación Product Catalog en Azure.

## 📋 Tabla de Contenidos

1. [Prerequisitos](#prerequisitos)
2. [Recursos de Azure](#recursos-de-azure)
3. [Configuración de GitHub Secrets](#configuración-de-github-secrets)
4. [Deployment Backend](#deployment-backend)
5. [Deployment Frontend](#deployment-frontend)
6. [Configuración de Base de Datos](#configuración-de-base-de-datos)
7. [Verificación del Deployment](#verificación-del-deployment)
8. [Troubleshooting](#troubleshooting)

## Prerequisitos

- Cuenta de Azure activa
- Repositorio en GitHub
- Azure CLI instalado (\`az cli\`)
- Docker instalado (para testing local)
- .NET 10.0 SDK (para migraciones locales)
- Node.js 20+ (para testing frontend local)

## Recursos de Azure

Los siguientes recursos ya están desplegados en Azure:

\`\`\`
Resource Group: rg-productcatalog
Región: eastus2

Recursos:
- PostgreSQL Server: productcatalog-db-56651.postgres.database.azure.com
- Base de Datos: ProductCatalogDb
- Container Registry: acr56651.azurecr.io
- Key Vault: kvpc56651
- Backend App Service: app-productcatalog-api
- Frontend App Service: app-productcatalog-web

URLs:
- Backend: https://app-productcatalog-api.azurewebsites.net
- Frontend: https://app-productcatalog-web.azurewebsites.net
\`\`\`

## Ver documentación completa

Para instrucciones detalladas paso a paso, consulta \`PRE-DEPLOYMENT-CHECKLIST.md\`

## Deployment Rápido

### 1. Configurar GitHub Secrets

Agrega todos los secrets listados en \`PRE-DEPLOYMENT-CHECKLIST.md\`

### 2. Configurar App Services

\`\`\`bash
# Backend
az webapp config appsettings set \
  --name app-productcatalog-api \
  --resource-group rg-productcatalog \
  --settings \
    ConnectionStrings__DefaultConnection="Host=productcatalog-db-56651.postgres.database.azure.com;Port=5432;Database=ProductCatalogDb;Username=adminuser;Password=Pa\$w0rd123456;SSL Mode=Require;" \
    Jwt__Key="YOUR-SECURE-JWT-KEY-MIN-32-CHARS" \
    ASPNETCORE_ENVIRONMENT="Production"

# Frontend
az webapp config appsettings set \
  --name app-productcatalog-web \
  --resource-group rg-productcatalog \
  --settings \
    VITE_API_BASE_URL="https://app-productcatalog-api.azurewebsites.net"
\`\`\`

### 3. Push y Deploy

\`\`\`bash
git add .
git commit -m "Initial deployment"
git push origin main
\`\`\`

### 4. Verificar

\`\`\`bash
curl https://app-productcatalog-api.azurewebsites.net/health
curl https://app-productcatalog-web.azurewebsites.net/health
\`\`\`

## Troubleshooting

Ver README.md para más detalles de troubleshooting.

---

**Última actualización:** 2025-11-28
