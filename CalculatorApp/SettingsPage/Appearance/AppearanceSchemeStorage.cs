using System.Text.Json;
using System.Text.Json.Serialization;

namespace CalculatorApp.SettingsPage.Appearance;

public sealed record AppearanceSchemeCache(string Key,
    Dictionary<string, int> RoleColors);

[JsonSerializable(typeof(AppearanceSchemeCache))]
internal sealed partial class AppearanceSchemeCacheJsonContext : JsonSerializerContext { }

public class AppearanceSchemeStorage
{
    private static readonly string Path = System.IO.Path.Combine(
        FileSystem.AppDataDirectory, "appearance_scheme.json");

    public AppearanceSchemeStorage()
    {
        try
        {
            if (!File.Exists(Path))
            {
                Cache = new AppearanceSchemeCache(string.Empty, []);
                return;
            }

            if (JsonSerializer.Deserialize<AppearanceSchemeCache>(
                    File.ReadAllText(Path), AppearanceSchemeCacheJsonContext
                        .Default.AppearanceSchemeCache) is { } cache)
            {
                Cache = cache;
            }
            else
            {
                Cache = new AppearanceSchemeCache(string.Empty, []);
            }
        }
        catch
        {
            Cache = new AppearanceSchemeCache(string.Empty, []);
        }
    }

    public AppearanceSchemeCache Cache { get; }

    public void SetColorRole(string role, int color) =>
        Cache.RoleColors[role] = color;

    public void Save() =>
        File.WriteAllText(Path, JsonSerializer.Serialize(
            Cache, AppearanceSchemeCacheJsonContext.Default.AppearanceSchemeCache));
}
