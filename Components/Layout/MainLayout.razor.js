const colorQuery = window.matchMedia("(prefers-color-scheme: dark)");
const contrastQuery = window.matchMedia("(prefers-contrast: more)");

let mainLayoutRef;

export function init(mainLayoutRef_) {
    mainLayoutRef = mainLayoutRef_;

    colorQuery.onchange = onChange;
    contrastQuery.onchange = onChange;
    onChange();
}

function onChange() {
    console.log(`changed! ${colorQuery.matches} ${contrastQuery.matches}`);

    mainLayoutRef.invokeMethodAsync(
        "handleSystemThemeChanged",
        colorQuery.matches,
        contrastQuery.matches,
    );
}
