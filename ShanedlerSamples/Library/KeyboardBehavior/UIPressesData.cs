#if IOS || MACCATALYST
using Foundation;
using UIKit;

namespace Maui.FixesAndWorkarounds;

internal class UIPressesData
{
    internal UIKeyModifierFlags Modifiers { get; set; } = 0;
    internal List<UIKeyboardHidUsage> Keys { get; set; } = new List<UIKeyboardHidUsage>();

    internal static UIPressesData Create(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        var pressData = new UIPressesData
        {
            Modifiers = presses.AnyObject is UIPress anyPress && anyPress.Key is UIKey anyPressKey ? anyPressKey.ModifierFlags : 0
        };

        var checkForModifier = pressData.Modifiers == 0;

        var keys = new List<UIKeyboardHidUsage>();

        foreach (var press in evt.AllPresses.Cast<UIPress>())
        {
            if (press.Key is UIKey key && !string.IsNullOrWhiteSpace(key.CharactersIgnoringModifiers))
            {
                keys.Add(key.KeyCode);

                if (checkForModifier)
                {
                    pressData.Modifiers = key.ModifierFlags;
                    checkForModifier = pressData.Modifiers == 0;
                }
            }
        }

        pressData.Keys = keys.OrderBy(i => i).ToList();

        return pressData;
    }

    public override string ToString()
    {
        var builder = new System.Text.StringBuilder();

        builder.Append($"{nameof(Modifiers)}: {(Modifiers != 0 ? Modifiers : "None")}");
        builder.Append($", {nameof(Keys)}: ");

        if (!Keys.Any())
        {
            builder.Append("None");
            return builder.ToString();
        }

        foreach (var key in Keys)
            builder.Append($"{key}, ");

        builder.Remove(builder.Length - 2, 2);

        return builder.ToString();
    }
}
#endif