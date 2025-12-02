using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Domain.Interfaces;

namespace ProductCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(ISupplierRepository supplierRepository, ILogger<SuppliersController> logger)
        {
            _supplierRepository = supplierRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get all suppliers
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDto>>> GetSuppliers()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            
            var supplierDtos = suppliers.Select(s => new SupplierDto
            {
                SupplierID = s.SupplierID,
                CompanyName = s.CompanyName,
                ContactName = s.ContactName,
                City = s.City,
                Country = s.Country,
                Phone = s.Phone
            }).ToList();

            return Ok(supplierDtos);
        }

        /// <summary>
        /// Get supplier by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierDto>> GetSupplier(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null)
            {
                return NotFound(new { message = $"Supplier with ID {id} not found" });
            }

            var supplierDto = new SupplierDto
            {
                SupplierID = supplier.SupplierID,
                CompanyName = supplier.CompanyName,
                ContactName = supplier.ContactName,
                City = supplier.City,
                Country = supplier.Country,
                Phone = supplier.Phone
            };

            return Ok(supplierDto);
        }
    }
}
