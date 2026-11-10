/** @type {import("stylelint").Config} */
const config = {
  extends: ["stylelint-config-standard-scss"],
  rules: {
    // CSS module classes are read as `styles.fooBar`, so use camelCase.
    "selector-class-pattern": [
      "^[a-z][a-zA-Z0-9]*$",
      { message: "Expected class selector to be camelCase" },
    ],
    // Colours come from design tokens only (var(--color-…)).
    "color-no-hex": true,
    "color-named": "never",
    "function-disallowed-list": [
      ["rgb", "rgba", "hsl", "hsla", "hwb", "lab", "lch", "oklab", "oklch", "color"],
      { message: "Use a colour token from _tokens.scss instead of a raw colour" },
    ],
  },
  overrides: [
    {
      files: ["**/_tokens.scss"],
      rules: {
        "color-no-hex": null,
        "color-named": null,
        "function-disallowed-list": null,
      },
    },
  ],
};

export default config;
