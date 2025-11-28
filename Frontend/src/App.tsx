import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { ToastProvider } from './context/ToastContext';
import { ErrorBoundary } from './components/common/ErrorBoundary';
import { ProtectedRoute } from './guards/ProtectedRoute';
import { AppLayout } from './components/layout/AppLayout';
import { LoginPage } from './pages/Login/LoginPage';
import { ProductListPage } from './pages/Products/List/ProductListPage';
import { ProductCreatePage } from './pages/Products/Create/ProductCreatePage';
import { ProductEditPage } from './pages/Products/Edit/ProductEditPage';
import { NotFoundPage } from './pages/NotFound/NotFoundPage';
import './styles/global.css';

function AppRoutes() {
  const { isAuthenticated, user, logout } = useAuth();

  return (
    <AppLayout 
      isAuthenticated={isAuthenticated} 
      username={user?.username}
      onLogout={logout}
    >
      <Routes>
        {/* Public Routes */}
        <Route path="/login" element={<LoginPage />} />

        {/* Protected Routes */}
        <Route path="/" element={<Navigate to="/products" replace />} />
        <Route
          path="/products"
          element={
            <ProtectedRoute>
              <ProductListPage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/products/create"
          element={
            <ProtectedRoute>
              <ProductCreatePage />
            </ProtectedRoute>
          }
        />
        <Route
          path="/products/:id/edit"
          element={
            <ProtectedRoute>
              <ProductEditPage />
            </ProtectedRoute>
          }
        />

        {/* 404 Not Found */}
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </AppLayout>
  );
}

function App() {
  return (
    <ErrorBoundary>
      <BrowserRouter>
        <AuthProvider>
          <ToastProvider>
            <AppRoutes />
          </ToastProvider>
        </AuthProvider>
      </BrowserRouter>
    </ErrorBoundary>
  );
}

export default App;
