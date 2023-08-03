#if MACCATALYST || IOS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Foundation;
using UIKit;

namespace Maui.FixesAndWorkarounds
{
	public partial class KeyboardBehavior : PlatformBehavior<VisualElement>
	{
        protected override void OnAttachedTo(VisualElement bindable, UIView platformView)
        {
            base.OnAttachedTo(bindable, platformView);
            CustomKeyboardController.Register(this);
        }

        protected override void OnDetachedFrom(VisualElement bindable, UIView platformView)
        {
            base.OnDetachedFrom(bindable, platformView);
            CustomKeyboardController.UnRegister(this);
        }

        public bool PressesBegan(NSSet<UIPress> presses, UIPressesEvent evt)
		{
			System.Diagnostics.Debug.WriteLine($"STARTING PressesBegan");

			foreach (var item in evt.AllPresses)
                if (item is UIPress allPress)
                    System.Diagnostics.Debug.WriteLine($"PressesBegan: {allPress?.Key}");

			System.Diagnostics.Debug.WriteLine($"PressesBegan: {evt}");

			System.Diagnostics.Debug.WriteLine($"FINISHING PressesBegan");

			return true;
        }

        public bool PressesEnded(NSSet<UIPress> presses, UIPressesEvent evt)
		{
			System.Diagnostics.Debug.WriteLine($"STARTING PressesEnded");
			foreach (var item in evt.AllPresses)
				if (item is UIPress allPress)
					System.Diagnostics.Debug.WriteLine($"PressesEnded: {allPress?.Key}");

			System.Diagnostics.Debug.WriteLine($"PressesEnded: {evt}");
			System.Diagnostics.Debug.WriteLine($"STARTING PressesEnded");

			return true;
		}
    }
}
#endif