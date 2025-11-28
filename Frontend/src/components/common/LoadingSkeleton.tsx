import styles from './LoadingSkeleton.module.css';

interface LoadingSkeletonProps {
  variant?: 'text' | 'circular' | 'rectangular';
  width?: string | number;
  height?: string | number;
  className?: string;
}

export function LoadingSkeleton({
  variant = 'text',
  width = '100%',
  height,
  className = '',
}: LoadingSkeletonProps) {
  const classNames = `${styles.skeleton} ${styles[variant]} ${className}`;
  
  // Use CSS custom properties for dynamic sizing without inline styles warning
  const dataAttributes = {
    'data-width': typeof width === 'number' ? `${width}px` : width,
    'data-height': typeof height === 'number' ? `${height}px` : height,
  };

  return <div className={classNames} {...dataAttributes} />;
}

export function TableSkeleton({ rows = 5 }: { rows?: number }) {
  return (
    <div className={styles['table-skeleton']}>
      <div className={styles['skeleton-header']}>
        {[...Array(7)].map((_, i) => (
          <LoadingSkeleton key={i} variant="rectangular" height={40} />
        ))}
      </div>
      {[...Array(rows)].map((_, rowIndex) => (
        <div key={rowIndex} className={styles['skeleton-row']}>
          {[...Array(7)].map((_, colIndex) => (
            <LoadingSkeleton key={colIndex} variant="text" height={24} />
          ))}
        </div>
      ))}
    </div>
  );
}

export function FormSkeleton() {
  return (
    <div className={styles['form-skeleton']}>
      {[...Array(8)].map((_, i) => (
        <div key={i} className={styles['skeleton-field']}>
          <LoadingSkeleton variant="text" width="30%" height={20} />
          <LoadingSkeleton variant="rectangular" height={40} />
        </div>
      ))}
      <div className={styles['skeleton-actions']}>
        <LoadingSkeleton variant="rectangular" width={120} height={40} />
        <LoadingSkeleton variant="rectangular" width={120} height={40} />
      </div>
    </div>
  );
}
