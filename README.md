# 🚀 Product Catalog - Full Stack Application

Aplicación completa de catálogo de productos con backend en .NET 10.0 y frontend en React + TypeScript, lista para deployment en Azure.

## 📋 Descripción

Sistema de catálogo de productos empresarial que implementa Clean Architecture, autenticación JWT, y operaciones CRUD completas. Incluye funcionalidades de bulk insert para manejo eficiente de grandes volúmenes de datos.

## 🏗️ Arquitectura

### Backend (.NET 10.0)
- **Clean Architecture** con 4 capas:
  - `Domain`: Entidades y contratos
  - `Application`: Servicios y DTOs
  - `Infrastructure`: Repositorios y DbContext
  - `API`: Controllers y configuración
- **Entity Framework Core 10** con PostgreSQL
- **Autenticación JWT** con roles (Admin/User)
- **Swagger/OpenAPI** para documentación
- **Tests**: 48 tests (20 unitarios + 28 integración) ✅

### Frontend (React + TypeScript)
- **React 19** con TypeScript
- **Vite** para build y desarrollo
- **React Router** para navegación
- **React Hook Form** para formularios
- **Vitest** para testing
- **Responsive Design** compatible con mobile

### Infraestructura
- **Docker** multi-stage builds
- **GitHub Actions** CI/CD
- **Azure App Services** (Linux containers)
- **PostgreSQL** en Azure
- **Azure Container Registry**

## 🛠️ Tecnologías

### Backend
- .NET 10.0
- Entity Framework Core 10
- PostgreSQL (Npgsql)
- JWT Authentication
- BCrypt password hashing
- Swagger/OpenAPI
- xUnit + Moq (Testing)

### Frontend
- React 19.2
- TypeScript 5.9
- Vite 7.2
- React Router 7.9
- React Hook Form 7.66
- Vitest 4.0
- NGINX (production)

### DevOps
- Docker
- GitHub Actions
- Azure CLI
- Azure App Services
- Azure Container Registry
- Azure PostgreSQL

## 🚀 Quick Start

### Prerequisitos
- .NET 10.0 SDK
- Node.js 20+
- Docker Desktop
- PostgreSQL 14+ (para desarrollo local)
- Git

### Development Local

#### Backend
```bash
cd Backend

# Restaurar dependencias
dotnet restore

# Aplicar migraciones
dotnet ef database update --project ProductCatalog.Infrastructure --startup-project ProductCatalog.Api

# Ejecutar API
dotnet run --project ProductCatalog.Api
# API disponible en http://localhost:5000
# Swagger en http://localhost:5000/swagger
```

#### Frontend
```bash
cd Frontend

# Instalar dependencias
npm install

# Ejecutar en modo desarrollo
npm run dev
# App disponible en http://localhost:3000
```

#### Tests
```bash
# Backend - Todos los tests
cd Backend
dotnet test ProductCatalog.Tests

# Frontend - Tests unitarios
cd Frontend
npm test
```

### Docker Local

#### Backend
```bash
docker build -t productcatalog-api -f Dockerfile .
docker run -d -p 8080:80 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=ProductCatalogDb;Username=postgres;Password=postgres" \
  -e Jwt__Key="your-secret-key-minimum-32-characters" \
  productcatalog-api
```

#### Frontend
```bash
docker build -t productcatalog-web -f Dockerfile.frontend \
  --build-arg VITE_API_BASE_URL=http://localhost:8080 \
  .
docker run -d -p 3000:80 \
  -e VITE_API_BASE_URL=http://localhost:8080 \
  productcatalog-web
```

## 📦 Deployment a Azure

### Prerequisitos de Deployment
1. ✅ Recursos de Azure desplegados (ver `azure-setup.sh`)
2. ✅ GitHub Secrets configurados (ver `PRE-DEPLOYMENT-CHECKLIST.md`)
3. ✅ Dockerfiles validados
4. ✅ Workflows de GitHub Actions configurados

### Primera Deployment

1. **Configurar GitHub Secrets** siguiendo `PRE-DEPLOYMENT-CHECKLIST.md`

2. **Push a main:**
```bash
git add .
git commit -m "Initial deployment"
git push origin main
```

3. **Monitorear workflows** en GitHub Actions

4. **Verificar deployment:**
```bash
# Backend health
curl https://app-productcatalog-api.azurewebsites.net/health

# Frontend
curl https://app-productcatalog-web.azurewebsites.net/health
```

Ver documentación completa en [`DEPLOYMENT.md`](DEPLOYMENT.md)

## 🔌 API Endpoints

### Autenticación
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Registro (Admin only)

### Products
- `GET /api/products` - Listar productos (paginado)
- `GET /api/products/{id}` - Obtener producto
- `POST /api/products` - Crear producto (Auth)
- `PUT /api/products/{id}` - Actualizar producto (Auth)
- `DELETE /api/products/{id}` - Eliminar producto (Admin)
- `POST /api/products/bulk` - Inserción masiva (Auth)

