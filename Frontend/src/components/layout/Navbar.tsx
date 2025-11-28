import { Link, NavLink } from 'react-router-dom';
import styles from './Navbar.module.css';

interface NavbarProps {
  isAuthenticated?: boolean;
  username?: string;
  onLogout?: () => void;
}

export function Navbar({ isAuthenticated = false, username, onLogout }: NavbarProps) {
  return (
    <nav className={styles.navbar}>
      <div className={`container ${styles['navbar-container']}`}>
        <Link to="/" className={styles['navbar-brand']}>
          📦 Product Catalog
        </Link>

        {isAuthenticated && (
          <>
            <ul className={styles['navbar-nav']}>
              <li>
                <NavLink
                  to="/products"
                  className={({ isActive }) =>
                    `${styles['navbar-link']} ${isActive ? styles.active : ''}`
                  }
                >
                  Products
                </NavLink>
              </li>
              <li>
                <NavLink
                  to="/products/create"
                  className={({ isActive }) =>
                    `${styles['navbar-link']} ${isActive ? styles.active : ''}`
                  }
                >
                  Create Product
                </NavLink>
              </li>
            </ul>

            <div className={styles['navbar-actions']}>
              {username && (
                <div className={styles['navbar-user']}>
                  <span>👤</span>
                  <span>{username}</span>
                </div>
              )}
              <button
                onClick={onLogout}
                className={styles['navbar-logout-btn']}
                aria-label="Logout"
              >
                Logout
              </button>
            </div>
          </>
        )}
      </div>
    </nav>
  );
}
