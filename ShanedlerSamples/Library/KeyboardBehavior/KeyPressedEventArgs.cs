namespace Maui.FixesAndWorkarounds;

public sealed class KeyPressedEventArgs : EventArgs
{
    public KeyboardModifiers Modifiers { get; internal set; }
    public KeyboardKeys Keys { get; internal set; }
}
