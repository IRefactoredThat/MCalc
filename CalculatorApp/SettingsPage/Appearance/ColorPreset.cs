namespace CalculatorApp.SettingsPage.Appearance;

public static class ColorPresets
{
    private const int Black = unchecked((int)0xFF000000);
    private const int White = unchecked((int)0xFFFFFFFF);
    private const double DarkContainerMix = 0.72;
    private const double LightContainerMix = 0.82;

    public static bool TryGetColors(ColorPreset preset, bool isDark,
        out Dictionary<string, int> colors)
    {
        if (preset == ColorPreset.Custom)
        {
            colors = [];
            return false;
        }

        var palette = PaletteCreator.GetPalette(preset, isDark);
        if (palette is null)
        {
            colors = [];
            return false;
        }

        colors = Colors(palette, isDark);
        return true;
    }

    public static bool Matches(ColorPreset preset, bool isDark,
        IReadOnlyList<ColorRole> roles)
    {
        if (!TryGetColors(preset, isDark, out var colors))
        {
            return false;
        }

        foreach (var role in roles)
        {
            if (!colors.TryGetValue(role.Key, out var value) ||
                role.Color.ToInt() != value)
            {
                return false;
            }
        }

        return true;
    }

    private static Dictionary<string, int> Colors(ThemePalette palette, bool isDark)
    {
        var base16 = palette.Base16;
        var background = base16[0x00];
        var lowSurface = base16[0x01];
        var surface = base16[0x02];
        var outline = base16[0x03];
        var muted = base16[0x04];
        var onSurface = base16[0x05];
        var primary = base16[palette.Primary];
        var secondary = base16[palette.Secondary];
        var tertiary = base16[palette.Tertiary];
        var error = base16[palette.Error];
        var containerMix = isDark ? DarkContainerMix : LightContainerMix;
        var onSurfaceVariant = isDark
            ? Mix(muted, onSurface, 0.30)
            : Mix(muted, onSurface, 0.55);
        var outlineColor = isDark ? outline : Mix(outline, onSurface, 0.25);

        return new Dictionary<string, int>(ColorRoleDefinitions.All.Count)
        {
            [nameof(MaterialScheme.Primary)] = primary,
            [nameof(MaterialScheme.OnPrimary)] = background,
            [nameof(MaterialScheme.PrimaryContainer)] =
                Mix(primary, background, containerMix),
            [nameof(MaterialScheme.OnPrimaryContainer)] = primary,
            [nameof(MaterialScheme.Secondary)] = secondary,
            [nameof(MaterialScheme.OnSecondary)] = background,
            [nameof(MaterialScheme.SecondaryContainer)] =
                Mix(secondary, background, containerMix),
            [nameof(MaterialScheme.OnSecondaryContainer)] = secondary,
            [nameof(MaterialScheme.Tertiary)] = tertiary,
            [nameof(MaterialScheme.OnTertiary)] = background,
            [nameof(MaterialScheme.TertiaryContainer)] =
                Mix(tertiary, background, containerMix),
            [nameof(MaterialScheme.OnTertiaryContainer)] = tertiary,
            [nameof(MaterialScheme.Error)] = error,
            [nameof(MaterialScheme.OnError)] = background,
            [nameof(MaterialScheme.ErrorContainer)] =
                Mix(error, background, containerMix),
            [nameof(MaterialScheme.OnErrorContainer)] = error,
            [nameof(MaterialScheme.Background)] = background,
            [nameof(MaterialScheme.OnBackground)] = onSurface,
            [nameof(MaterialScheme.Surface)] = background,
            [nameof(MaterialScheme.OnSurface)] = onSurface,
            [nameof(MaterialScheme.SurfaceVariant)] = surface,
            [nameof(MaterialScheme.OnSurfaceVariant)] = onSurfaceVariant,
            [nameof(MaterialScheme.Outline)] = outlineColor,
            [nameof(MaterialScheme.Shadow)] = Black,
            [nameof(MaterialScheme.InverseSurface)] = onSurface,
            [nameof(MaterialScheme.InverseOnSurface)] = background,
            [nameof(MaterialScheme.InversePrimary)] = isDark
                ? Mix(primary, Black, 0.20)
                : Mix(primary, White, 0.30),
            [nameof(MaterialScheme.SurfaceDim)] = isDark
                ? Mix(background, Black, 0.20)
                : Mix(background, lowSurface, 0.70),
            [nameof(MaterialScheme.SurfaceBright)] = isDark
                ? Mix(background, onSurface, 0.12)
                : background,
            [nameof(MaterialScheme.SurfaceContainerLowest)] = isDark
                ? Mix(background, Black, 0.12)
                : background,
            [nameof(MaterialScheme.SurfaceContainerLow)] = isDark
                ? Mix(background, surface, 0.25)
                : lowSurface,
            [nameof(MaterialScheme.SurfaceContainer)] = isDark
                ? Mix(background, surface, 0.45)
                : Mix(lowSurface, surface, 0.45),
            [nameof(MaterialScheme.SurfaceContainerHigh)] = isDark
                ? Mix(background, surface, 0.70)
                : surface,
            [nameof(MaterialScheme.SurfaceContainerHighest)] = isDark
                ? surface
                : Mix(surface, outline, 0.35),
            [nameof(MaterialScheme.OutlineVariant)] = isDark
                ? Mix(surface, outline, 0.55)
                : Mix(surface, outline, 0.40),
        };
    }

    private static int Mix(int first, int second, double amount)
    {
        return unchecked((int)0xFF000000) |
               (Blend(16) << 16) |
               (Blend(8) << 8) |
               Blend(0);

        int Blend(int shift) => (int)Math.Round(
            Channel(first, shift) * (1 - amount) +
            Channel(second, shift) * amount);

        static int Channel(int color, int shift) => (color >> shift) & 0xFF;
    }
}
