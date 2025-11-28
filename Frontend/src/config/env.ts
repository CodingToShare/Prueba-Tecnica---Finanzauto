/**
 * Environment configuration
 * Reads values from .env files and provides type-safe access
 * In production (Docker), reads from window._env_ injected at runtime
 */

// TypeScript declaration for window._env_
declare global {
  interface Window {
    _env_?: {
      VITE_API_BASE_URL?: string;
    };
  }
}

export const config = {
  // In production Docker, use runtime env, otherwise use build-time env
  apiBaseUrl: window._env_?.VITE_API_BASE_URL || import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000',
  environment: import.meta.env.MODE || 'development',
  isDevelopment: import.meta.env.DEV,
  isProduction: import.meta.env.PROD,
} as const;

// Validate required environment variables
if (!config.apiBaseUrl) {
  console.error('VITE_API_BASE_URL is not defined in environment variables');
}

export default config;
