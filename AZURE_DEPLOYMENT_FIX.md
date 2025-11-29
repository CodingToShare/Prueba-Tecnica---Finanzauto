# Solución al Problema de Deployment en Azure

## 🔍 Problemas Identificados

### 1. **Error en las Migraciones del Workflow**
```
error NETSDK1004: Assets file 'project.assets.json' not found
```
**Causa**: El workflow intentaba ejecutar migraciones manualmente sin restaurar paquetes NuGet primero.

### 2. **Health Check Timeout (504)**
```
curl: (22) The requested URL returned error: 504
```
**Causa**: 
- El contenedor tarda 2-3 minutos en iniciar (migraciones + rename + seed)
- El workflow solo esperaba 30 segundos
- No se configuraron las variables de entorno en Azure App Service

### 3. **Docker Entrypoint No Compatible con Azure**
El script `docker-entrypoint.sh` estaba diseñado solo para Docker Compose local, usando el host `db` hardcodeado.

## ✅ Soluciones Implementadas

### 1. **Workflow de GitHub Actions Mejorado**

**Cambios en `.github/workflows/azure-deploy-backend.yml`:**

- ❌ **Eliminado**: Paso manual de migraciones (era innecesario y causaba errores)
- ✅ **Agregado**: Configuración automática de variables de entorno en App Service
- ✅ **Agregado**: Tiempo de espera aumentado a 120 segundos
- ✅ **Agregado**: Health checks con reintentos (10 intentos con 30s entre cada uno)
- ✅ **Agregado**: Verificación de logs de Azure
- ✅ **Agregado**: Test del endpoint de autenticación

**Nuevas variables de entorno configuradas automáticamente:**
```yaml
ConnectionStrings__ProductCatalogDb  # Connection string de PostgreSQL
ASPNETCORE_ENVIRONMENT               # Production
JWT__Key                             # Secret key para JWT
JWT__Issuer                          # Emisor del token
JWT__Audience                        # Audiencia del token
JWT__ExpirationHours                 # 24 horas
```

### 2. **Docker Entrypoint Multi-Entorno**

**Cambios en `Backend/docker-entrypoint.sh`:**

✅ **Detección automática de entorno:**
- Si existe variable `ConnectionStrings__ProductCatalogDb` → **Azure App Service**
- Si no existe → **Docker Compose local**

✅ **Modo Azure:**
- Inicia directamente sin esperar PostgreSQL local
- Las migraciones se ejecutan automáticamente por EF Core
- Usa la connection string de variables de entorno

✅ **Modo Local (Docker Compose):**
- Espera a que PostgreSQL esté listo
- Ejecuta migraciones + rename + seed automáticamente
- Funciona igual que antes

### 3. **Documentación y Scripts**

✅ **Nuevo archivo: `AZURE_SETUP_GITHUB_SECRETS.md`**
- Lista completa de todos los secrets requeridos
- Comandos para obtener cada valor
- Checklist de verificación

✅ **Nuevo script: `initialize-azure-database.sh`**
- Inicializa la base de datos de Azure PostgreSQL manualmente
- Ejecuta migraciones + rename + seed
- Verifica la instalación
- Útil para setup inicial o troubleshooting

## 📋 Pasos para Deployar

### **Opción 1: Deployment Completo desde Cero**

#### 1. Inicializar la Base de Datos de Azure (Una vez)

```bash
# Ejecutar desde la raíz del repositorio
./initialize-azure-database.sh
```

El script te pedirá:
- Servidor PostgreSQL (ej: `productcatalog-postgres.postgres.database.azure.com`)
- Usuario admin
- Contraseña
- Nombre de la base de datos

Esto creará las tablas y cargará los datos iniciales.

#### 2. Configurar Secrets en GitHub

Sigue las instrucciones en `AZURE_SETUP_GITHUB_SECRETS.md` para configurar:

- ✅ `AZURE_CREDENTIALS`
- ✅ `AZURE_SUBSCRIPTION_ID`
- ✅ `AZURE_RESOURCE_GROUP`
- ✅ `AZURE_BACKEND_APP_NAME`
- ✅ `ACR_URL`
- ✅ `ACR_USERNAME`
- ✅ `ACR_PASSWORD`
- ✅ `POSTGRES_CONNECTION_STRING`
- ✅ `JWT_KEY`

