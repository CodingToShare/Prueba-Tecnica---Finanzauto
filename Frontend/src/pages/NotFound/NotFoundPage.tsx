import { useNavigate, useLocation } from 'react-router-dom';
import styles from './NotFoundPage.module.css';

export function NotFoundPage() {
  const navigate = useNavigate();
  const location = useLocation();

  return (
    <div className={styles['not-found-page']}>
      <div className={styles['not-found-content']}>
        <div className={styles['error-code']}>404</div>
        <h1 className={styles['error-title']}>Page Not Found</h1>
        <p className={styles['error-message']}>
          The page <code>{location.pathname}</code> doesn't exist or has been moved.
        </p>
        
        <div className={styles['error-actions']}>
          <button
            onClick={() => navigate('/')}
            className={`${styles.btn} ${styles['btn-primary']}`}
          >
            Go to Home
          </button>
          <button
            onClick={() => navigate(-1)}
            className={`${styles.btn} ${styles['btn-secondary']}`}
          >
            Go Back
          </button>
        </div>

        <div className={styles['help-text']}>
          <p>Need help? Here are some useful links:</p>
          <ul>
            <li>
              <a href="/products" className={styles.link}>Browse Products</a>
            </li>
            <li>
              <a href="/login" className={styles.link}>Login</a>
            </li>
          </ul>
        </div>
      </div>
    </div>
  );
}
