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

			var page = GetParentPage(bindable);

			if (page == null)
				return;

			// Register to key press events
			if (page.Handler is not IPlatformViewHandler viewHandler ||
				viewHandler.ViewController is not KeyboardPageViewController keyboardPageViewController)
				return;

			keyboardPageViewController.RegisterKeyboardBehavior(this);
		}

		protected override void OnDetachedFrom(VisualElement bindable, UIView platformView)
		{
			base.OnDetachedFrom(bindable, platformView);

			var page = GetParentPage(bindable);

			if (page == null)
				return;

			// Unregister from key press events
			if (page.Handler is not IPlatformViewHandler viewHandler ||
				viewHandler.ViewController is not KeyboardPageViewController keyboardPageViewController)
				return;

			keyboardPageViewController.UnregisterKeyboardBehavior(this);
		}

		static Page GetParentPage(VisualElement element)
		{
			if (element is Page)
				return element as Page;

			Element currentElement = element;

			while (currentElement != null && currentElement is not Page)
				currentElement = currentElement.Parent;

			return currentElement as Page;
		}
	}
}
#endif