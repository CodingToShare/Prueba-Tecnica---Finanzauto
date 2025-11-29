# GitHub Secrets Configuration for Azure Deployment

Este documento detalla todos los secrets que debes configurar en GitHub para que el workflow de CI/CD funcione correctamente.

## Cómo Configurar Secrets en GitHub

1. Ve a tu repositorio en GitHub
2. Click en **Settings** → **Secrets and variables** → **Actions**
3. Click en **New repository secret**
4. Añade cada uno de los siguientes secrets:

## Secrets Requeridos

### 1. AZURE_CREDENTIALS
Service Principal credentials en formato JSON.

**Cómo obtenerlo:**
```bash
az ad sp create-for-rbac \
  --name "github-actions-productcatalog" \
  --role contributor \
  --scopes /subscriptions/{subscription-id}/resourceGroups/{resource-group} \
  --sdk-auth
```

**Formato esperado:**
```json
{
  "clientId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "clientSecret": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
  "subscriptionId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "tenantId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
}
```

### 2. AZURE_SUBSCRIPTION_ID
Tu Azure subscription ID.

**Cómo obtenerlo:**
```bash
az account show --query id -o tsv
```

**Ejemplo:**
```
12345678-1234-1234-1234-123456789012
```

### 3. AZURE_RESOURCE_GROUP
Nombre del resource group donde están tus recursos de Azure.

**Ejemplo:**
```
rg-productcatalog-prod
```

### 4. AZURE_BACKEND_APP_NAME
Nombre de tu Azure App Service para el backend.

**Ejemplo:**
```
app-productcatalog-api
```

### 5. ACR_URL
URL de tu Azure Container Registry (sin https://).

**Cómo obtenerlo:**
```bash
az acr show --name {tu-acr-name} --query loginServer -o tsv
```

**Ejemplo:**
```
productcatalogacr.azurecr.io
```

### 6. ACR_USERNAME
Username del Azure Container Registry.

**Cómo obtenerlo:**
```bash
az acr credential show --name {tu-acr-name} --query username -o tsv
```

### 7. ACR_PASSWORD
Password del Azure Container Registry.

**Cómo obtenerlo:**
```bash
az acr credential show --name {tu-acr-name} --query "passwords[0].value" -o tsv
```

### 8. POSTGRES_CONNECTION_STRING
Connection string completo de tu Azure PostgreSQL Flexible Server.

**Formato:**
```
Host={server-name}.postgres.database.azure.com;Port=5432;Database=ProductCatalogDb;Username={admin-user};Password={password};SSL Mode=Require;Trust Server Certificate=true
```

**Ejemplo:**
```
Host=productcatalog-postgres.postgres.database.azure.com;Port=5432;Database=ProductCatalogDb;Username=adminuser;Password=YourSecurePassword123!;SSL Mode=Require;Trust Server Certificate=true
```

### 9. JWT_KEY
Clave secreta para firmar los JWT tokens. **Debe ser una cadena segura de al menos 32 caracteres.**

**Generar una:**
```bash
openssl rand -base64 32
```

**Ejemplo:**
```
Kq8B9FJc2d5E6fG7hH8iI9jJ0kK1lL2mM3nN4oO5pP6qQ==
```

## Verificar Secrets Configurados

Después de configurar todos los secrets, verifica que estén correctamente guardados:

1. Ve a **Settings** → **Secrets and variables** → **Actions**
2. Deberías ver todos los secrets listados (los valores estarán ocultos)

## Lista de Verificación

- [ ] AZURE_CREDENTIALS (JSON del Service Principal)
- [ ] AZURE_SUBSCRIPTION_ID
- [ ] AZURE_RESOURCE_GROUP
- [ ] AZURE_BACKEND_APP_NAME
- [ ] ACR_URL
- [ ] ACR_USERNAME
- [ ] ACR_PASSWORD
- [ ] POSTGRES_CONNECTION_STRING
- [ ] JWT_KEY

## Siguiente Paso

Una vez configurados todos los secrets, puedes hacer push de tus cambios a la rama `main` o `develop` para que se dispare el workflow automáticamente.

```bash
git add .
git commit -m "Configure Azure deployment workflow"
git push origin main
```

El workflow se ejecutará automáticamente y podrás ver el progreso en:
**GitHub** → **Actions** → **Build and Deploy Backend to Azure**

## Troubleshooting

### Error: "secret is not set"
- Verifica que el nombre del secret coincida exactamente (case-sensitive)
- Verifica que el secret esté configurado en el repositorio correcto

### Error de autenticación en Azure
- Verifica que AZURE_CREDENTIALS tenga el formato JSON correcto
- Verifica que el Service Principal tenga permisos de contributor

### Error de conexión a PostgreSQL
- Verifica que la connection string sea correcta
- Verifica que las reglas de firewall de Azure PostgreSQL permitan conexiones desde Azure services
- Verifica que el usuario y password sean correctos
