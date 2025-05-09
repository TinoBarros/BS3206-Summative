using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Data;
using System.Text.Json.Serialization;
using bs3206.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add support for controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

// Add MVC services including TempData
builder.Services.AddMvc();
builder.Services.AddSingleton<Microsoft.AspNetCore.Mvc.ViewFeatures.ITempDataDictionaryFactory, Microsoft.AspNetCore.Mvc.ViewFeatures.TempDataDictionaryFactory>();

// Add HttpClient services
builder.Services.AddHttpClient();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

// Configure JWT
var jwtKey = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).PadRight(32, '=');
builder.Services.AddSingleton<JwtService>(new JwtService(jwtKey));

// Configure authentication with more secure settings
builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(options => {
    options.Cookie.Name = "health_app_auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.LoginPath = "/login";
    options.LogoutPath = "/logout";
    options.AccessDeniedPath = "/access-denied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    
    // Ensure cookies are accessible to all paths in the application
    options.Cookie.Path = "/";
    
    // Proper redirect handling
    options.Events.OnRedirectToLogin = context =>
    {
        // Do not redirect API calls (return 401 instead)
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        }
        
        // For browser requests, redirect to login page
        if (!context.Response.HasStarted)
        {
            context.Response.Redirect(context.RedirectUri);
        }
        
        return Task.CompletedTask;
    };
    
    options.Events.OnRedirectToAccessDenied = context =>
    {
        // Do not redirect API calls (return 403 instead)
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        }
        
        // For browser requests, redirect to access denied page
        if (!context.Response.HasStarted)
        {
            context.Response.Redirect(context.RedirectUri);
        }
        
        return Task.CompletedTask;
    };
});

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = null;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ProtectedSessionStorage>();

// Configure core services
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// Configure authentication services in correct order
builder.Services.AddScoped<CustomAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthStateProvider>());
builder.Services.AddScoped<AuthService>();

var app = builder.Build();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        
        // Create admin user if it doesn't exist
        var authService = services.GetRequiredService<AuthService>();
        await authService.CreateAdminUserAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

// Serve static files before authentication
app.UseStaticFiles();

// Add antiforgery middleware
app.UseAntiforgery();

// Add authentication and authorization - important to have before routing
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map Razor components and configure routes
app.MapRazorComponents<bs3206.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();
