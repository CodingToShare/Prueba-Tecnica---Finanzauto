using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated and filtered list of products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? search = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100; // Max page size

            var result = await _productService.GetProductsAsync(page, pageSize, categoryId, minPrice, maxPrice, search);
            return Ok(result);
        }

        /// <summary>
        /// Get product by ID with full details
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDetailDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }

        /// <summary>
        /// Generate and insert N random products in bulk (optimized for large datasets)
        /// </summary>
        [HttpPost("bulk")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<BulkInsertResultDto>> BulkGenerateProducts([FromBody] BulkGenerateProductsDto request)
        {
            if (request.Count < 1 || request.Count > 100000)
            {
                return BadRequest(new { message = "Count must be between 1 and 100,000" });
            }

            _logger.LogInformation("Starting bulk generation of {Count} products", request.Count);

            var result = await _productService.BulkGenerateProductsAsync(request.Count);

            _logger.LogInformation("Successfully generated {Count} products in {ElapsedMs}ms",
                result.ProductsCreated, result.ElapsedMilliseconds);

            return Ok(result);
        }

        /// <summary>
        /// Update an existing product
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductDto updateDto)
        {
            var product = await _productService.UpdateProductAsync(id, updateDto);
            if (product == null)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return Ok(product);
        }

        /// <summary>
        /// Delete a product (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Product with ID {id} not found" });
            }

            return NoContent();
        }
    }
}
