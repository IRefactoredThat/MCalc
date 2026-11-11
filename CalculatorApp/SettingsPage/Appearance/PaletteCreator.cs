namespace CalculatorApp.SettingsPage.Appearance;

public enum ColorPreset
{ Catppuccin, Gruvbox, Dracula, Nord,
    TokyoNight, Solarized, OneDark, RosePine, Custom}

public sealed record ThemePalette(
    int[] Base16, int Primary, int Secondary, int Tertiary, int Error);

public static class PaletteCreator
{
    public static ThemePalette? GetPalette(ColorPreset preset, bool isDark) => (preset, isDark) switch
    {
        (ColorPreset.Catppuccin, false) => Palette(
            "eff1f5", "e6e9ef", "ccd0da", "bcc0cc",
            "acb0be", "4c4f69", "dc8a78", "7287fd",
            "d20f39", "fe640b", "df8e1d", "40a02b",
            "179299", "1e66f5", "8839ef", "dd7878",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Catppuccin, true) => Palette(
            "1e1e2e", "181825", "313244", "45475a",
            "585b70", "cdd6f4", "f5e0dc", "b4befe",
            "f38ba8", "fab387", "f9e2af", "a6e3a1",
            "94e2d5", "89b4fa", "cba6f7", "f2cdcd",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Gruvbox, false) => Palette(
            "fbf1c7", "ebdbb2", "d5c4a1", "bdae93",
            "665c54", "504945", "3c3836", "282828",
            "9d0006", "af3a03", "b57614", "79740e",
            "427b58", "076678", "8f3f71", "d65d0e",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Gruvbox, true) => Palette(
            "282828", "3c3836", "504945", "665c54",
            "bdae93", "d5c4a1", "ebdbb2", "fbf1c7",
            "fb4934", "fe8019", "fabd2f", "b8bb26",
            "8ec07c", "83a598", "d3869b", "d65d0e",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Dracula, false) => Palette(
            "fffbeb", "efeddc", "dedccf", "cfcfde",
            "6c664b", "1f1f1f", "2c2b31", "000000",
            "cb3a2a", "a34d14", "846e15", "14710a",
            "036a96", "644ac9", "a3144d", "6c664b",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Dracula, true) => Palette(
            "282a36", "343746", "44475a", "6272a4",
            "6272a4", "f8f8f2", "f8f8f2", "ffffff",
            "ff5555", "ffb86c", "f1fa8c", "50fa7b",
            "8be9fd", "bd93f9", "ff79c6", "6272a4",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Nord, false) => Palette(
            "eceff4", "e5e9f0", "d8dee9", "9aa5b8",
            "4c566a", "2e3440", "3b4252", "434c5e",
            "bf616a", "d08770", "b48e00", "a3be8c",
            "5e81ac", "5e81ac", "b48ead", "8fbcbb",
            primary: 0x0C, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Nord, true) => Palette(
            "2e3440", "3b4252", "434c5e", "4c566a",
            "d8dee9", "e5e9f0", "eceff4", "8fbcbb",
            "bf616a", "d08770", "ebcb8b", "a3be8c",
            "88c0d0", "81a1c1", "b48ead", "5e81ac",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.TokyoNight, false) => Palette(
            "d5d6db", "cbccd1", "dfe0e5", "9699a3",
            "4c505e", "343b59", "1a1b26", "1a1b26",
            "343b58", "965027", "166775", "485e30",
            "3e6968", "34548a", "5a4a78", "8c4351",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x0F),
        (ColorPreset.TokyoNight, true) => Palette(
            "1a1b26", "24283b", "2f3549", "444b6a",
            "787c99", "a9b1d6", "c0caf5", "d5d6db",
            "f7768e", "ff9e64", "e0af68", "9ece6a",
            "7dcfff", "7aa2f7", "bb9af7", "2ac3de",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.Solarized, false) => Palette(
            "fdf6e3", "eee8d5", "eee8d5", "839496",
            "657b83", "586e75", "073642", "002b36",
            "dc322f", "cb4b16", "b58900", "859900",
            "2aa198", "268bd2", "6c71c4", "d33682",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0F, error: 0x08),
        (ColorPreset.Solarized, true) => Palette(
            "002b36", "073642", "586e75", "657b83",
            "839496", "93a1a1", "eee8d5", "fdf6e3",
            "dc322f", "cb4b16", "b58900", "859900",
            "2aa198", "268bd2", "6c71c4", "d33682",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0F, error: 0x08),
        (ColorPreset.OneDark, false) => Palette(
            "fafafa", "f0f0f1", "e5e5e6", "a0a1a7",
            "696c77", "383a42", "202227", "090a0b",
            "ca1243", "d75f00", "c18401", "50a14f",
            "0184bc", "4078f2", "a626a4", "986801",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.OneDark, true) => Palette(
            "282c34", "353b45", "3e4451", "545862",
            "565c64", "abb2bf", "b6bdca", "c8ccd4",
            "e06c75", "d19a66", "e5c07b", "98c379",
            "56b6c2", "61afef", "c678dd", "be5046",
            primary: 0x0D, secondary: 0x0B, tertiary: 0x0E, error: 0x08),
        (ColorPreset.RosePine, false) => Palette(
            "faf4ed", "fffaf3", "f2e9de", "9893a5",
            "797593", "575279", "575279", "cecacd",
            "b4637a", "ea9d34", "d7827e", "286983",
            "56949f", "907aa9", "ea9d34", "cecacd",
            primary: 0x0D, secondary: 0x0C, tertiary: 0x08, error: 0x0A),
        (ColorPreset.RosePine, true) => Palette(
            "191724", "1f1d2e", "26233a", "6e6a86",
            "908caa", "e0def4", "e0def4", "524f67",
            "eb6f92", "f6c177", "ebbcba", "31748f",
            "9ccfd8", "c4a7e7", "f6c177", "524f67",
            primary: 0x0D, secondary: 0x0C, tertiary: 0x0A, error: 0x08),
        _ => null
    };

    private static ThemePalette Palette(
        string base00, string base01, string base02, string base03,
        string base04, string base05, string base06, string base07,
        string base08, string base09, string base0A, string base0B,
        string base0C, string base0D, string base0E, string base0F,
        int primary, int secondary, int tertiary, int error) =>
        new(
        [
            Hex(base00), Hex(base01), Hex(base02), Hex(base03),
            Hex(base04), Hex(base05), Hex(base06), Hex(base07),
            Hex(base08), Hex(base09), Hex(base0A), Hex(base0B),
            Hex(base0C), Hex(base0D), Hex(base0E), Hex(base0F),
        ], primary, secondary, tertiary, error);

    private static int Hex(string value) =>
        unchecked((int)0xFF000000) | Convert.ToInt32(value, 16);
}
