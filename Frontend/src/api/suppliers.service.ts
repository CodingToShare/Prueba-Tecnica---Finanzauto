import { apiClient } from './client';
import type { SupplierDto } from '@/types/api';

/**
 * Suppliers API Service
 * Handles supplier-related API operations
 */
export const suppliersService = {
  /**
   * Get all suppliers
   */
  getAll: async (): Promise<SupplierDto[]> => {
    const response = await apiClient.get<SupplierDto[]>('/api/Suppliers');
    return response;
  },

  /**
   * Get supplier by ID
   */
  getById: async (id: number): Promise<SupplierDto> => {
    const response = await apiClient.get<SupplierDto>(`/api/Suppliers/${id}`);
    return response;
  },
};
