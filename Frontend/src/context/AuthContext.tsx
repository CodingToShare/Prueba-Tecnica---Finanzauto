/**
 * Authentication Context
 * Manages global auth state and provides login/logout methods
 */

import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { authService } from '@/api/auth.service';
import type { LoginResponseDto } from '@/types/api';

interface AuthUser {
  username: string;
  email: string;
  role: string;
}

interface AuthContextType {
  isAuthenticated: boolean;
  user: AuthUser | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
  isLoading: boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  // Initialize auth state on mount
  useEffect(() => {
    const initAuth = () => {
      const token = authService.getToken();
      
      if (token) {
        // Token exists, consider user authenticated
        // In a real app, you might want to validate the token with the backend
        // For now, we'll just check if token exists
        // User data would ideally come from token decode or a separate API call
        setUser({
          username: 'User', // Placeholder - would come from token or API
          email: '',
          role: 'User',
        });
      }
      
      setIsLoading(false);
    };

    initAuth();
  }, []);

  const login = async (username: string, password: string): Promise<void> => {
    try {
      const response: LoginResponseDto = await authService.login({
        username,
        password,
      });

      // Set user data from login response
      setUser({
        username: response.username,
        email: response.email,
        role: response.role,
      });
    } catch (error) {
      // Clean up any partial state
      setUser(null);
      authService.removeToken();
      throw error;
    }
  };

  const logout = (): void => {
    authService.logout();
    setUser(null);
    // Redirect to login page
    window.location.href = '/login';
  };

  const value: AuthContextType = {
    isAuthenticated: user !== null,
    user,
    login,
    logout,
    isLoading,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextType {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
