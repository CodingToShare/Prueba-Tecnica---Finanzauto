import { useState, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/context/AuthContext';
import styles from './LoginPage.module.css';

export function LoginPage() {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  const { login, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  // Redirect if already authenticated
  if (isAuthenticated) {
    navigate('/products', { replace: true });
    return null;
  }

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError(null);

    // Client-side validation
    if (!username.trim()) {
      setError('Username is required');
      return;
    }

    if (username.length < 3) {
      setError('Username must be at least 3 characters');
      return;
    }

    if (!password) {
      setError('Password is required');
      return;
    }

    if (password.length < 6) {
      setError('Password must be at least 6 characters');
      return;
    }

    setIsLoading(true);

    try {
      await login(username, password);
      // Redirect to products page on success
      navigate('/products', { replace: true });
    } catch (err) {
      // Handle login error
      if (err instanceof Error) {
        setError(err.message || 'Login failed. Please check your credentials.');
      } else {
        setError('Login failed. Please check your credentials.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className={styles['login-container']}>
      <div className={styles['login-card']}>
        <div className={styles['login-header']}>
          <div className={styles['login-icon']}>🔐</div>
          <h1 className={styles['login-title']}>Welcome Back</h1>
          <p className={styles['login-subtitle']}>
            Sign in to access the Product Catalog
          </p>
        </div>

        {error && (
          <div className={styles['error-message']} role="alert">
            {error}
          </div>
        )}

        <form className={styles['login-form']} onSubmit={handleSubmit}>
          <div className={styles['form-group']}>
            <label htmlFor="username" className={styles['form-label']}>
              Username
            </label>
            <input
              id="username"
              type="text"
              className={styles['form-input']}
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              disabled={isLoading}
              placeholder="Enter your username"
              autoComplete="username"
              autoFocus
            />
          </div>

          <div className={styles['form-group']}>
            <label htmlFor="password" className={styles['form-label']}>
              Password
            </label>
            <input
              id="password"
              type="password"
              className={styles['form-input']}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={isLoading}
              placeholder="Enter your password"
              autoComplete="current-password"
            />
          </div>

          <button
            type="submit"
            className={styles['submit-button']}
            disabled={isLoading}
          >
            {isLoading ? (
              <>
                <span className={styles['loading-spinner']} />
                Signing in...
              </>
            ) : (
              'Sign In'
            )}
          </button>
        </form>

        <div className={styles['login-footer']}>
          <p>Finanzauto Product Catalog - Technical Test</p>
        </div>
      </div>
    </div>
  );
}
