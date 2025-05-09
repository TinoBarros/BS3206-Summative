# BS3206 Summative

## Recent Updates

The following improvements have been made to the application:

1. **Fixed Layout Issues**
   - Sidebar navigation now only appears on authenticated pages
   - Sidebar is properly positioned and centered in its container
   - Content areas have consistent margins and padding

2. **Authentication Improvements**
   - User profile now shows the currently logged-in user's information
   - Multiple user accounts are supported
   - Consistent login/registration pages with improved styling
   - Logout functionality works correctly

3. **UI Styling Enhancements**
   - Dark theme applied consistently across all pages - this can be changes once settings has been introduced as we can have a toggle or something.
   - Form fields have consistent styling
   - Improved error messages and validation

## How to use

### Prerequisites

- `dotnet` CLI and relevant SDKs for Blazor
    - TODO install guide for this?
- SASS
    - Only required if modifying `wwwroot/app.scss`
    - Easiest way to install is to install node.js and NPM, then run the following:

    ```
    npm install -g sass
    ```

    On linux or macOS you might need to run with `sudo`.

### Running

```bash
cd BS3206-Summative # replace with the directory you cloned this repository to

dotnet restore
dotnet build 
dotnet watch # Run with live reload, for development.
dotnet run   # Run normally.
```

### Compiling SASS to CSS

```bash
sass wwwroot/app.scss:wwwroot/app.css # compile SASS to CSS
```

## Default Login Credentials

For testing purposes, several accounts are pre-seeded in the database:

- **Admin Account**: 
  - Email: admin@admin.com
  - Password: admin123

  **More Accounts**
  - You can sign up using the register screen for more accounts if you need them for testing

## Project structure

- The base template for every page is at `Components/App.razor`
    - This contains the basic HTML template.
    - The global CSS file (i.e. for every page) is `wwwroot/app.scss`
        - Note that this is in [SASS SCSS format](https://sass-lang.com/) and needs to be compile with the command in the `Compiling SASS to CSS` section when changed.
    - The favicon is `wwwroot/favicon.png`
- The main page layout is at `Components/Layout/MainLayout.razor`
    - This contains things that should show on every page like the project logo, name, and containers for the content
- The individual pages are found at `Components/Pages/*.razor`
    - Page-specific CSS can be defined in a `*.razor.css` file with the same filename as the `*.razor` file for the page

## Authentication System

The application uses a cookie-based authentication system:

1. **Login Process**:
   - User credentials are validated against database records
   - JWT token is generated and user claims are stored in cookies
   - Authenticated users are redirected to the Feed page

2. **User Profiles**:
   - User profiles are stored in the SQLite database
   - Profile images can be uploaded and stored
   - User data can be edited through the profile page

3. **Logout**:
   - Clicking logout clears authentication cookies
   - Users are redirected to the login page

### Database

There is a temp database introduced, this will be migrated once ours is fully working, feel free to look into migrations.