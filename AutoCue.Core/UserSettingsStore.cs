using System.Text.Json;

namespace AutoCue.Core;

public sealed class UserSettingsStore
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly string _settingsPath;

    public UserSettingsStore(string? settingsPath = null)
    {
        _settingsPath = settingsPath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "AutoCue",
            "settings.json");
    }

    public UserSettings Load()
    {
        if (!File.Exists(_settingsPath))
        {
            return UserSettings.Default;
        }

        var json = File.ReadAllText(_settingsPath);
        var settings = JsonSerializer.Deserialize<UserSettings>(json)
            ?? throw new InvalidDataException("The preferences file did not contain valid settings.");
        return settings.Normalize();
    }

    public void Save(UserSettings settings)
    {
        var directory = Path.GetDirectoryName(_settingsPath)
            ?? throw new InvalidOperationException("The preferences file path must include a directory.");
        Directory.CreateDirectory(directory);

        var temporaryPath = _settingsPath + ".tmp";
        File.WriteAllText(temporaryPath, JsonSerializer.Serialize(settings.Normalize(), JsonOptions));
        File.Move(temporaryPath, _settingsPath, overwrite: true);
    }
}
