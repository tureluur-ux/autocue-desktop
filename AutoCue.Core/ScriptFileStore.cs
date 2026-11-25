using System.Text;

namespace AutoCue.Core;

public static class ScriptFileStore
{
    public static string Load(string path)
    {
        ValidatePath(path);
        return File.ReadAllText(path);
    }

    public static void Save(string path, string content)
    {
        ValidatePath(path);
        File.WriteAllText(path, content, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    private static void ValidatePath(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        var extension = Path.GetExtension(path);
        if (!string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(extension, ".md", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Scripts must use the .txt or .md file extension.", nameof(path));
        }
    }
}
