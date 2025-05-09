using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.Tasks;
using Data;
using Microsoft.Extensions.Logging;

namespace bs3206.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            AuthService authService, 
            JwtService jwtService, 
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password, [FromForm] string returnUrl = "/feed")
        {
            _logger.LogInformation("Form-based login endpoint called for email: {Email}", email);
            
            try
            {
                var result = await _authService.LoginAsync(email, password);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Login failed: {Message}", result.Message);
                    return Redirect($"/login?error={Uri.EscapeDataString(result.Message)}");
                }

                // Generate claims from the token
                var principal = _jwtService.GetPrincipalFromToken(result.Token);
                if (principal == null)
                {
                    _logger.LogError("Failed to generate principal from token");
                    return Redirect("/login?error=Authentication+failed");
                }

                // Set authentication cookie
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow,
                    RedirectUri = returnUrl
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                _logger.LogInformation("Login successful for user {Email}, redirecting to {ReturnUrl}", email, returnUrl);
                
                // Clean return URL - handle escaped slashes if needed
                if (returnUrl.StartsWith("%2F"))
                {
                    returnUrl = "/" + returnUrl.Substring(3);
                }
                
                // Do a proper HTTP redirect to the return URL
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login API call");
                return Redirect("/login?error=Internal+server+error");
            }
        }
        
        // Keep the JSON API endpoint for potential client-side usage
        [HttpPost("login-api")]
        public async Task<IActionResult> LoginApi([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login API endpoint called for email: {Email}", request.Email);
            
            try
            {
                var result = await _authService.LoginAsync(request.Email, request.Password);
                
                if (!result.Success)
                {
                    _logger.LogWarning("Login failed: {Message}", result.Message);
                    return BadRequest(new { Success = false, Message = result.Message });
                }

                // Generate claims from the token
                var principal = _jwtService.GetPrincipalFromToken(result.Token);
                if (principal == null)
                {
                    _logger.LogError("Failed to generate principal from token");
                    return BadRequest(new { Success = false, Message = "Authentication failed" });
                }

                // Set authentication cookie
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                    AllowRefresh = true,
                    IssuedUtc = DateTimeOffset.UtcNow
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    authProperties);

                _logger.LogInformation("Login successful for user {Email}", request.Email);
                
                return Ok(new { Success = true, Token = result.Token, RedirectTo = "/feed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login API call");
                return StatusCode(500, new { Success = false, Message = "Internal server error" });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/login");
        }

        public class LoginRequest
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }
    }
} 