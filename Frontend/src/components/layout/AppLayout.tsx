import { type ReactNode } from 'react';
import { Navbar } from './Navbar';
import styles from './AppLayout.module.css';

interface AppLayoutProps {
  children: ReactNode;
  isAuthenticated?: boolean;
  username?: string;
  onLogout?: () => void;
}

export function AppLayout({ children, isAuthenticated, username, onLogout }: AppLayoutProps) {
  return (
    <div className={styles['app-layout']}>
      <Navbar 
        isAuthenticated={isAuthenticated} 
        username={username} 
        onLogout={onLogout} 
      />
      <main className={styles['app-main']}>
        {children}
      </main>
      <footer className={styles['app-footer']}>
        <div className="container">
          © {new Date().getFullYear()} Product Catalog - Finanzauto Technical Test
        </div>
      </footer>
    </div>
  );
}
