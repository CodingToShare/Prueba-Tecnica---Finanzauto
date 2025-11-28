import { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { productsService } from '@/api/products.service';
import { categoriesService } from '@/api/categories.service';
import type { ProductDto, CategoryDto, PagedProductsDto } from '@/types/api';
import styles from './ProductListPage.module.css';

export function ProductListPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const navigate = useNavigate();

  // State
  const [products, setProducts] = useState<ProductDto[]>([]);
  const [categories, setCategories] = useState<CategoryDto[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(0);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Filters from URL
  const page = parseInt(searchParams.get('page') || '1', 10);
  const pageSize = parseInt(searchParams.get('pageSize') || '20', 10);
  const categoryId = searchParams.get('categoryId') || '';
  const minPrice = searchParams.get('minPrice') || '';
  const maxPrice = searchParams.get('maxPrice') || '';
  const search = searchParams.get('search') || '';

  // Local filter state (before applying)
  const [localCategoryId, setLocalCategoryId] = useState(categoryId);
  const [localMinPrice, setLocalMinPrice] = useState(minPrice);
  const [localMaxPrice, setLocalMaxPrice] = useState(maxPrice);
  const [localSearch, setLocalSearch] = useState(search);

  // Fetch categories on mount
  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await categoriesService.getAll();
        setCategories(data);
      } catch (err) {
        console.error('Failed to fetch categories:', err);
      }
    };
    fetchCategories();
  }, []);

  // Fetch products when filters change
  useEffect(() => {
    const fetchProducts = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const params: {
          page: number;
          pageSize: number;
          categoryId?: number;
          minPrice?: number;
          maxPrice?: number;
          search?: string;
        } = {
          page,
          pageSize,
        };

        if (categoryId) params.categoryId = parseInt(categoryId, 10);
        if (minPrice) params.minPrice = parseFloat(minPrice);
        if (maxPrice) params.maxPrice = parseFloat(maxPrice);
        if (search) params.search = search;

        const data: PagedProductsDto = await productsService.getAll(params);
        setProducts(data.items);
        setTotalCount(data.totalCount);
        setTotalPages(data.totalPages);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch products');
      } finally {
        setIsLoading(false);
      }
    };

    fetchProducts();
  }, [page, pageSize, categoryId, minPrice, maxPrice, search]);

  // Apply filters
  const handleApplyFilters = () => {
    const params = new URLSearchParams();
    params.set('page', '1'); // Reset to first page
    params.set('pageSize', pageSize.toString());
    
    if (localCategoryId) params.set('categoryId', localCategoryId);
    if (localMinPrice) params.set('minPrice', localMinPrice);
    if (localMaxPrice) params.set('maxPrice', localMaxPrice);
    if (localSearch) params.set('search', localSearch);

    setSearchParams(params);
  };

  // Clear filters
  const handleClearFilters = () => {
    setLocalCategoryId('');
    setLocalMinPrice('');
    setLocalMaxPrice('');
    setLocalSearch('');
    setSearchParams({ page: '1', pageSize: pageSize.toString() });
  };

  // Change page
  const handlePageChange = (newPage: number) => {
    const params = new URLSearchParams(searchParams);
    params.set('page', newPage.toString());
    setSearchParams(params);
  };

  // Change page size
  const handlePageSizeChange = (newPageSize: number) => {
    const params = new URLSearchParams(searchParams);
    params.set('page', '1'); // Reset to first page
    params.set('pageSize', newPageSize.toString());
    setSearchParams(params);
  };

  // Navigate to edit page
  const handleEdit = (productId: number) => {
    navigate(`/products/${productId}/edit`);
  };

  // Format price
  const formatPrice = (price: number | null) => {
    if (price === null || price === undefined) return 'N/A';
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD',
    }).format(price);
  };

  // Render pagination buttons
  const renderPaginationButtons = () => {
    const buttons = [];
    const maxButtons = 5;
    let startPage = Math.max(1, page - Math.floor(maxButtons / 2));
    const endPage = Math.min(totalPages, startPage + maxButtons - 1);

    if (endPage - startPage + 1 < maxButtons) {
      startPage = Math.max(1, endPage - maxButtons + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      buttons.push(
        <button
          key={i}
          className={`${styles['pagination-button']} ${i === page ? styles.active : ''}`}
          onClick={() => handlePageChange(i)}
        >
          {i}
        </button>
      );
    }

    return buttons;
  };

  return (
    <div className={styles['product-list-page']}>
      <div className={styles['page-header']}>
        <h1 className={styles['page-title']}>Products</h1>
        <p className={styles['page-description']}>
          Browse and manage your product catalog
        </p>
      </div>

      {/* Filters */}
      <div className={styles['filters-section']}>
        <div className={styles['filters-grid']}>
          <div className={styles['filter-group']}>
            <label htmlFor="search" className={styles['filter-label']}>
              Search
            </label>
            <input
              id="search"
              type="text"
              className={styles['filter-input']}
              placeholder="Search by name..."
              value={localSearch}
              onChange={(e) => setLocalSearch(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleApplyFilters()}
            />
          </div>

          <div className={styles['filter-group']}>
            <label htmlFor="category" className={styles['filter-label']}>
              Category
            </label>
            <select
              id="category"
              className={styles['filter-select']}
              value={localCategoryId}
              onChange={(e) => setLocalCategoryId(e.target.value)}
            >
              <option value="">All Categories</option>
              {categories.map((cat) => (
                <option key={cat.categoryID} value={cat.categoryID}>
                  {cat.categoryName}
                </option>
              ))}
            </select>
          </div>

          <div className={styles['filter-group']}>
            <label htmlFor="minPrice" className={styles['filter-label']}>
              Min Price
            </label>
            <input
              id="minPrice"
              type="number"
              className={styles['filter-input']}
              placeholder="0"
              value={localMinPrice}
              onChange={(e) => setLocalMinPrice(e.target.value)}
              min="0"
              step="0.01"
            />
          </div>

          <div className={styles['filter-group']}>
            <label htmlFor="maxPrice" className={styles['filter-label']}>
              Max Price
            </label>
            <input
              id="maxPrice"
              type="number"
              className={styles['filter-input']}
              placeholder="999999"
              value={localMaxPrice}
              onChange={(e) => setLocalMaxPrice(e.target.value)}
              min="0"
              step="0.01"
            />
          </div>
        </div>

        <div className={styles['filter-actions']}>
          <button
            className={`${styles.btn} ${styles['btn-secondary']}`}
            onClick={handleClearFilters}
          >
            Clear
          </button>
          <button
            className={`${styles.btn} ${styles['btn-primary']}`}
            onClick={handleApplyFilters}
          >
            Apply Filters
          </button>
        </div>
      </div>

      {/* Results Info */}
      {!isLoading && !error && (
        <div className={styles['results-info']}>
          <span>
            Showing {products.length} of {totalCount} products
          </span>
        </div>
      )}

      {/* Loading State */}
      {isLoading && (
        <div className={styles['loading-container']}>
          <div className={styles['loading-spinner']} />
          <p>Loading products...</p>
        </div>
      )}

      {/* Error State */}
      {error && (
        <div className={styles['error-container']}>
          <div className={styles['error-message']}>{error}</div>
          <button
            className={`${styles.btn} ${styles['btn-primary']}`}
            onClick={() => window.location.reload()}
          >
            Retry
          </button>
        </div>
      )}

      {/* Empty State */}
      {!isLoading && !error && products.length === 0 && (
        <div className={styles['empty-container']}>
          <div className={styles['empty-icon']}>📦</div>
          <h2 className={styles['empty-title']}>No products found</h2>
          <p className={styles['empty-description']}>
            Try adjusting your filters or search terms
          </p>
          <button
            className={`${styles.btn} ${styles['btn-primary']}`}
            onClick={handleClearFilters}
          >
            Clear Filters
          </button>
        </div>
      )}

      {/* Products Table */}
      {!isLoading && !error && products.length > 0 && (
        <>
          <div className={styles['table-container']}>
            <table className={styles['products-table']}>
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Product Name</th>
                  <th>Category</th>
                  <th>Unit Price</th>
                  <th>Units In Stock</th>
                  <th>Status</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {products.map((product) => (
                  <tr key={product.productID}>
                    <td>{product.productID}</td>
                    <td className={styles['product-name']}>
                      {product.productName}
                    </td>
                    <td>{product.categoryName || 'N/A'}</td>
                    <td className={styles['product-price']}>
                      {formatPrice(product.unitPrice)}
                    </td>
                    <td>{product.unitsInStock ?? 'N/A'}</td>
                    <td>
                      {product.discontinued ? (
                        <span className={styles['product-discontinued']}>
                          Discontinued
                        </span>
                      ) : (
                        <span className={styles['product-active']}>Active</span>
                      )}
                    </td>
                    <td>
                      <div className={styles['table-actions']}>
                        <button
                          className={styles['btn-icon']}
                          onClick={() => handleEdit(product.productID)}
                          title="Edit product"
                        >
                          ✏️
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Pagination */}
          <div className={styles.pagination}>
            <div className={styles['pagination-info']}>
              Page {page} of {totalPages} ({totalCount} total)
            </div>

            <div className={styles['pagination-controls']}>
              <button
                className={styles['pagination-button']}
                onClick={() => handlePageChange(page - 1)}
                disabled={page === 1}
              >
                « Previous
              </button>

              {renderPaginationButtons()}

              <button
                className={styles['pagination-button']}
                onClick={() => handlePageChange(page + 1)}
                disabled={page === totalPages}
              >
                Next »
              </button>

              <div className={styles['page-size-selector']}>
                <label htmlFor="pageSize">Show:</label>
                <select
                  id="pageSize"
                  value={pageSize}
                  onChange={(e) =>
                    handlePageSizeChange(parseInt(e.target.value, 10))
                  }
                >
                  <option value="10">10</option>
                  <option value="20">20</option>
                  <option value="50">50</option>
                  <option value="100">100</option>
                </select>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