### Categories
- `GET /api/categories` - Listar categorías
- `GET /api/categories/{id}` - Obtener categoría
- `POST /api/categories` - Crear categoría (Admin)

### Health
- `GET /health` - Health check básico
- `GET /health/detailed` - Health check detallado (con BD)
- `GET /health/ready` - Readiness probe
- `GET /health/live` - Liveness probe

Ver documentación completa en Swagger: `https://app-productcatalog-api.azurewebsites.net/swagger`

## 🧪 Testing

### Backend
```bash
cd Backend

# Todos los tests
dotnet test ProductCatalog.Tests

# Solo unitarios
dotnet test ProductCatalog.Tests --filter "FullyQualifiedName~Unit"

# Solo integración
dotnet test ProductCatalog.Tests --filter "FullyQualifiedName~Integration"

# Con cobertura
dotnet test ProductCatalog.Tests --collect:"XPlat Code Coverage"
```

**Resultado actual:** ✅ 48/48 tests passing
- 20 tests unitarios
- 28 tests de integración

### Frontend
```bash
cd Frontend

# Tests
npm test

# Tests con UI
npm run test:ui

# Coverage
npm run test:coverage
```

## 🏢 Estructura del Proyecto

```
├── Backend/
│   ├── ProductCatalog.Api/              # API Controllers
│   ├── ProductCatalog.Application/      # Business Logic & DTOs
│   ├── ProductCatalog.Domain/           # Entities & Interfaces
│   ├── ProductCatalog.Infrastructure/   # Data Access & Repositories
│   └── ProductCatalog.Tests/            # Tests
├── Frontend/
│   ├── src/
│   │   ├── api/                         # API clients
│   │   ├── components/                  # React components
│   │   ├── context/                     # React Context
│   │   ├── pages/                       # Page components
│   │   ├── types/                       # TypeScript types
│   │   └── config/                      # Configuration
│   ├── public/                          # Static assets
│   └── nginx.conf                       # NGINX config
├── .github/
│   └── workflows/
│       ├── azure-deploy-backend.yml     # Backend CI/CD
│       └── azure-deploy-frontend.yml    # Frontend CI/CD
├── Dockerfile                           # Backend Docker
├── Dockerfile.frontend                  # Frontend Docker
├── .dockerignore
├── azure-setup.sh                       # Azure resources setup
├── CLAUDE.md                            # Architecture docs
├── DEPLOYMENT.md                        # Deployment guide
├── PRE-DEPLOYMENT-CHECKLIST.md          # Pre-deployment checklist
└── README.md                            # This file
```

## 🔐 Seguridad

- ✅ Autenticación JWT con expiración
- ✅ Passwords hasheados con BCrypt
- ✅ Autorización basada en roles (Admin/User)
- ✅ CORS configurado
- ✅ HTTPS en producción (Azure)
- ✅ Secrets en Azure Key Vault y GitHub Secrets
- ✅ SQL injection protection (EF Core parameterized queries)
- ✅ XSS protection (React escapes by default)

## 📊 Recursos de Azure

```
Resource Group: rg-productcatalog
Región: eastus2

Recursos:
- PostgreSQL: productcatalog-db-56651.postgres.database.azure.com
- Container Registry: acr56651.azurecr.io
- Backend App: app-productcatalog-api
- Frontend App: app-productcatalog-web
- Key Vault: kvpc56651

URLs Producción:
- Backend: https://app-productcatalog-api.azurewebsites.net
- Frontend: https://app-productcatalog-web.azurewebsites.net
- Swagger: https://app-productcatalog-api.azurewebsites.net/swagger
```

## 🤝 Contributing

1. Fork el repositorio
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📝 Convenciones de Código

### Backend
- Clean Architecture principles
- Repository Pattern
- Dependency Injection
- Async/await para operaciones I/O
- DTOs para contratos API
- Nombres descriptivos en inglés

### Frontend
- Functional components con hooks
- TypeScript strict mode
- ESLint + Prettier
- Component-based architecture
- Custom hooks para lógica reutilizable

## 🐛 Troubleshooting

Ver sección de Troubleshooting en [`DEPLOYMENT.md`](DEPLOYMENT.md)

### Problemas Comunes

1. **Tests de integración fallan:** Asegúrate de que la variable de entorno `ASPNETCORE_TESTING_SKIP_DBCONTEXT` no esté configurada
2. **Docker build falla:** Verifica `.dockerignore` y que todos los archivos necesarios estén presentes
3. **API no conecta a BD:** Verifica connection string y firewall de PostgreSQL

## 📄 License

Este proyecto es parte de una prueba técnica.

## 👥 Authors

- **Development** - Prueba Técnica Finanzauto
- **Architecture** - Clean Architecture + Domain-Driven Design

## 🙏 Acknowledgments

- Clean Architecture por Robert C. Martin
- Entity Framework Core Team
- React Team
- .NET Community

---

**Status:** ✅ Production Ready
**Last Updated:** 2025-11-28
**Version:** 1.0.0
