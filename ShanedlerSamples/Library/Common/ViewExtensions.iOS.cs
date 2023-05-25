#if IOS || MACCATALYST
#nullable enable
using CoreGraphics;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using static Microsoft.Maui.Primitives.Dimension;

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

				// TableView and ListView cells may not be in order so handle separately
				if (view.FindResponder<UITableView>() is UITableView tableView)
				{
					nextView = view.FindNextInTableView(tableView, isValidType);

					if (nextView is null)
						view = tableView;
				}

				else
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
			var siblings = view is UITableView table ? table.VisibleCells : view?.Superview?.Subviews;

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

		static UIView? FindNextInTableView(this UIView view, UITableView table, Func<UIView, bool> isValidType)
		{
			if (isValidType(view))
			{
				var index = view.FindTableViewCellIndex(table);

				return index == -1 ? null : table.FindNextView(index + 1, isValidType);
			}

			return null;
		}

		static int FindTableViewCellIndex(this UIView view, UITableView table)
		{
			var cells = table.VisibleCells;
			var viewCell = view.FindResponder<UITableViewCell>();

			for (int i = 0; i < cells.Length; i++)
			{
				if (cells[i] == viewCell)
					return i;
			}
			return -1;
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

		internal static Size GetDesiredSizeFromHandler(this IViewHandler viewHandler, double widthConstraint, double heightConstraint)
		{
			var virtualView = viewHandler.VirtualView;
			var platformView = (viewHandler as IPlatformViewHandler)?.ContainerView
				?? (viewHandler as IPlatformViewHandler)?.PlatformView;

			if (platformView == null || virtualView == null)
			{
				return new Size(widthConstraint, heightConstraint);
			}

			var sizeThatFits = platformView.SizeThatFits(new CoreGraphics.CGSize((float)widthConstraint, (float)heightConstraint));

			var size = new Size(
				sizeThatFits.Width == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Width,
				sizeThatFits.Height == float.PositiveInfinity ? double.PositiveInfinity : sizeThatFits.Height);

			if (double.IsInfinity(size.Width) || double.IsInfinity(size.Height))
			{
				platformView.SizeToFit();
				size = new Size(platformView.Frame.Width, platformView.Frame.Height);
			}

			var finalWidth = ResolveConstraints(size.Width, virtualView.Width, virtualView.MinimumWidth, virtualView.MaximumWidth);
			var finalHeight = ResolveConstraints(size.Height, virtualView.Height, virtualView.MinimumHeight, virtualView.MaximumHeight);

			return new Size(finalWidth, finalHeight);
		}

		internal static void PlatformArrangeHandler(this IViewHandler viewHandler, Rect rect)
		{
			var platformView = (viewHandler.ContainerView ?? viewHandler.PlatformView) as UIView;

			if (platformView == null)
				return;

			var centerX = rect.Center.X;

			var parent = platformView.Superview;
			if (parent?.EffectiveUserInterfaceLayoutDirection == UIUserInterfaceLayoutDirection.RightToLeft)
			{
				// We'll need to adjust the center point to reflect the RTL layout
				// Find the center of the parent
				var parentCenter = parent.Bounds.Right - (parent.Bounds.Width / 2);

				// Figure out how far the center of the destination rect is from the center of the parent
				var distanceFromParentCenter = parentCenter - centerX;

				// Mirror the center to the other side of the center of the parent
				centerX += (distanceFromParentCenter * 2);
			}

			// We set Center and Bounds rather than Frame because Frame is undefined if the CALayer's transform is 
			// anything other than the identity (https://developer.apple.com/documentation/uikit/uiview/1622459-transform)
			platformView.Center = new CGPoint(centerX, rect.Center.Y);
			platformView.Bounds = new CGRect(platformView.Bounds.X, platformView.Bounds.Y, rect.Width, rect.Height);

			viewHandler.Invoke(nameof(IView.Frame), rect);
		}

		internal static double ResolveConstraints(double measured, double exact, double min, double max)
		{
			var resolved = measured;

			min = ResolveMinimum(min);

			if (IsExplicitSet(exact))
			{
				// If an exact value has been specified, try to use that
				resolved = exact;
			}

			if (resolved > max)
			{
				// Apply the max value constraint (if any)
				// If the exact value is in conflict with the max value, the max value should win
				resolved = max;
			}

			if (resolved < min)
			{
				// Apply the min value constraint (if any)
				// If the exact or max value is in conflict with the min value, the min value should win
				resolved = min;
			}

			return resolved;
		}
		public static bool IsRightToLeft(this EffectiveFlowDirection self)
		{
			return (self & EffectiveFlowDirection.RightToLeft) == EffectiveFlowDirection.RightToLeft;
		}

		static partial void ProcessAutoPackage(IElement element)
		{
			if (element?.Handler?.PlatformView is not UIView viewGroup)
				return;

			viewGroup.ClearSubviews();

			if (element is not IVisualTreeElement vte)
				return;

			var mauiContext = element?.Handler?.MauiContext;
			if (mauiContext == null)
				return;

			foreach (var child in vte.GetVisualChildren())
			{
				if (child is IElement childElement)
					viewGroup.AddSubview(childElement.ToPlatform(mauiContext));
			}
		}
	}
}
#endif