#### 3. Hacer Push para Disparar el Deployment

```bash
git add .
git commit -m "Fix Azure deployment workflow"
git push origin main
```

El workflow se ejecutará automáticamente y:
1. Construirá la imagen Docker (2-3 minutos)
2. La subirá a Azure Container Registry
3. Configurará las variables de entorno en App Service
4. Desplegará el contenedor
5. Esperará que el servicio inicie (2 minutos)
6. Ejecutará health checks con reintentos
7. Probará el endpoint de autenticación

### **Opción 2: Usar Azure con Migraciones Automáticas**

Si prefieres que las migraciones se ejecuten automáticamente en cada deployment:

#### 1. Asegúrate de que la base de datos exista

Crea la base de datos en Azure PostgreSQL:
```bash
az postgres flexible-server db create \
  --resource-group {tu-resource-group} \
  --server-name {tu-server} \
  --database-name ProductCatalogDb
```

#### 2. Configura los Secrets en GitHub

Mismo paso que la Opción 1.

#### 3. Haz Push

El contenedor ejecutará automáticamente:
- Migraciones de EF Core
- **PERO**: No ejecutará los scripts de rename ni seed data (porque detecta entorno Azure)

**Nota**: En este caso, necesitarás ejecutar manualmente los scripts de rename y seed después de la primera migración.

## 🔧 Troubleshooting

### El Health Check sigue fallando

1. **Verifica los logs en Azure:**
```bash
az webapp log tail \
  --name {tu-app-name} \
  --resource-group {tu-resource-group}
```

2. **Verifica que las variables de entorno estén configuradas:**
```bash
az webapp config appsettings list \
  --name {tu-app-name} \
  --resource-group {tu-resource-group}
```

3. **Verifica la connection string:**
```bash
# Prueba la conexión desde tu máquina
psql "Host={server}.postgres.database.azure.com;Port=5432;Database=ProductCatalogDb;Username={user};Password={pass};SSL Mode=Require"
```

### Las migraciones no se aplican en Azure

**Solución**: Ejecuta el script de inicialización manual:
```bash
./initialize-azure-database.sh
```

Esto asegura que la base de datos esté correctamente configurada antes del deployment.

### Error 504 Gateway Timeout

El contenedor puede tardar hasta 3-4 minutos en estar completamente listo en el primer deployment. El workflow ahora tiene reintentos, pero si aún falla:

1. Ve al Azure Portal
2. Abre tu App Service
3. Ve a "Log stream" para ver logs en tiempo real
4. Espera a que veas "Application started" en los logs

## 📊 Tiempo Estimado de Deployment

| Fase | Tiempo |
|------|--------|
| Build Docker image | 2-3 min |
| Push to ACR | 1-2 min |
| Deploy to App Service | 1-2 min |
| Container startup | 1-2 min |
| Health checks | 1-2 min |
| **Total** | **6-11 min** |

## ✅ Verificación Post-Deployment

Una vez que el deployment sea exitoso, verifica:

```bash
# Health check
curl https://{tu-app}.azurewebsites.net/health

# Swagger UI
curl -I https://{tu-app}.azurewebsites.net/swagger

# Login test
curl -X POST https://{tu-app}.azurewebsites.net/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```

## 📝 Notas Importantes

1. **Primera vez**: Usa `initialize-azure-database.sh` para setup inicial
2. **Deployments posteriores**: El workflow manejará todo automáticamente
3. **Variables de entorno**: Se configuran automáticamente en cada deployment
4. **Seed data**: Solo se aplica en el setup inicial manual, no en cada deployment

## 🎯 Resultado Esperado

Después de seguir estos pasos:

✅ El backend se desplegará correctamente en Azure App Service  
✅ Las migraciones se aplicarán automáticamente  
✅ La base de datos tendrá la estructura correcta (lowercase)  
✅ Los datos iniciales estarán cargados  
✅ Los health checks pasarán  
✅ El endpoint de autenticación funcionará  
✅ Swagger UI estará disponible  

**URL del API**: `https://{tu-app-name}.azurewebsites.net`  
**Swagger**: `https://{tu-app-name}.azurewebsites.net/swagger`
