import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ProductForm } from '@/components/forms/ProductForm';
import { productsService } from '@/api/products.service';
import { categoriesService } from '@/api/categories.service';
import { suppliersService } from '@/api/suppliers.service';
import type { UpdateProductDto, CategoryDto, SupplierDto, ProductDetailDto } from '@/types/api';
import type { ProductFormData } from '@/components/forms/ProductForm';
import styles from './ProductEditPage.module.css';

export function ProductEditPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [product, setProduct] = useState<ProductDetailDto | null>(null);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [suppliers, setSuppliers] = useState<SupplierDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  // Fetch product, categories, and suppliers on mount
  useEffect(() => {
    const fetchData = async () => {
      if (!id) {
        setError('Product ID is required');
        setIsLoading(false);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        const [productData, categoriesData, suppliersData] = await Promise.all([
          productsService.getById(parseInt(id, 10)),
          categoriesService.getAll(),
          suppliersService.getAll(),
        ]);

        setProduct(productData);
        setCategories(categoriesData);
        setSuppliers(suppliersData);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to load product data');
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleSubmit = async (data: UpdateProductDto) => {
    if (!id) return;

    setIsSubmitting(true);
    setError(null);
    setSuccessMessage(null);

    try {
      await productsService.update(parseInt(id, 10), data);
      setSuccessMessage('Product updated successfully!');

      // Redirect to products list after 1.5 seconds
      setTimeout(() => {
        navigate('/products');
      }, 1500);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to update product');
      setIsSubmitting(false);
    }
  };

  const handleCancel = () => {
    navigate('/products');
  };

  if (isLoading) {
    return (
      <div className={styles['edit-page']}>
        <div className={styles['loading-container']}>
          <div className={styles['loading-spinner']} />
          <p>Loading product data...</p>
        </div>
      </div>
    );
  }

  if (error && !product) {
    return (
      <div className={styles['edit-page']}>
        <div className={styles['error-container']}>
          <div className={styles['error-message']}>{error}</div>
          <button
            className={`${styles.btn} ${styles['btn-primary']}`}
            onClick={() => navigate('/products')}
          >
            Back to Products
          </button>
        </div>
      </div>
    );
  }

  if (!product) {
    return null;
  }

  // Convert ProductDetailDto to ProductFormData
  const initialData: Partial<ProductFormData> = {
    productName: product.productName,
    supplierID: product.supplierID,
    categoryID: product.categoryID,
    quantityPerUnit: product.quantityPerUnit || '',
    unitPrice: product.unitPrice,
    unitsInStock: product.unitsInStock,
    unitsOnOrder: product.unitsOnOrder,
    reorderLevel: product.reorderLevel,
    discontinued: product.discontinued,
  };

  return (
    <div className={styles['edit-page']}>
      <div className={styles['page-header']}>
        <h1 className={styles['page-title']}>Edit Product #{id}</h1>
        <p className={styles['page-description']}>
          Update product information
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
          initialData={initialData}
          categories={categories}
          suppliers={suppliers}
          onSubmit={handleSubmit}
          onCancel={handleCancel}
          isSubmitting={isSubmitting}
          submitLabel="Update Product"
        />
      </div>
    </div>
  );
}
