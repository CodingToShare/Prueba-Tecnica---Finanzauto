/**
 * HTTP Client Configuration
 * Fetch-based client with JWT token injection and 401 error handling
 */

import { config } from '@/config/env';

const TOKEN_KEY = 'auth_token';

/**
 * Get token from localStorage
 */
function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

/**
 * Remove token from localStorage (used on 401)
 */
function removeToken(): void {
  localStorage.removeItem(TOKEN_KEY);
}

/**
 * Custom error class for API errors
 */
export class ApiError extends Error {
  status: number;
  statusText: string;
  data?: unknown;

  constructor(status: number, statusText: string, data?: unknown) {
    super(`API Error ${status}: ${statusText}`);
    this.name = 'ApiError';
    this.status = status;
    this.statusText = statusText;
    this.data = data;
  }
}

/**
 * Enhanced fetch wrapper with JWT token injection
 */
async function apiFetch<T>(
  endpoint: string,
  options: RequestInit = {}
): Promise<T> {
  const token = getToken();
  
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
  };

  // Merge existing headers
  if (options.headers) {
    const existingHeaders = new Headers(options.headers);
    existingHeaders.forEach((value, key) => {
      headers[key] = value;
    });
  }

  // Add Authorization header if token exists
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }

  const url = `${config.apiBaseUrl}${endpoint}`;

  try {
    const response = await fetch(url, {
      ...options,
      headers,
    });

    // Handle 401 Unauthorized - token expired or invalid
    if (response.status === 401) {
      removeToken();
      // Redirect to login page
      window.location.href = '/login';
      throw new ApiError(401, 'Unauthorized', 'Token expired or invalid');
    }

    // Handle other non-2xx responses
    if (!response.ok) {
      let errorData: unknown;
      try {
        errorData = await response.json();
      } catch {
        errorData = await response.text();
      }
      throw new ApiError(response.status, response.statusText, errorData);
    }

    // Handle 204 No Content
    if (response.status === 204) {
      return undefined as T;
    }

    // Parse JSON response
    return await response.json();
  } catch (error) {
    if (error instanceof ApiError) {
      throw error;
    }
    // Network or other fetch errors
    throw new Error(`Network error: ${error instanceof Error ? error.message : 'Unknown error'}`);
  }
}

/**
 * API Client with common HTTP methods
 */
export const apiClient = {
  get: <T>(endpoint: string, options?: RequestInit): Promise<T> => {
    return apiFetch<T>(endpoint, { ...options, method: 'GET' });
  },

  post: <T>(endpoint: string, data?: unknown, options?: RequestInit): Promise<T> => {
    return apiFetch<T>(endpoint, {
      ...options,
      method: 'POST',
      body: data ? JSON.stringify(data) : undefined,
    });
  },

  put: <T>(endpoint: string, data?: unknown, options?: RequestInit): Promise<T> => {
    return apiFetch<T>(endpoint, {
      ...options,
      method: 'PUT',
      body: data ? JSON.stringify(data) : undefined,
    });
  },

  delete: <T>(endpoint: string, options?: RequestInit): Promise<T> => {
    return apiFetch<T>(endpoint, { ...options, method: 'DELETE' });
  },
};
