import { useEffect, useState } from 'react';
import type { ToastType } from '@/context/ToastContext';
import styles from './Toast.module.css';

interface ToastProps {
  message: string;
  type: ToastType;
  onClose: () => void;
}

const TOAST_ICONS = {
  success: '✓',
  error: '✕',
  info: 'ℹ',
  warning: '⚠',
};

export function Toast({ message, type, onClose }: ToastProps) {
  const [isVisible, setIsVisible] = useState(false);

  useEffect(() => {
    // Trigger animation
    requestAnimationFrame(() => {
      setIsVisible(true);
    });
  }, []);

  const handleClose = () => {
    setIsVisible(false);
    setTimeout(onClose, 300); // Wait for animation to complete
  };

  return (
    <div
      className={`${styles.toast} ${styles[type]} ${isVisible ? styles.visible : ''}`}
      role="alert"
    >
      <div className={styles['toast-icon']}>{TOAST_ICONS[type]}</div>
      <div className={styles['toast-message']}>{message}</div>
      <button
        className={styles['toast-close']}
        onClick={handleClose}
        aria-label="Close notification"
      >
        ×
      </button>
    </div>
  );
}
