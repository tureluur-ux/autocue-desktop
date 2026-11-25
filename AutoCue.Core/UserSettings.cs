namespace AutoCue.Core;

public sealed record UserSettings(double ScrollSpeed = 42, double FontSize = 36)
{
    public static UserSettings Default { get; } = new();

    public UserSettings Normalize() => this with
    {
        ScrollSpeed = Math.Clamp(ScrollSpeed, 10, 240),
        FontSize = Math.Clamp(FontSize, 24, 56)
    };
}
