namespace Maui.FixesAndWorkarounds;

[Flags]
public enum KeyboardModifiers
{
    None = 0,
    Shift = 2,
    Command = 4,
    Control = 8,
    Alt = 16
}

[Flags]
public enum KeyboardKeys
{
    None,
    A,
    B
}