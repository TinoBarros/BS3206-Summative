# BS3206 Summative

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


dotnet watch # Run with live reload, for development.
dotnet run   # Run normally.
```

### Compiling SASS to CSS

```bash
sass wwwroot/app.scss:wwwroot/app.css # compile SASS to CSS
```

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
