# ✅ Checklist Pre-Deployment - Product Catalog

Esta lista asegura que todo esté configurado antes del primer deployment.

## 📝 Checklist Esencial

### 1. GitHub Secrets - CRÍTICO
- [ ] \`AZURE_CREDENTIALS\` configurado
- [ ] \`AZURE_SUBSCRIPTION_ID\` = 68be0e84-45c0-4bb0-828e-6577724407f4
- [ ] \`AZURE_RESOURCE_GROUP\` = rg-productcatalog
- [ ] \`AZURE_BACKEND_APP_NAME\` = app-productcatalog-api
- [ ] \`AZURE_FRONTEND_APP_NAME\` = app-productcatalog-web
- [ ] \`ACR_URL\` = acr56651.azurecr.io
- [ ] \`ACR_USERNAME\` = acr56651
- [ ] \`ACR_PASSWORD\` configurado
- [ ] \`POSTGRES_CONNECTION_STRING\` configurado

### 2. Azure App Settings - CRÍTICO

#### Backend Settings:
\`\`\`bash
az webapp config appsettings set \
  --name app-productcatalog-api \
  --resource-group rg-productcatalog \
  --settings \
    ConnectionStrings__DefaultConnection="Host=productcatalog-db-56651.postgres.database.azure.com;Port=5432;Database=ProductCatalogDb;Username=adminuser;Password=Pa\$w0rd123456;SSL Mode=Require;" \
    Jwt__Key="GENERA-UNA-CLAVE-SEGURA-32-CHARS" \
    Jwt__Issuer="ProductCatalogApi" \
    Jwt__Audience="ProductCatalogClient" \
    Jwt__ExpirationHours="24" \
    ASPNETCORE_ENVIRONMENT="Production" \
    DOCKER_REGISTRY_SERVER_URL="https://acr56651.azurecr.io" \
    DOCKER_REGISTRY_SERVER_USERNAME="acr56651" \
    DOCKER_REGISTRY_SERVER_PASSWORD="<TU_ACR_PASSWORD_AQUI>"
\`\`\`

#### Frontend Settings:
\`\`\`bash
az webapp config appsettings set \
  --name app-productcatalog-web \
  --resource-group rg-productcatalog \
  --settings \
    VITE_API_BASE_URL="https://app-productcatalog-api.azurewebsites.net" \
    DOCKER_REGISTRY_SERVER_URL="https://acr56651.azurecr.io" \
    DOCKER_REGISTRY_SERVER_USERNAME="acr56651" \
    DOCKER_REGISTRY_SERVER_PASSWORD="<TU_ACR_PASSWORD_AQUI>"
\`\`\`

### 3. PostgreSQL Firewall
\`\`\`bash
az postgres server firewall-rule create \
  --resource-group rg-productcatalog \
  --server-name productcatalog-db-56651 \
  --name AllowAzureServices \
  --start-ip-address 0.0.0.0 \
  --end-ip-address 0.0.0.0
\`\`\`

### 4. Archivos Verificados
- [x] Dockerfile (backend)
- [x] Dockerfile.frontend
- [x] .github/workflows/azure-deploy-backend.yml
- [x] .github/workflows/azure-deploy-frontend.yml
- [x] Backend tests: 48/48 passing
- [x] README.md creado

## 🚀 Ready to Deploy

Cuando todo esté ✅:

\`\`\`bash
git add .
git commit -m "Initial Azure deployment configuration"
git push origin main
\`\`\`

Luego monitorea GitHub Actions y verifica:
- https://app-productcatalog-api.azurewebsites.net/health
- https://app-productcatalog-web.azurewebsites.net/health
- https://app-productcatalog-api.azurewebsites.net/swagger

---

**Última actualización:** 2025-11-28
