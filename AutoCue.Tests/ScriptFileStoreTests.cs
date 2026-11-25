using AutoCue.Core;
using Xunit;

namespace AutoCue.Tests;

public sealed class ScriptFileStoreTests
{
    [Fact]
    public void SavesAndLoadsUnicodeText()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var path = Path.Combine(directory.FullName, "my-script.txt");
            const string script = "Hello, world!\nGoedemorgen 🌍";

            ScriptFileStore.Save(path, script);

            Assert.Equal(script, ScriptFileStore.Load(path));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }

    [Fact]
    public void RejectsUnsupportedExtensions()
    {
        var directory = Directory.CreateTempSubdirectory();
        try
        {
            var path = Path.Combine(directory.FullName, "script.docx");

            Assert.Throws<ArgumentException>(() => ScriptFileStore.Save(path, "script"));
        }
        finally
        {
            directory.Delete(recursive: true);
        }
    }
}
