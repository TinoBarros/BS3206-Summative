using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components.Authorization;
using System.Text.RegularExpressions;

namespace Data {
    public class AuthService {
        private readonly AppDbContext _db;
        private readonly ILogger<AuthService> _logger;
        private readonly JwtService _jwtService;
        private readonly CustomAuthStateProvider _authStateProvider;

        public AuthService(
            AppDbContext db, 
            ILogger<AuthService> logger, 
            JwtService jwtService, 
            CustomAuthStateProvider authStateProvider) 
        { 
            _db = db;
            _logger = logger;
            _jwtService = jwtService;
            _authStateProvider = authStateProvider;
        }

        public async Task<(bool Success, string Message)> CreateAdminUserAsync() {
            try {
                const string adminEmail = "admin@admin.com";
                const string adminPassword = "admin123";

                // Check if admin exists
                if (await _db.Users.AnyAsync(u => u.Email == adminEmail)) {
                    return (true, $"Admin user already exists. Email: {adminEmail}, Password: {adminPassword}");
                }

                var adminUser = new User {
                    DisplayName = "Admin",
                    Email = adminEmail,
                    PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(adminPassword),
                    IsAdmin = true,
                };

                _db.Users.Add(adminUser);
                await _db.SaveChangesAsync();
                
                return (true, $"Admin user created successfully. Email: {adminEmail}, Password: {adminPassword}");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error creating admin user");
                return (false, "Failed to create admin user");
            }
        }

        public async Task<(bool Success, string Message, string? Token)> LoginAsync(string email, string password) {
            try {
                _logger.LogInformation($"Attempting to find user with email: {email}");
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                
                if (user == null) {
                    _logger.LogWarning($"No user found with email: {email}");
                    return (false, "Invalid email or password", null);
                }

                _logger.LogInformation($"User found. Verifying password");
                if (!BCrypt.Net.BCrypt.EnhancedVerify(password, user.PasswordHash)) {
                    _logger.LogWarning("Password verification failed");
                    return (false, "Invalid email or password", null);
                }

                var token = _jwtService.GenerateToken(user);
                _logger.LogInformation("Login successful");
                return (true, "Login successful", token);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error during login");
                return (false, "An error occurred during login", null);
            }
        }

        public async Task<(bool Success, string Message)> RegisterAsync(string displayName, string email, string password) {
            try {
                if (await _db.Users.AnyAsync(u => u.Email == email)) {
                    return (false, "Email already registered");
                }

                var user = new User {
                    DisplayName = displayName,
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(password)
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();
                return (true, "Registration successful");
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error during registration");
                return (false, "An error occurred during registration");
            }
        }

        public string? CheckUsername(string username)
        {
            if (username.Length < 3)
            {
                return "Username must be at least 3 characters";
            }

            var regex = new Regex("^[a-z0-9_]+$");
            if (!regex.IsMatch(username))
            {
                return "Username can only contain lowercase letters, numbers, and underscores _";
            }

            return null;
        }

        public async Task<bool> LogoutAsync()
        {
            await _authStateProvider.UpdateAuthenticationState(null);
            return true;
        }
    }
} 