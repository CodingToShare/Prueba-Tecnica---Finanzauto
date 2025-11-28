# ProductCatalog API - Complete API Reference

Complete documentation for all API endpoints, request/response formats, and authentication.

## Table of Contents

- [Base URL](#base-url)
- [Authentication](#authentication)
- [Common Headers](#common-headers)
- [Response Format](#response-format)
- [Error Handling](#error-handling)
- [Pagination](#pagination)
- [Endpoints](#endpoints)
  - [Authentication](#authentication-endpoints)
  - [Products](#products-endpoints)
  - [Categories](#categories-endpoints)
  - [Suppliers](#suppliers-endpoints)
  - [Health Checks](#health-check-endpoints)

---

## Base URL

**Development**:
```
http://localhost:5000
```

**Production**:
```
https://api.yourdomain.com
```

**Docker**:
```
http://localhost:5000  (mapped from container port 8080)
```

---

## Authentication

The API uses **JWT (JSON Web Tokens)** for authentication.

### Getting a Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin123!"
}
```

**Response**:
```json
{
  "username": "admin",
  "role": "Admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

### Using the Token

Include the token in the `Authorization` header:

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Token Expiration

- Default: 1 hour
- Configurable in `appsettings.json` → `Jwt:ExpirationHours`

---

## Common Headers

### Request Headers

```http
Content-Type: application/json
Authorization: Bearer <token>    # For protected endpoints
Accept: application/json
```

### Response Headers

```http
Content-Type: application/json
X-Total-Count: 100              # Total items (for paginated responses)
X-Page: 1                       # Current page
X-Page-Size: 10                 # Items per page
```

---

## Response Format

### Success Response

```json
{
  "data": { ... },
  "message": "Success",
  "statusCode": 200
}
```

### Error Response

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ProductName": ["The ProductName field is required."]
  },
  "traceId": "00-abc123..."
}
```

---

## Error Handling

### HTTP Status Codes

| Code | Description | Example |
|------|-------------|---------|
| 200 | OK | Successful GET, PUT |
| 201 | Created | Successful POST |
| 204 | No Content | Successful DELETE |
| 400 | Bad Request | Invalid input data |
| 401 | Unauthorized | Missing/invalid token |
| 403 | Forbidden | Insufficient permissions |
| 404 | Not Found | Resource not found |
| 500 | Internal Server Error | Server error |

### Common Error Scenarios

#### 401 Unauthorized
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "No authentication token provided"
}
```

#### 403 Forbidden
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "User does not have required permissions"
}
```

#### 400 Validation Error
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "ProductName": ["The ProductName field is required."],
    "UnitPrice": ["The field UnitPrice must be between 0 and 99999."]
  }
}
```

---

## Pagination

### Request Parameters

```http
GET /api/products?page=1&pageSize=10&sortBy=ProductName&sortOrder=asc
```

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number (1-based) |
| `pageSize` | int | 10 | Items per page (max 100) |
| `sortBy` | string | null | Field to sort by |
| `sortOrder` | string | asc | Sort direction (asc/desc) |

### Response Format

```json
{
  "items": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## Endpoints

## Authentication Endpoints

### Login

Authenticate user and get JWT token.

```http
POST /api/auth/login
```

**Request Body**:
```json
{
  "username": "admin",
  "password": "Admin123!"
}
```

**Validation Rules**:
- `username`: Required, 3-50 characters
- `password`: Required, 6-100 characters

**Success Response** (200 OK):
```json
{
  "username": "admin",
  "role": "Admin",
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Error Responses**:
- `400 Bad Request`: Invalid input
- `401 Unauthorized`: Invalid credentials

**Example**:
```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"Admin123!"}'
```

---

### Register User

Register a new user (Admin only).

```http
POST /api/auth/register
Authorization: Bearer <admin-token>
```

**Request Body**:
```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "SecurePass123!",
  "role": "User"
}
```

**Validation Rules**:
- `username`: Required, 3-50 characters, unique
- `email`: Required, valid email format, unique
- `password`: Required, min 6 characters
- `role`: Required, "Admin" or "User"

**Success Response** (201 Created):
```json
{
  "userID": 3,
  "username": "newuser",
  "email": "newuser@example.com",
  "role": "User",
  "isActive": true,
  "createdAt": "2025-01-28T10:00:00Z"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid input or user already exists
- `401 Unauthorized`: No token provided
- `403 Forbidden`: Non-admin user

**Example**:
```bash
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{
    "username":"newuser",
    "email":"newuser@example.com",
    "password":"SecurePass123!",
    "role":"User"
  }'
```

---

## Products Endpoints

### Get Products

Get paginated list of products with filtering.

```http
GET /api/products?page=1&pageSize=10&categoryId=1&supplierId=2&discontinued=false
```

**Query Parameters**:

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| `page` | int | No | Page number (default: 1) |
| `pageSize` | int | No | Items per page (default: 10, max: 100) |
| `categoryId` | int | No | Filter by category ID |
| `supplierId` | int | No | Filter by supplier ID |
| `discontinued` | bool | No | Filter by discontinued status |

**Success Response** (200 OK):
```json
{
  "items": [
    {
      "productID": 1,
      "productName": "Product 1",
      "supplierID": 1,
      "categoryID": 1,
      "quantityPerUnit": "10 boxes",
      "unitPrice": 99.99,
      "unitsInStock": 100,
      "unitsOnOrder": 0,
      "reorderLevel": 10,
      "discontinued": false
    }
  ],
  "page": 1,
  "pageSize": 10,
  "totalCount": 50,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**Example**:
```bash
curl http://localhost:5000/api/products?page=1&pageSize=10&categoryId=1
```

---

### Get Product by ID

Get a single product with full details including category and supplier.

```http
GET /api/products/{id}
```

**Path Parameters**:
- `id` (int): Product ID

**Success Response** (200 OK):
```json
{
  "productID": 1,
  "productName": "Product 1",
  "supplierID": 1,
  "categoryID": 1,
  "quantityPerUnit": "10 boxes",
  "unitPrice": 99.99,
  "unitsInStock": 100,
  "unitsOnOrder": 0,
  "reorderLevel": 10,
  "discontinued": false,
  "category": {
    "categoryID": 1,
    "categoryName": "Electronics",
    "description": "Electronic devices"
  },
  "supplier": {
    "supplierID": 1,
    "companyName": "Supplier A",
    "contactName": "John Doe",
    "city": "New York",
    "country": "USA"
  }
}
```

**Error Responses**:
- `404 Not Found`: Product not found

**Example**:
```bash
curl http://localhost:5000/api/products/1
```

---

### Create Product

Create a new product (Authentication required).

```http
POST /api/products
Authorization: Bearer <token>
```

**Request Body**:
```json
{
  "productName": "New Product",
  "supplierID": 1,
  "categoryID": 1,
  "quantityPerUnit": "10 boxes",
  "unitPrice": 99.99,
  "unitsInStock": 100,
  "unitsOnOrder": 0,
  "reorderLevel": 10,
  "discontinued": false
}
```

**Validation Rules**:
- `productName`: Required, 1-100 characters
- `supplierID`: Optional
- `categoryID`: Optional
- `unitPrice`: Optional, 0-99999
- `unitsInStock`: Optional, 0-32767
- `unitsOnOrder`: Optional, 0-32767
- `reorderLevel`: Optional, 0-32767

**Success Response** (201 Created):
```json
{
  "productID": 101,
  "productName": "New Product",
  "supplierID": 1,
  "categoryID": 1,
  "unitPrice": 99.99,
  "unitsInStock": 100,
  "discontinued": false
}
```

**Error Responses**:
- `400 Bad Request`: Invalid input
- `401 Unauthorized`: No token provided

**Example**:
```bash
curl -X POST http://localhost:5000/api/products \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{
    "productName":"New Product",
    "unitPrice":99.99,
    "unitsInStock":100
  }'
```

---

### Update Product

Update an existing product (Authentication required).

```http
PUT /api/products/{id}
Authorization: Bearer <token>
```

**Path Parameters**:
- `id` (int): Product ID

**Request Body**:
```json
{
  "productName": "Updated Product",
  "supplierID": 1,
  "categoryID": 1,
  "unitPrice": 149.99,
  "unitsInStock": 150
}
```

**Success Response** (200 OK):
```json
{
  "productID": 1,
  "productName": "Updated Product",
  "unitPrice": 149.99,
  "unitsInStock": 150
}
```

**Error Responses**:
- `400 Bad Request`: Invalid input
- `401 Unauthorized`: No token provided
- `404 Not Found`: Product not found

**Example**:
```bash
curl -X PUT http://localhost:5000/api/products/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"productName":"Updated Product","unitPrice":149.99}'
```

---

### Delete Product

Soft delete a product (Admin only).

```http
DELETE /api/products/{id}
Authorization: Bearer <admin-token>
```

**Path Parameters**:
- `id` (int): Product ID

**Success Response** (204 No Content)

**Error Responses**:
- `401 Unauthorized`: No token provided
- `403 Forbidden`: Non-admin user
- `404 Not Found`: Product not found

**Note**: This is a soft delete - sets `discontinued` to `true`.

**Example**:
```bash
curl -X DELETE http://localhost:5000/api/products/1 \
  -H "Authorization: Bearer <admin-token>"
```

---

### Bulk Generate Products

Generate multiple products for testing (Authentication required).

```http
POST /api/products/bulk
Authorization: Bearer <token>
```

**Request Body**:
```json
{
  "count": 1000
}
```

**Validation Rules**:
- `count`: Required, 1-100,000

**Success Response** (200 OK):
```json
{
  "productsCreated": 1000,
  "elapsedMilliseconds": 1250,
  "message": "Successfully generated 1000 products in 1250ms"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid count
- `401 Unauthorized`: No token provided

**Example**:
```bash
curl -X POST http://localhost:5000/api/products/bulk \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token>" \
  -d '{"count":1000}'
```

---

## Categories Endpoints

### Get All Categories

Get all categories.

```http
GET /api/categories
```

**Success Response** (200 OK):
```json
[
  {
    "categoryID": 1,
    "categoryName": "Electronics",
    "description": "Electronic devices"
  },
  {
    "categoryID": 2,
    "categoryName": "Books",
    "description": "Books and literature"
  }
]
```

**Example**:
```bash
curl http://localhost:5000/api/categories
```

---

### Get Category by ID

Get a single category.

```http
GET /api/categories/{id}
```

**Path Parameters**:
- `id` (int): Category ID

**Success Response** (200 OK):
```json
{
  "categoryID": 1,
  "categoryName": "Electronics",
  "description": "Electronic devices"
}
```

**Error Responses**:
- `404 Not Found`: Category not found

---

### Create Category

Create a new category (Admin only).

```http
POST /api/categories
Authorization: Bearer <admin-token>
```

**Request Body**:
```json
{
  "categoryName": "New Category",
  "description": "Category description"
}
```

**Validation Rules**:
- `categoryName`: Required, 1-100 characters
- `description`: Optional, max 500 characters

**Success Response** (201 Created):
```json
{
  "categoryID": 3,
  "categoryName": "New Category",
  "description": "Category description"
}
```

**Error Responses**:
- `400 Bad Request`: Invalid input
- `401 Unauthorized`: No token
- `403 Forbidden`: Non-admin user

---

## Health Check Endpoints

### Basic Health Check

Check if the API is running.

```http
GET /health
```

**Success Response** (200 OK):
```json
{
  "status": "Healthy",
  "timestamp": "2025-01-28T10:00:00Z",
  "service": "ProductCatalog API",
  "version": "1.0.0"
}
```

---

### Detailed Health Check

Check API health with database connectivity.

```http
GET /health/detailed
```

**Success Response** (200 OK):
```json
{
  "status": "Healthy",
  "timestamp": "2025-01-28T10:00:00Z",
  "service": "ProductCatalog API",
  "version": "1.0.0",
  "checks": {
    "database": {
      "status": "Healthy",
      "responseTime": "< 100ms"
    },
    "migrations": {
      "status": "Healthy",
      "pendingCount": 0
    }
  }
}
```

**Error Response** (503 Service Unavailable):
```json
{
  "status": "Unhealthy",
  "timestamp": "2025-01-28T10:00:00Z",
  "service": "ProductCatalog API",
  "error": "Cannot connect to database"
}
```

---

### Readiness Check

Check if the API is ready to accept requests.

```http
GET /health/ready
```

**Success Response** (200 OK):
```json
{
  "status": "Ready"
}
```

**Error Response** (503 Service Unavailable):
```json
{
  "status": "Not Ready"
}
```

---

### Liveness Check

Check if the API is alive.

```http
GET /health/live
```

**Success Response** (200 OK):
```json
{
  "status": "Alive"
}
```

---

## Rate Limiting

When using Nginx reverse proxy, rate limiting is applied:

- **Default**: 10 requests/second per IP
- **Burst**: 20 requests
- **Response**: 429 Too Many Requests

```json
{
  "error": "Too many requests",
  "retryAfter": 1
}
```

---

## Versioning

Currently using implicit versioning through URL path:
- v1: `/api/...` (current)

Future versions will use explicit versioning:
- v2: `/api/v2/...`

---

## OpenAPI/Swagger

Interactive API documentation available at:

**Development**:
```
http://localhost:5000/openapi/index.html
```

Features:
- Try out endpoints interactively
- View request/response schemas
- Export OpenAPI specification
- Test authentication

---

## Best Practices

### Authentication
- Store JWT securely (httpOnly cookies or secure storage)
- Refresh tokens before expiry
- Don't share tokens

### Error Handling
- Check HTTP status codes
- Handle all error responses
- Log errors for debugging

### Performance
- Use pagination for large datasets
- Cache responses when appropriate
- Minimize payload size

### Security
- Always use HTTPS in production
- Validate input on client side
- Don't expose sensitive data in URLs

---

## SDKs & Client Libraries

Coming soon:
- C# Client Library
- JavaScript/TypeScript SDK
- Python Client
- Postman Collection

---

## Changelog

See [CHANGELOG.md](../CHANGELOG.md) for version history and API changes.

---

## Support

- Issues: GitHub Issues
- Email: api-support@example.com
- Documentation: This guide

---

**[Back to Main Documentation](../README.md)**
