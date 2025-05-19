// Stores global state across all pages such as the chosen theme.
using Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

public class Session
{
    /// <summary> The currently chosen theme, null represents no preference (use
    /// the system theme (SystemTheme)) </summary>
    public ThemeChoice? Theme = null;

    /// <summary>
    /// The theme detected from system preferences via media queries.
    /// This is updated by a Javascript event handler in MainLayout whenever a
    /// relevant media query value is updated.
    /// </summary>
    public ThemeChoice? SystemTheme = null;

    public User? CurrentUser = null;

    public enum ThemeChoice
    {
        Light,
        Dark,
        HighContrastLight,
        HighContrastDark,
    }

    public static string ThemeChoiceToClasses(ThemeChoice themeChoice)
    {
        return themeChoice switch
        {
            ThemeChoice.Light => "theme-light",
            ThemeChoice.Dark => "theme-dark",
            ThemeChoice.HighContrastLight => "theme-high-contrast-light high-contrast",
            ThemeChoice.HighContrastDark => "theme-high-contrast-dark high-contrast",
        };
    }

    public string ThemeClasses()
    {
        if (this.SystemTheme != null)
        {
            return ThemeChoiceToClasses((ThemeChoice)(Theme ?? SystemTheme));
        }
        else
        {
            return "";
        }
    }

    public async Task TrySetCurrentUser(AuthenticationStateProvider authStateProvider, AppDbContext dbContext)
    {
        if (CurrentUser != null)
        {
            return;
        }

        var authState = await authStateProvider.GetAuthenticationStateAsync();
        var userEmail = authState.User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(userEmail))
        {
            return;
        }

        var currentUser = await dbContext.Users
            .Where(u => u.Email == userEmail)
            .Include(u => u.Follows)
            .Include(u => u.Followers)
            .FirstOrDefaultAsync();
        if (currentUser == null)
            return;
        else
            CurrentUser = currentUser;
    }
}
