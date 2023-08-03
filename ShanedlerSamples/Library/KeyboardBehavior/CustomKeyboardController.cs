#if IOS || MACCATALYST
using System;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace Maui.FixesAndWorkarounds
{
    public class CustomKeyboardController : PageViewController
    {
        static List<KeyboardBehavior> keyboardBehaviors = new List<KeyboardBehavior>();
        public CustomKeyboardController(IView page, IMauiContext mauiContext) : base(page, mauiContext)
        {
        }


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

   
}
#endif