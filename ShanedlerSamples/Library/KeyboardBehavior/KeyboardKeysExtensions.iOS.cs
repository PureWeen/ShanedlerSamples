#if IOS || MACCATALYST
using UIKit;

namespace Maui.FixesAndWorkarounds;

internal static class KeyboardKeysExtensions
{
    internal static UIKeyboardHidUsage ToPlatformKeys(this KeyboardKeys virtualKeys)
    {
        List<UIKeyboardHidUsage> platformKeyValues = new();

        foreach (KeyboardKeys virtualKey in Enum.GetValues(typeof(KeyboardKeys)))
        {
            if (virtualKeys.HasFlag(virtualKey))
            {
                UIKeyboardHidUsage platformKey = ToPlatformKey(virtualKey);

                if (platformKey != 0)
                    platformKeyValues.Add(platformKey);
            }
        }

        var platformKeys = ToPlatformKeys(platformKeyValues);

        return platformKeys;
    }

    internal static List<UIKeyboardHidUsage> ToPlatformKeyValues(this KeyboardKeys virtualKeys)
    {
        List<UIKeyboardHidUsage> platformKeyValues = new();

        foreach (KeyboardKeys virtualKey in Enum.GetValues(typeof(KeyboardKeys)))
        {
            if (virtualKeys.HasFlag(virtualKey))
            {
                UIKeyboardHidUsage platformKey = ToPlatformKey(virtualKey);

                if (platformKey != 0)
                    platformKeyValues.Add(platformKey);
            }
        }

        return platformKeyValues;
    }

    internal static KeyboardKeys ToVirtualKeys(this UIKeyboardHidUsage platformKeys)
    {
        List<KeyboardKeys> virtualKeyValues = new();

        foreach (UIKeyboardHidUsage platformKey in Enum.GetValues(typeof(UIKeyboardHidUsage)))
        {
            if (platformKeys.HasFlag(platformKey))
            {
                KeyboardKeys virtualKey = ToVirtualKey(platformKey);

                if (virtualKey != 0)
                    virtualKeyValues.Add(virtualKey);
            }
        }

        var virtualKeys = ToVirtualKeys(virtualKeyValues);

        return virtualKeys;
    }

    internal static KeyboardKeys ToVirtualKeys(this List<UIKeyboardHidUsage> platformKeys)
    {
        List<KeyboardKeys> virtualKeyValues = new();

        foreach (UIKeyboardHidUsage platformKey in platformKeys)
        {
            if (platformKeys.Contains(platformKey))
            {
                KeyboardKeys virtualKey = ToVirtualKey(platformKey);

                if (virtualKey != 0)
                    virtualKeyValues.Add(virtualKey);
            }
        }

        var virtualKeys = ToVirtualKeys(virtualKeyValues);

        return virtualKeys;
    }

    internal static UIKeyboardHidUsage ToPlatformKeys(List<UIKeyboardHidUsage> platformKeyValues)
    {
        UIKeyboardHidUsage platformKeys = 0;

        foreach (UIKeyboardHidUsage platformKey in platformKeyValues)
            platformKeys |= platformKey;

        return platformKeys;
    }

    internal static KeyboardKeys ToVirtualKeys(List<KeyboardKeys> virtualKeyValues)
    {
        KeyboardKeys virtualKeys = 0;

        foreach (KeyboardKeys virtualKey in virtualKeyValues)
            virtualKeys |= virtualKey;

        return virtualKeys;
    }

    static UIKeyboardHidUsage ToPlatformKey(KeyboardKeys virtualKey) => virtualKey switch
    {
        KeyboardKeys.A => UIKeyboardHidUsage.KeyboardA,
        KeyboardKeys.B => UIKeyboardHidUsage.KeyboardB,
        _ => 0
    };

    static KeyboardKeys ToVirtualKey(UIKeyboardHidUsage platformKey) => platformKey switch
    {
        UIKeyboardHidUsage.KeyboardA => KeyboardKeys.A,
        UIKeyboardHidUsage.KeyboardB => KeyboardKeys.B,
        _ => 0
    };
}
#endif