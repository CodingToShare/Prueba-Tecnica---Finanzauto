import { Component, type ReactNode } from 'react';
import styles from './ErrorBoundary.module.css';

interface ErrorBoundaryProps {
  children: ReactNode;
  fallback?: ReactNode;
}

interface ErrorBoundaryState {
  hasError: boolean;
  error: Error | null;
}

export class ErrorBoundary extends Component<ErrorBoundaryProps, ErrorBoundaryState> {
  constructor(props: ErrorBoundaryProps) {
    super(props);
    this.state = { hasError: false, error: null };
  }

  static getDerivedStateFromError(error: Error): ErrorBoundaryState {
    return { hasError: true, error };
  }

  componentDidCatch(error: Error, errorInfo: { componentStack: string }) {
    console.error('ErrorBoundary caught an error:', error, errorInfo);
  }

  handleReset = () => {
    this.setState({ hasError: false, error: null });
    window.location.href = '/';
  };

  render() {
    if (this.state.hasError) {
      if (this.props.fallback) {
        return this.props.fallback;
      }

      return (
        <div className={styles['error-boundary']}>
          <div className={styles['error-content']}>
            <div className={styles['error-icon']}>⚠️</div>
            <h1 className={styles['error-title']}>Something went wrong</h1>
            <p className={styles['error-message']}>
              {this.state.error?.message || 'An unexpected error occurred'}
            </p>
            <div className={styles['error-actions']}>
              <button
                onClick={this.handleReset}
                className={styles['btn-primary']}
              >
                Go to Home
              </button>
              <button
                onClick={() => window.location.reload()}
                className={styles['btn-secondary']}
              >
                Reload Page
              </button>
            </div>
          </div>
        </div>
      );
    }

    return this.props.children;
  }
}
