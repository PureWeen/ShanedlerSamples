#if IOS || MACCATALYST
using UIKit;

namespace Maui.FixesAndWorkarounds;

public sealed partial class KeyboardBehaviorTrigger
{
	internal UIKeyModifierFlags PlatformModifiers { get; private set; }
    internal List<UIKeyboardHidUsage> PlatformKeys { get; private set; }

    void SetPlatformModifiers(KeyboardModifiers modifiers)
        => PlatformModifiers = modifiers.ToPlatformModifiers();

    void SetPlatformKeys(KeyboardKeys keys)
        => PlatformKeys = keys.ToPlatformKeyValues();
}

internal class KeyboardBehaviorTriggerComparer : IEqualityComparer<KeyboardBehaviorTrigger>
{
    public bool Equals(KeyboardBehaviorTrigger x, KeyboardBehaviorTrigger y)
    {
        if (x == null && y == null)
            return true;
        if (x == null || y == null)
            return false;
        
        return x.PlatformKeys.SequenceEqual(y.PlatformKeys) && x.PlatformModifiers == y.PlatformModifiers;
    }

    public int GetHashCode(KeyboardBehaviorTrigger obj)
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + obj.Keys.GetHashCode();
            hash = hash * 23 + obj.PlatformModifiers.GetHashCode();
            return hash;
        }
    }
}
#endif