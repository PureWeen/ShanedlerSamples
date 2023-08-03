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
			bool result = false;
			foreach (var item in keyboardBehaviors)
			{
				result = item.PressesBegan(presses, evt);
				if (result)
					break;
			}

			if (!result)
				base.PressesBegan(presses, evt);
		}

        public override void PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
        {
            bool result = false;
            foreach (var item in keyboardBehaviors)
            {
				result = item.PressesEnded(presses, evt);
                if (result)
                    break;
            }


            if (!result)
			    base.PressesEnded(presses, evt);

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