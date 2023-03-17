#if IOS || MACCATALYST
#nullable enable
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Maui.FixesAndWorkarounds.Library.Common
{
	internal static partial class Extensions
	{
		public static UIWindow? GetKeyWindow(this UIApplication application)
		{
#pragma warning disable CA1416 // TODO: 'UIApplication.Windows' is unsupported on: 'ios' 15.0 and later.
#pragma warning disable CA1422 // Validate platform compatibility
			var windows = application.Windows;
#pragma warning restore CA1422 // Validate platform compatibility
#pragma warning restore CA1416

			for (int i = 0; i < windows.Length; i++)
			{
				var window = windows[i];
				if (window.IsKeyWindow)
					return window;
			}

			return null;
		}
		public static IWindow? GetWindow(this UIWindow? platformWindow)
		{
			if (platformWindow is null)
				return null;

			foreach (var window in MauiUIApplicationDelegate.Current.Application.Windows)
			{
				if (window?.Handler?.PlatformView == platformWindow)
					return window;
			}

			return null;
		}

		public static IWindow? GetWindow(this UIApplication application) =>
			application.GetKeyWindow().GetWindow();

		internal static T? FindResponder<T>(this UIView view) where T : UIResponder
		{
			var nextResponder = view as UIResponder;
			while (nextResponder is not null)
			{
				nextResponder = nextResponder.NextResponder;

				if (nextResponder is T responder)
					return responder;
			}
			return null;
		}

		internal static T? FindResponder<T>(this UIViewController controller) where T : UIViewController
		{
			var nextResponder = controller.View as UIResponder;
			while (nextResponder is not null)
			{
				nextResponder = nextResponder.NextResponder;

				if (nextResponder is T responder && responder != controller)
					return responder;
			}
			return null;
		}

		internal static UIView? FindNextView(this UIView? view, UIView containerView, Func<UIView, bool> isValidType)
		{
			UIView? nextView = null;

			while (view is not null && view != containerView && nextView is null)
			{
				var siblings = view.Superview?.Subviews;

				if (siblings is null)
					break;

				nextView = view.FindNextView(siblings.IndexOf(view) + 1, isValidType);

				view = view.Superview;
			}

			// if we did not find the next view, try to find the first one
			nextView ??= containerView.Subviews?[0]?.FindNextView(0, isValidType);

			return nextView;
		}

		static UIView? FindNextView(this UIView? view, int index, Func<UIView, bool> isValidType)
		{
			// search through the view's siblings and traverse down their branches
			var siblings = view?.Superview?.Subviews;

			if (siblings is null)
				return null;

			for (int i = index; i < siblings.Length; i++)
			{
				var sibling = siblings[i];

				if (sibling.Subviews is not null && sibling.Subviews.Length > 0)
				{
					var childVal = sibling.Subviews[0].FindNextView(0, isValidType);
					if (childVal is not null)
						return childVal;
				}

				if (isValidType(sibling))
					return sibling;
			}

			return null;
		}

		internal static void ChangeFocusedView(this UIView view, UIView? newView)
		{
			if (newView is null)
				view.ResignFirstResponder();

			else
				newView.BecomeFirstResponder();
		}
		internal static T? FindTopController<T>(this UIView view) where T : UIViewController
		{
			var bestController = view.FindResponder<T>();
			var tempController = bestController;

			while (tempController is not null)
			{
				tempController = tempController.FindResponder<T>();

				if (tempController is not null)
					bestController = tempController;
			}

			return bestController;
		}

		internal static UIView? GetContainerView(this UIView? startingPoint)
		{
			var rootView = startingPoint?.FindResponder<ContainerViewController>()?.View;

			if (rootView is not null)
				return rootView;

			var firstViewController = startingPoint?.FindTopController<UIViewController>();

			if (firstViewController?.ViewIfLoaded is not null)
				return firstViewController.ViewIfLoaded.FindDescendantView<Microsoft.Maui.Platform.ContentView>();

			return null;
		}
	}
}
#endif