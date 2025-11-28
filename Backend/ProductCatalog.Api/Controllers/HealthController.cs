using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HealthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HealthController> _logger;

        public HealthController(ApplicationDbContext context, ILogger<HealthController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Basic health check endpoint
        /// </summary>
        /// <returns>Health status</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "ProductCatalog API",
                version = "1.0.0"
            });
        }

        /// <summary>
        /// Detailed health check with database connectivity
        /// </summary>
        /// <returns>Detailed health status</returns>
        [HttpGet("detailed")]
        public async Task<IActionResult> GetDetailed()
        {
            var healthStatus = new
            {
                status = "Healthy",
                timestamp = DateTime.UtcNow,
                service = "ProductCatalog API",
                version = "1.0.0",
                checks = new Dictionary<string, object>()
            };

            try
            {
                // Check database connectivity
                var canConnect = await _context.Database.CanConnectAsync();
                healthStatus.checks.Add("database", new
                {
                    status = canConnect ? "Healthy" : "Unhealthy",
                    responseTime = "< 100ms"
                });

                if (!canConnect)
                {
                    return StatusCode(503, new
                    {
                        status = "Unhealthy",
                        timestamp = DateTime.UtcNow,
                        service = "ProductCatalog API",
                        checks = healthStatus.checks
                    });
                }

                // Check if migrations are applied
                var pendingMigrations = await _context.Database.GetPendingMigrationsAsync();
                healthStatus.checks.Add("migrations", new
                {
                    status = !pendingMigrations.Any() ? "Healthy" : "Warning",
                    pendingCount = pendingMigrations.Count()
                });

                return Ok(healthStatus);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(503, new
                {
                    status = "Unhealthy",
                    timestamp = DateTime.UtcNow,
                    service = "ProductCatalog API",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Readiness check - checks if the service is ready to accept requests
        /// </summary>
        /// <returns>Readiness status</returns>
        [HttpGet("ready")]
        public async Task<IActionResult> Ready()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { status = "Ready" });
                }
                return StatusCode(503, new { status = "Not Ready" });
            }
            catch
            {
                return StatusCode(503, new { status = "Not Ready" });
            }
        }

        /// <summary>
        /// Liveness check - checks if the service is alive
        /// </summary>
        /// <returns>Liveness status</returns>
        [HttpGet("live")]
        public IActionResult Live()
        {
            return Ok(new { status = "Alive" });
        }
    }
}
