using System.IO;
using System.Text.Json;
using SaiBridge.Models;

namespace SaiBridge.Services;

public static class SettingsService
{
    private static readonly string SettingsFile =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "settings.json");

    public static Settings Load()
    {
        try
        {
            if (!File.Exists(SettingsFile))
                return new Settings();

            string json = File.ReadAllText(SettingsFile);

            return JsonSerializer.Deserialize<Settings>(json)
                   ?? new Settings();
        }
        catch
        {
            return new Settings();
        }
    }

    public static void Save(Settings settings)
    {
        string json = JsonSerializer.Serialize(
            settings,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });

        File.WriteAllText(SettingsFile, json);
    }
}