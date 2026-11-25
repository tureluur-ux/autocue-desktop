using AutoCue.Core;
using Xunit;

namespace AutoCue.Tests;

public sealed class UserSettingsStoreTests
{
    [Fact]
    public void MissingFileReturnsDefaultSettings()
    {
        using var directory = new TemporaryDirectory();
        var store = new UserSettingsStore(Path.Combine(directory.Path, "settings.json"));

        Assert.Equal(UserSettings.Default, store.Load());
    }

    [Fact]
    public void SaveAndLoadPreservesSettings()
    {
        using var directory = new TemporaryDirectory();
        var store = new UserSettingsStore(Path.Combine(directory.Path, "settings.json"));
        var expected = new UserSettings(88, 42);

        store.Save(expected);

        Assert.Equal(expected, store.Load());
    }

    [Fact]
    public void LoadClampsOutOfRangeSettings()
    {
        using var directory = new TemporaryDirectory();
        var path = Path.Combine(directory.Path, "settings.json");
        File.WriteAllText(path, """{"ScrollSpeed":500,"FontSize":12}""");
        var store = new UserSettingsStore(path);

        Assert.Equal(new UserSettings(240, 24), store.Load());
    }

    private sealed class TemporaryDirectory : IDisposable
    {
        public string Path { get; } = Directory.CreateTempSubdirectory().FullName;

        public void Dispose() => Directory.Delete(Path, recursive: true);
    }
}
