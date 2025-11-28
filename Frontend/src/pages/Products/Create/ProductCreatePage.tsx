import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { ProductForm } from '@/components/forms/ProductForm';
import { productsService } from '@/api/products.service';
import { categoriesService } from '@/api/categories.service';
import { suppliersService } from '@/api/suppliers.service';
import type { UpdateProductDto, CategoryDto, SupplierDto } from '@/types/api';
import styles from './ProductCreatePage.module.css';

export function ProductCreatePage() {
  const navigate = useNavigate();
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [suppliers, setSuppliers] = useState<SupplierDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  // Fetch categories and suppliers on mount
  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const [categoriesData, suppliersData] = await Promise.all([
          categoriesService.getAll(),
          suppliersService.getAll(),
        ]);

        setCategories(categoriesData);
        setSuppliers(suppliersData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load form data');
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, []);

  const handleSubmit = async (data: UpdateProductDto) => {
    setIsSubmitting(true);
    setError(null);
    setSuccessMessage(null);

    try {
      await productsService.create(data);
      setSuccessMessage('Product created successfully!');

      // Redirect to products list after 1.5 seconds
      setTimeout(() => {
        navigate('/products');
      }, 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to create product');
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    navigate('/products');
  };

  if (isLoading) {
    return (
      <div className={styles['create-page']}>
        <div className={styles['loading-container']}>
          <div className={styles['loading-spinner']} />
          <p>Loading form data...</p>
        </div>
      </div>
    );
  }

  if (error && !categories.length && !suppliers.length) {
    return (
      <div className={styles['create-page']}>
        <div className={styles['error-container']}>
          <div className={styles['error-message']}>{error}</div>
          <button
            className={`${styles.btn} ${styles['btn-primary']}`}
            onClick={() => window.location.reload()}
          >
            Retry
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className={styles['create-page']}>
      <div className={styles['page-header']}>
        <h1 className={styles['page-title']}>Create New Product</h1>
        <p className={styles['page-description']}>
          Add a new product to your catalog
        </p>
      </div>

      {successMessage && (
        <div className={styles['success-message']} role="alert">
          ✓ {successMessage}
        </div>
      )}

      {error && (
        <div className={styles['error-banner']} role="alert">
          {error}
        </div>
      )}

      <div className={styles['form-container']}>
        <ProductForm
          categories={categories}
          suppliers={suppliers}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          isSubmitting={isSubmitting}
          submitLabel="Create Product"
        />
      </div>
    </div>
  );
}
