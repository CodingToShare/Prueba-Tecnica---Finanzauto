using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Application.DTOs;
using ProductCatalog.Application.Interfaces;

namespace ProductCatalog.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Login with username and password to receive JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input", errors = ModelState });
            }

            try
            {
                var response = await _authService.LoginAsync(loginRequest);

                if (response == null)
                {
                    _logger.LogWarning("Failed login attempt for username: {Username}", loginRequest.Username);
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                _logger.LogInformation("User {Username} logged in successfully", loginRequest.Username);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for username: {Username}", loginRequest.Username);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }

        /// <summary>
        /// Register a new user (Admin only)
        /// </summary>
        [HttpPost("register")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid input", errors = ModelState });
            }

            try
            {
                var user = await _authService.RegisterAsync(registerRequest);
                _logger.LogInformation("New user registered: {Username} with role {Role}", user.Username, user.Role);
                return CreatedAtAction(nameof(Register), new { id = user.UserID }, user);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed for username: {Username}", registerRequest.Username);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for username: {Username}", registerRequest.Username);
                return StatusCode(500, new { message = "An error occurred during registration" });
            }
        }
    }
}
