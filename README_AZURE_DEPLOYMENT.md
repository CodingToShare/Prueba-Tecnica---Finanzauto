# 🚀 Azure Deployment - Resumen de Cambios

## 📦 Archivos Modificados

### 1. `.github/workflows/azure-deploy-backend.yml`
**Antes**: El workflow intentaba ejecutar migraciones manualmente y fallaba
**Ahora**: 
- ✅ Configura variables de entorno automáticamente en Azure
- ✅ Espera 2 minutos para que el contenedor inicie
- ✅ Health checks con 10 reintentos
- ✅ Elimina el paso de migraciones manuales (innecesario)

### 2. `Backend/docker-entrypoint.sh`
**Antes**: Solo funcionaba con Docker Compose (host hardcodeado a "db")
**Ahora**:
- ✅ Detecta automáticamente si está en Azure o Docker Compose
- ✅ En Azure: Inicia directamente sin scripts de inicialización
- ✅ En local: Funciona igual que antes (migraciones + rename + seed)

### 3. `AZURE_SETUP_GITHUB_SECRETS.md` ⭐ NUEVO
Documento detallado con:
- Lista de todos los secrets necesarios en GitHub
- Comandos para obtener cada valor
- Checklist de verificación

### 4. `initialize-azure-database.sh` ⭐ NUEVO
Script interactivo para:
- Inicializar la base de datos de Azure PostgreSQL
- Ejecutar migraciones + rename + seed
- Verificar la instalación

### 5. `AZURE_DEPLOYMENT_FIX.md` ⭐ NUEVO
Documentación completa con:
- Explicación de los problemas
- Soluciones implementadas
- Pasos para deployar
- Troubleshooting

## 🎯 ¿Qué Necesitas Hacer Ahora?

### PASO 1: Inicializar la Base de Datos (PRIMERO)

```bash
./initialize-azure-database.sh
```

Te pedirá:
- **Servidor**: Tu server de Azure PostgreSQL (ej: `productcatalog-postgres.postgres.database.azure.com`)
- **Usuario**: El usuario admin
- **Contraseña**: La contraseña del admin
- **Base de datos**: `ProductCatalogDb`

Este script:
1. ✅ Conecta a tu Azure PostgreSQL
2. ✅ Ejecuta las migraciones de EF Core
3. ✅ Renombra tablas/columnas a lowercase
4. ✅ Inserta los datos iniciales (categorías, productos, usuarios)
5. ✅ Verifica que todo esté correcto

**IMPORTANTE**: Ejecuta esto ANTES de hacer push a GitHub.

### PASO 2: Configurar Secrets en GitHub

Ve a: **GitHub** → **Tu Repo** → **Settings** → **Secrets and variables** → **Actions**

Necesitas configurar estos 9 secrets (ver detalles en `AZURE_SETUP_GITHUB_SECRETS.md`):

1. ✅ `AZURE_CREDENTIALS` - JSON del Service Principal
2. ✅ `AZURE_SUBSCRIPTION_ID` - Tu subscription ID
3. ✅ `AZURE_RESOURCE_GROUP` - Nombre del resource group
4. ✅ `AZURE_BACKEND_APP_NAME` - Nombre del App Service
5. ✅ `ACR_URL` - URL del Container Registry (sin https://)
6. ✅ `ACR_USERNAME` - Usuario del ACR
7. ✅ `ACR_PASSWORD` - Password del ACR
8. ✅ `POSTGRES_CONNECTION_STRING` - Connection string completo
9. ✅ `JWT_KEY` - Secret key para JWT (genera uno con: `openssl rand -base64 32`)

### PASO 3: Hacer Push

```bash
git add .
git commit -m "Fix Azure deployment: improved workflow and multi-environment support"
git push origin main
```

### PASO 4: Monitorear el Deployment

1. Ve a **GitHub** → **Actions**
2. Verás el workflow "Build and Deploy Backend to Azure" ejecutándose
3. Tomará entre 6-11 minutos
4. Al finalizar verás: ✅ Deployment successful

## 📊 ¿Qué Hace el Workflow Ahora?

```
1. 🏗️  Build Docker Image (2-3 min)
   └─ Compila el proyecto .NET
   └─ Crea la imagen con multi-stage build

2. 📤 Push to Azure Container Registry (1-2 min)
   └─ Sube la imagen con tag latest y {commit-sha}

3. ⚙️  Configure App Service (30 seg)
   └─ ConnectionStrings__ProductCatalogDb
   └─ ASPNETCORE_ENVIRONMENT=Production
   └─ JWT__Key, JWT__Issuer, JWT__Audience

4. 🚀 Deploy to App Service (1-2 min)
   └─ Actualiza el contenedor con la nueva imagen

5. ⏳ Wait for Startup (2 min)
   └─ El contenedor detecta que está en Azure
   └─ Inicia directamente (sin scripts de DB)
   └─ EF Core ejecuta migraciones automáticamente

6. 🏥 Health Checks (1-2 min)
   └─ 10 intentos con 30s entre cada uno
   └─ /health debe responder 200 OK
   └─ Si falla, reintenta automáticamente

7. ✅ Success!
   └─ API disponible en: https://{tu-app}.azurewebsites.net
   └─ Swagger en: https://{tu-app}.azurewebsites.net/swagger
```

## 🔍 Verificación Post-Deployment

```bash
# 1. Health check
curl https://{tu-app}.azurewebsites.net/health

# 2. Login con admin
curl -X POST https://{tu-app}.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'

# 3. Obtener categorías
curl https://{tu-app}.azurewebsites.net/api/categories

# 4. Ver Swagger
open https://{tu-app}.azurewebsites.net/swagger
```

## ❓ FAQ

### ¿Por qué necesito ejecutar el script de inicialización primero?

El workflow ya NO ejecuta los scripts de rename y seed data en Azure (por diseño). Solo ejecuta las migraciones de EF Core. El script de inicialización se asegura de que:
1. Las tablas estén en lowercase
2. Los datos iniciales estén cargados
3. Los usuarios admin/user existan

### ¿Qué pasa si ya tengo datos en la base de datos?

El script detectará duplicados y los saltará. No borrará datos existentes.

### ¿Puedo ejecutar el script múltiples veces?

Sí, es idempotente. Detecta lo que ya existe y solo aplica lo que falta.

### ¿Qué pasa en deployments futuros?

En deployments posteriores:
- El contenedor detecta que está en Azure
- Solo ejecuta migraciones nuevas (si las hay)
- NO ejecuta rename ni seed (porque ya se hizo en el setup inicial)
- Inicia más rápido

## 🎉 Resultado Final

Después de completar estos pasos tendrás:

✅ Backend desplegado en Azure App Service  
✅ Base de datos PostgreSQL inicializada correctamente  
✅ Migraciones aplicadas automáticamente  
✅ Tablas en lowercase  
✅ Datos iniciales cargados (8 categorías, 12 productos, 2 usuarios)  
✅ Autenticación funcionando (admin/Admin123!, user/User123!)  
✅ Health checks pasando  
✅ Swagger UI disponible  
✅ Deployment automático en cada push a main/develop  

**¡Todo listo para producción!** 🚀
