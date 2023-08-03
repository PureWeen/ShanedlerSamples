using Foundation;
using Maui.FixesAndWorkarounds;
using UIKit;

namespace ShanedlerSamples;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    static List<KeyboardBehavior> keyboardBehaviors = new List<KeyboardBehavior>();
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    public override void PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        base.PressesBegan(presses, evt);

        foreach (var item in keyboardBehaviors)
            item.PressesBegan(presses, evt);

    }

    public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
    {
        base.PressesEnded(presses, evt);

        foreach (var item in keyboardBehaviors)
            item.PressesEnded(presses, evt);
    }

    public static void Register(KeyboardBehavior keyboardBehavior)
    {
        keyboardBehaviors.Add(keyboardBehavior);
    }

    public static void UnRegister(KeyboardBehavior keyboardBehavior)
    {
        keyboardBehaviors.Remove(keyboardBehavior);
    }
}
