using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Data {
    public class CustomAuthStateProvider : AuthenticationStateProvider {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProtectedSessionStorage _sessionStorage;
        private readonly JwtService _jwtService;
        private readonly ILogger<CustomAuthStateProvider> _logger;
        private readonly bool _isServerSide;
        private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
        private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

        public CustomAuthStateProvider(
            IHttpContextAccessor httpContextAccessor,
            ProtectedSessionStorage sessionStorage,
            JwtService jwtService,
            ILogger<CustomAuthStateProvider> logger) 
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionStorage = sessionStorage;
            _jwtService = jwtService;
            _logger = logger;
            _isServerSide = true;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync() {
            try {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var user = httpContext.User;
                    if (user.Identity?.IsAuthenticated == true)
                    {
                        _currentUser = user;
                        return new AuthenticationState(_currentUser);
                    }
                }

                return new AuthenticationState(_anonymous);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error getting authentication state");
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task UpdateAuthenticationState(string? token)
        {
            try
            {
                ClaimsPrincipal principal;
                
                if (string.IsNullOrEmpty(token))
                {
                    principal = _anonymous;
                    await SignOutAsync();
                }
                else
                {
                    principal = _jwtService.GetPrincipalFromToken(token) ?? _anonymous;
                    if (principal != _anonymous)
                    {
                        await SignInAsync(principal);
                        await _sessionStorage.SetAsync("token", token);
                    }
                }

                _currentUser = principal;
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication state update");
                throw;
            }
        }

        private async Task SignInAsync(ClaimsPrincipal principal)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                        AllowRefresh = true,
                        IssuedUtc = DateTimeOffset.UtcNow,
                        RedirectUri = httpContext.Request.Path.ToString()
                    };

                    await httpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        principal,
                        authProperties);

                    _currentUser = principal;
                }
                else
                {
                    _logger.LogWarning("No HttpContext available during sign in");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign in");
                throw;
            }
        }

        private async Task SignOutAsync()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }

                _currentUser = _anonymous;
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during sign out");
                throw;
            }
        }
    }
} 