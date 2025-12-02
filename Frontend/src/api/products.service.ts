/**
 * Products API Service
 * Handles all product CRUD operations
 */

import { apiClient } from './client';
import type { 
  ProductDto, 
  ProductDetailDto, 
  UpdateProductDto, 
  PagedProductsDto,
  ProductQueryParams 
} from '@/types/api';

export const productsService = {
  /**
   * Get paginated list of products with optional filters
   */
  getAll: async (params?: ProductQueryParams): Promise<PagedProductsDto> => {
    const queryParams = new URLSearchParams();

    if (params?.page) queryParams.append('page', params.page.toString());
    if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    if (params?.categoryId) queryParams.append('categoryId', params.categoryId.toString());
    if (params?.minPrice !== undefined) queryParams.append('minPrice', params.minPrice.toString());
    if (params?.maxPrice !== undefined) queryParams.append('maxPrice', params.maxPrice.toString());
    if (params?.search) queryParams.append('search', params.search);

    const query = queryParams.toString();
    const endpoint = query ? `/api/Products?${query}` : '/api/Products';

    return await apiClient.get<PagedProductsDto>(endpoint);
  },

  /**
   * Get single product by ID with full details
   */
  getById: async (id: number): Promise<ProductDetailDto> => {
    return await apiClient.get<ProductDetailDto>(`/api/Products/${id}`);
  },

  /**
   * Create new product
   */
  create: async (data: UpdateProductDto): Promise<ProductDto> => {
    return await apiClient.post<ProductDto>('/api/Products', data);
  },

  /**
   * Update existing product
   */
  update: async (id: number, data: UpdateProductDto): Promise<ProductDto> => {
    return await apiClient.put<ProductDto>(`/api/Products/${id}`, data);
  },

  /**
   * Delete product by ID
   */
  delete: async (id: number): Promise<void> => {
    return await apiClient.delete<void>(`/api/Products/${id}`);
  },
};
