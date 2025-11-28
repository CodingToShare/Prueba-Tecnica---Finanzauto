/**
 * Categories API Service
 * Handles category operations for filtering
 */

import { apiClient } from './client';
import type { CategoryDto } from '@/types/api';

export const categoriesService = {
  /**
   * Get all categories
   */
  getAll: async (): Promise<CategoryDto[]> => {
    return await apiClient.get<CategoryDto[]>('/api/Categories');
  },

  /**
   * Get single category by ID
   */
  getById: async (id: number): Promise<CategoryDto> => {
    return await apiClient.get<CategoryDto>(`/api/Categories/${id}`);
  },
};
