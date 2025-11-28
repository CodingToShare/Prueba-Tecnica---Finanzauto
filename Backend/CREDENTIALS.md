# 🔐 Credenciales y Datos Iniciales - Product Catalog API

## 📋 Información General

Esta aplicación incluye datos iniciales que se aplican automáticamente al desplegar la base de datos mediante migraciones de Entity Framework Core.

## 👥 Usuarios del Sistema

### Usuario Administrador
- **Username:** `admin`
- **Password:** `Admin123!`
- **Email:** admin@productcatalog.com
- **Rol:** Admin
- **Permisos:** Acceso completo al sistema

### Usuario Regular
- **Username:** `user`
- **Password:** `User123!`
- **Email:** user@productcatalog.com
- **Rol:** User
- **Permisos:** Acceso de lectura y operaciones básicas

## 🗄️ Datos Iniciales

La migración `SeedInitialData` incluye:

### Categorías (8)
1. Beverages - Bebidas, cafés, tés, cervezas
2. Condiments - Salsas, especias, aderezos
3. Confections - Postres, dulces, panes dulces
4. Dairy Products - Quesos
5. Grains/Cereals - Panes, galletas, pasta, cereales
6. Meat/Poultry - Carnes preparadas
7. Produce - Frutas secas y derivados de soja
8. Seafood - Algas y pescado

### Proveedores (5)
1. **Exotic Liquids** (UK) - Charlotte Cooper
2. **New Orleans Cajun Delights** (USA) - Shelley Burke
3. **Grandma Kelly's Homestead** (USA) - Regina Murphy
4. **Tokyo Traders** (Japan) - Yoshi Nagase
5. **Cooperativa de Quesos 'Las Cabras'** (Spain) - Antonio del Valle Saavedra

### Productos (12)
1. Chai - $18.00
2. Chang - $19.00
3. Aniseed Syrup - $10.00
4. Chef Anton's Cajun Seasoning - $22.00
5. Chef Anton's Gumbo Mix - $21.35 (Descontinuado)
6. Grandma's Boysenberry Spread - $25.00
7. Uncle Bob's Organic Dried Pears - $30.00
8. Northwoods Cranberry Sauce - $40.00
9. Mishi Kobe Niku - $97.00 (Descontinuado)
10. Ikura - $31.00
11. Queso Cabrales - $21.00
12. Queso Manchego La Pastora - $38.00

### Transportistas (3)
1. Speedy Express - (503) 555-9831
2. United Package - (503) 555-3199
3. Federal Shipping - (503) 555-9931

## 🚀 Aplicar Migraciones

### En Desarrollo Local
```bash
cd Backend
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.Api
```

### En Azure (mediante GitHub Actions)
Las migraciones se aplican automáticamente durante el deployment mediante el workflow `.github/workflows/azure-deploy-backend.yml`

## 📡 Endpoints Disponibles

### Swagger UI
- **URL:** `https://tu-app.azurewebsites.net/swagger`
- **Descripción:** Documentación interactiva de la API

### Health Checks
- **Basic:** `/health`
- **Detailed:** `/health/detailed`

### Autenticación
- **POST** `/auth/login` - Iniciar sesión
- **POST** `/auth/register` - Registrar nuevo usuario

### Productos
- **GET** `/product` - Listar productos (con paginación y filtros)
- **GET** `/product/{id}` - Obtener producto por ID
- **POST** `/product` - Crear producto (requiere autenticación)
- **PUT** `/product/{id}` - Actualizar producto (requiere autenticación)
- **DELETE** `/product/{id}` - Eliminar producto (soft delete, requiere autenticación)

### Categorías
- **GET** `/categories` - Listar categorías
- **GET** `/categories/{id}` - Obtener categoría por ID
- **POST** `/categories` - Crear categoría (requiere autenticación Admin)
- **PUT** `/categories/{id}` - Actualizar categoría (requiere autenticación Admin)
- **DELETE** `/categories/{id}` - Eliminar categoría (requiere autenticación Admin)

## 🔒 Autenticación JWT

Para usar endpoints protegidos:

1. **Obtener token JWT:**
```bash
curl -X POST https://tu-app.azurewebsites.net/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin123!"
  }'
```

2. **Usar token en requests:**
```bash
curl -X GET https://tu-app.azurewebsites.net/product \
  -H "Authorization: Bearer TU_TOKEN_JWT"
```

## 📝 Notas Importantes

1. **Contraseñas:** Las contraseñas están hasheadas con SHA256
2. **Soft Delete:** Los productos eliminados se marcan como "Discontinued" en lugar de eliminarse físicamente
3. **CORS:** La API acepta requests desde los orígenes configurados en `appsettings.Production.json`
4. **Base de Datos:** PostgreSQL en Azure con SSL requerido
5. **Swagger en Producción:** Ahora habilitado para facilitar pruebas y documentación

## 🔧 Troubleshooting

### No puedo acceder a Swagger
Verifica que la URL sea: `https://tu-app.azurewebsites.net/swagger` (sin `/index.html`)

### Error de autenticación
- Verifica que el token JWT no haya expirado (24 horas de validez)
- Asegúrate de incluir el prefijo "Bearer " en el header Authorization

### Error de CORS
- Verifica que tu dominio frontend esté incluido en `appsettings.Production.json` bajo `Cors:AllowedOrigins`

### Base de datos vacía
- Verifica que las migraciones se hayan aplicado correctamente
- Revisa los logs del workflow de GitHub Actions
