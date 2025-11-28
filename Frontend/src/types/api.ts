/**
 * TypeScript types generated from OpenAPI specification
 * These match the backend API contracts exactly
 */

// ============================================
// Auth DTOs
// ============================================

export interface LoginRequestDto {
  username: string; // 3-50 chars
  password: string; // 6-100 chars
}

export interface LoginResponseDto {
  token: string;
  username: string;
  email: string;
  role: string;
  expiresAt: string; // ISO date-time string
}

export interface RegisterRequestDto {
  username: string; // 3-50 chars
  email: string; // max 100 chars
  password: string; // 6-100 chars
  role: 'Admin' | 'User';
}

export interface UserDto {
  userID: number;
  username: string;
  email: string;
  role: string;
  createdAt: string; // ISO date-time string
  isActive: boolean;
}

// ============================================
// Product DTOs
// ============================================

export interface ProductDto {
  productID: number;
  productName: string;
  supplierID: number | null;
  categoryID: number | null;
  quantityPerUnit: string | null;
  unitPrice: number | null;
  unitsInStock: number | null;
  unitsOnOrder: number | null;
  reorderLevel: number | null;
  discontinued: boolean;
  categoryName: string | null;
  supplierName: string | null;
}

export interface ProductDetailDto {
  productID: number;
  productName: string;
  supplierID: number | null;
  categoryID: number | null;
  quantityPerUnit: string | null;
  unitPrice: number | null;
  unitsInStock: number | null;
  unitsOnOrder: number | null;
  reorderLevel: number | null;
  discontinued: boolean;
  category: CategoryDto | null;
  supplier: SupplierDto | null;
}

export interface UpdateProductDto {
  productName: string; // 1-40 chars, required
  supplierID?: number | null; // 1 to max int
  categoryID?: number | null; // 1 to max int
  quantityPerUnit?: string | null; // max 50 chars
  unitPrice?: number | null; // 0 to 999999.99
  unitsInStock?: number | null; // 0 to 32767
  unitsOnOrder?: number | null; // 0 to 32767
  reorderLevel?: number | null; // 0 to 32767
  discontinued: boolean;
}

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export type PagedProductsDto = PagedResultDto<ProductDto>;

// ============================================
// Category DTOs
// ============================================

export interface CategoryDto {
  categoryID: number;
  categoryName: string;
  description: string | null;
  picture: string | null; // URL
}

export interface CreateCategoryDto {
  categoryName: string; // 1-15 chars, required
  description?: string | null; // max 500 chars
  picture?: string | null; // max 500 chars, URI format
}

// ============================================
// Supplier DTOs
// ============================================

export interface SupplierDto {
  supplierID: number;
  companyName: string;
  contactName: string | null;
  city: string | null;
  country: string | null;
  phone: string | null;
}

// ============================================
// Bulk Operations DTOs
// ============================================

export interface BulkGenerateProductsDto {
  count: number; // 1 to 100000
}

export interface BulkInsertResultDto {
  productsCreated: number;
  elapsedMilliseconds: number;
  message: string;
}

// ============================================
// Query Parameters
// ============================================

export interface ProductQueryParams {
  page?: number;
  pageSize?: number;
  categoryId?: number;
  minPrice?: number;
  maxPrice?: number;
  search?: string;
}
