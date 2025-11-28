/**
 * Authentication API Service
 * Handles login, register, and token management
 */

import { apiClient } from './client';
import type { LoginRequestDto, LoginResponseDto, RegisterRequestDto, UserDto } from '@/types/api';

const TOKEN_KEY = 'auth_token';

export const authService = {
  /**
   * Login user and store JWT token
   */
  login: async (credentials: LoginRequestDto): Promise<LoginResponseDto> => {
    const response = await apiClient.post<LoginResponseDto>(
      '/api/Auth/login',
      credentials
    );
    
    // Store token in localStorage
    if (response.token) {
      authService.setToken(response.token);
    }
    
    return response;
  },

  /**
   * Register new user
   */
  register: async (data: RegisterRequestDto): Promise<UserDto> => {
    return await apiClient.post<UserDto>('/api/Auth/register', data);
  },

  /**
   * Logout user (clear token)
   */
  logout: () => {
    authService.removeToken();
  },

  /**
   * Get stored JWT token
   */
  getToken: (): string | null => {
    return localStorage.getItem(TOKEN_KEY);
  },

  /**
   * Store JWT token in localStorage
   */
  setToken: (token: string): void => {
    localStorage.setItem(TOKEN_KEY, token);
  },

  /**
   * Remove JWT token from localStorage
   */
  removeToken: (): void => {
    localStorage.removeItem(TOKEN_KEY);
  },

  /**
   * Check if user is authenticated (has valid token)
   */
  isAuthenticated: (): boolean => {
    return authService.getToken() !== null;
  },
};
