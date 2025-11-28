import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import type { UpdateProductDto, CategoryDto, SupplierDto } from '@/types/api';
import styles from './ProductForm.module.css';

export interface ProductFormData {
  productName: string;
  supplierID: number | null;
  categoryID: number | null;
  quantityPerUnit: string;
  unitPrice: number | null;
  unitsInStock: number | null;
  unitsOnOrder: number | null;
  reorderLevel: number | null;
  discontinued: boolean;
}

interface ProductFormProps {
  initialData?: Partial<ProductFormData>;
  categories: CategoryDto[];
  suppliers: SupplierDto[];
  onSubmit: (data: UpdateProductDto) => void;
  onCancel: () => void;
  isSubmitting?: boolean;
  submitLabel?: string;
}

export function ProductForm({
  initialData,
  categories,
  suppliers,
  onSubmit,
  onCancel,
  isSubmitting = false,
  submitLabel = 'Save Product',
}: ProductFormProps) {
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<ProductFormData>({
    defaultValues: {
      productName: '',
      supplierID: null,
      categoryID: null,
      quantityPerUnit: '',
      unitPrice: null,
      unitsInStock: null,
      unitsOnOrder: null,
      reorderLevel: null,
      discontinued: false,
      ...initialData,
    },
  });

  // Reset form when initialData changes (for edit mode)
  useEffect(() => {
    if (initialData) {
      reset({
        productName: initialData.productName || '',
        supplierID: initialData.supplierID ?? null,
        categoryID: initialData.categoryID ?? null,
        quantityPerUnit: initialData.quantityPerUnit || '',
        unitPrice: initialData.unitPrice ?? null,
        unitsInStock: initialData.unitsInStock ?? null,
        unitsOnOrder: initialData.unitsOnOrder ?? null,
        reorderLevel: initialData.reorderLevel ?? null,
        discontinued: initialData.discontinued ?? false,
      });
    }
  }, [initialData, reset]);

  const handleFormSubmit = (data: ProductFormData) => {
    // Convert form data to UpdateProductDto
    const dto: UpdateProductDto = {
      productName: data.productName,
      supplierID: data.supplierID,
      categoryID: data.categoryID,
      quantityPerUnit: data.quantityPerUnit || null,
      unitPrice: data.unitPrice,
      unitsInStock: data.unitsInStock ?? 0,
      unitsOnOrder: data.unitsOnOrder ?? 0,
      reorderLevel: data.reorderLevel ?? 0,
      discontinued: data.discontinued,
    };

    onSubmit(dto);
  };

  return (
    <form onSubmit={handleSubmit(handleFormSubmit)} className={styles['product-form']}>
      <div className={styles['form-grid']}>
        {/* Product Name */}
        <div className={styles['form-group']}>
          <label htmlFor="productName" className={styles['form-label']}>
            Product Name <span className={styles.required}>*</span>
          </label>
          <input
            id="productName"
            type="text"
            className={`${styles['form-input']} ${errors.productName ? styles.error : ''}`}
            {...register('productName', {
              required: 'Product name is required',
              minLength: {
                value: 2,
                message: 'Product name must be at least 2 characters',
              },
              maxLength: {
                value: 40,
                message: 'Product name must not exceed 40 characters',
              },
            })}
            disabled={isSubmitting}
          />
          {errors.productName && (
            <span className={styles['error-message']}>{errors.productName.message}</span>
          )}
        </div>

        {/* Supplier */}
        <div className={styles['form-group']}>
          <label htmlFor="supplierID" className={styles['form-label']}>
            Supplier
          </label>
          <select
            id="supplierID"
            className={`${styles['form-select']} ${errors.supplierID ? styles.error : ''}`}
            {...register('supplierID', {
              setValueAs: (v) => (v === '' ? null : parseInt(v, 10)),
            })}
            disabled={isSubmitting}
          >
            <option value="">Select a supplier</option>
            {suppliers.map((supplier) => (
              <option key={supplier.supplierID} value={supplier.supplierID}>
                {supplier.companyName}
              </option>
            ))}
          </select>
          {errors.supplierID && (
            <span className={styles['error-message']}>{errors.supplierID.message}</span>
          )}
        </div>

        {/* Category */}
        <div className={styles['form-group']}>
          <label htmlFor="categoryID" className={styles['form-label']}>
            Category
          </label>
          <select
            id="categoryID"
            className={`${styles['form-select']} ${errors.categoryID ? styles.error : ''}`}
            {...register('categoryID', {
              setValueAs: (v) => (v === '' ? null : parseInt(v, 10)),
            })}
            disabled={isSubmitting}
          >
            <option value="">Select a category</option>
            {categories.map((category) => (
              <option key={category.categoryID} value={category.categoryID}>
                {category.categoryName}
              </option>
            ))}
          </select>
          {errors.categoryID && (
            <span className={styles['error-message']}>{errors.categoryID.message}</span>
          )}
        </div>

        {/* Quantity Per Unit */}
        <div className={styles['form-group']}>
          <label htmlFor="quantityPerUnit" className={styles['form-label']}>
            Quantity Per Unit
          </label>
          <input
            id="quantityPerUnit"
            type="text"
            className={`${styles['form-input']} ${errors.quantityPerUnit ? styles.error : ''}`}
            placeholder="e.g., 10 boxes x 20 bags"
            {...register('quantityPerUnit', {
              maxLength: {
                value: 20,
                message: 'Quantity per unit must not exceed 20 characters',
              },
            })}
            disabled={isSubmitting}
          />
          {errors.quantityPerUnit && (
            <span className={styles['error-message']}>{errors.quantityPerUnit.message}</span>
          )}
        </div>

        {/* Unit Price */}
        <div className={styles['form-group']}>
          <label htmlFor="unitPrice" className={styles['form-label']}>
            Unit Price
          </label>
          <input
            id="unitPrice"
            type="number"
            step="0.01"
            min="0"
            className={`${styles['form-input']} ${errors.unitPrice ? styles.error : ''}`}
            {...register('unitPrice', {
              setValueAs: (v) => (v === '' ? null : parseFloat(v)),
              min: {
                value: 0,
                message: 'Unit price must be greater than or equal to 0',
              },
            })}
            disabled={isSubmitting}
          />
          {errors.unitPrice && (
            <span className={styles['error-message']}>{errors.unitPrice.message}</span>
          )}
        </div>

        {/* Units In Stock */}
        <div className={styles['form-group']}>
          <label htmlFor="unitsInStock" className={styles['form-label']}>
            Units In Stock
          </label>
          <input
            id="unitsInStock"
            type="number"
            min="0"
            className={`${styles['form-input']} ${errors.unitsInStock ? styles.error : ''}`}
            {...register('unitsInStock', {
              setValueAs: (v) => (v === '' ? null : parseInt(v, 10)),
              min: {
                value: 0,
                message: 'Units in stock must be greater than or equal to 0',
              },
            })}
            disabled={isSubmitting}
          />
          {errors.unitsInStock && (
            <span className={styles['error-message']}>{errors.unitsInStock.message}</span>
          )}
        </div>

        {/* Units On Order */}
        <div className={styles['form-group']}>
          <label htmlFor="unitsOnOrder" className={styles['form-label']}>
            Units On Order
          </label>
          <input
            id="unitsOnOrder"
            type="number"
            min="0"
            className={`${styles['form-input']} ${errors.unitsOnOrder ? styles.error : ''}`}
            {...register('unitsOnOrder', {
              setValueAs: (v) => (v === '' ? null : parseInt(v, 10)),
              min: {
                value: 0,
                message: 'Units on order must be greater than or equal to 0',
              },
            })}
            disabled={isSubmitting}
          />
          {errors.unitsOnOrder && (
            <span className={styles['error-message']}>{errors.unitsOnOrder.message}</span>
          )}
        </div>

        {/* Reorder Level */}
        <div className={styles['form-group']}>
          <label htmlFor="reorderLevel" className={styles['form-label']}>
            Reorder Level
          </label>
          <input
            id="reorderLevel"
            type="number"
            min="0"
            className={`${styles['form-input']} ${errors.reorderLevel ? styles.error : ''}`}
            {...register('reorderLevel', {
              setValueAs: (v) => (v === '' ? null : parseInt(v, 10)),
              min: {
                value: 0,
                message: 'Reorder level must be greater than or equal to 0',
              },
            })}
            disabled={isSubmitting}
          />
          {errors.reorderLevel && (
            <span className={styles['error-message']}>{errors.reorderLevel.message}</span>
          )}
        </div>
      </div>

      {/* Discontinued Checkbox */}
      <div className={styles['form-group-checkbox']}>
        <input
          id="discontinued"
          type="checkbox"
          className={styles['form-checkbox']}
          {...register('discontinued')}
          disabled={isSubmitting}
        />
        <label htmlFor="discontinued" className={styles['form-label-checkbox']}>
          Discontinued
        </label>
      </div>

      {/* Form Actions */}
      <div className={styles['form-actions']}>
        <button
          type="button"
          className={`${styles.btn} ${styles['btn-secondary']}`}
          onClick={onCancel}
          disabled={isSubmitting}
        >
          Cancel
        </button>
        <button
          type="submit"
          className={`${styles.btn} ${styles['btn-primary']}`}
          disabled={isSubmitting}
        >
          {isSubmitting ? 'Saving...' : submitLabel}
        </button>
      </div>
    </form>
  );
}
