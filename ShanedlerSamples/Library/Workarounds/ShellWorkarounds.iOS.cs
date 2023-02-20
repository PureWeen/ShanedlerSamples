#if IOS || MACCATALYST
using CoreGraphics;
using Foundation;
using Microsoft.Maui;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Internals;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using System.Reflection;
using UIKit;

namespace Maui.FixesAndWorkarounds
{
	// Fix Thank you to https://github.com/vhugogarcia
	public class ShellWorkarounds : ShellRenderer
	{
		public static IShellPageRendererTracker Tracker { get; set; }

		protected override IShellPageRendererTracker CreatePageRendererTracker()
		{
			if (Tracker != null)
				throw new InvalidOperationException("This should have been cleared out by CustomShellSectionRenderer");

			return (Tracker = new CustomShellPageRendererTracker(this));
		}

		protected override IShellNavBarAppearanceTracker CreateNavBarAppearanceTracker()
		{
			return new NoLineAppearanceTracker();
		}

		protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
		{
			return new CustomShellSectionRenderer(this);
		}
	}

	public class CustomShellSectionRootRenderer : ShellSectionRootRenderer
	{
		CustomShellSectionRenderer _customShellSectionRenderer;
		public CustomShellSectionRootRenderer(ShellSection shellSection, IShellContext shellContext, CustomShellSectionRenderer customShellSectionRenderer) : base(shellSection, shellContext)
		{
			_customShellSectionRenderer = customShellSectionRenderer;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			_customShellSectionRenderer.SnagTracker();
		}
	}

	public class CustomShellSectionRenderer : ShellSectionRenderer
	{
		readonly Dictionary<Element, IShellPageRendererTracker> _trackers =
			new Dictionary<Element, IShellPageRendererTracker>();

		UINavigationControllerDelegate _navDelegate;
		public CustomShellSectionRenderer(IShellContext context) : base(context)
		{
			_navDelegate = Delegate as UINavigationControllerDelegate;
			Delegate = new NavDelegate(_navDelegate, this);
			Context = context;
		}

		public IShellContext Context { get; }

		protected override IShellSectionRootRenderer CreateShellSectionRootRenderer(ShellSection shellSection, IShellContext shellContext)
		{
			return new CustomShellSectionRootRenderer(shellSection, shellContext, this);
		}

		public void SnagTracker()
		{
			if (ShellWorkarounds.Tracker is null)
				return;

			_trackers[ShellWorkarounds.Tracker.Page] = ShellWorkarounds.Tracker;
			ShellWorkarounds.Tracker = null;
		}

		protected override void OnNavigationRequested(object sender, NavigationRequestedEventArgs e)
		{
			SnagTracker();
			base.OnNavigationRequested(sender, e);
			SnagTracker();
		}

		protected override void OnPushRequested(NavigationRequestedEventArgs e)
		{
			SnagTracker();
			base.OnPushRequested(e);
			SnagTracker();
		}

		protected override void OnInsertRequested(NavigationRequestedEventArgs e)
		{
			SnagTracker();
			base.OnInsertRequested(e);
			SnagTracker();
		}

		public override void PushViewController(UIViewController viewController, bool animated)
		{
			SnagTracker();
			base.PushViewController(viewController, animated);
			SnagTracker();
		}

		class NavDelegate : UINavigationControllerDelegate
		{
			UINavigationControllerDelegate navDelegate;
			CustomShellSectionRenderer _self;

			public NavDelegate(UINavigationControllerDelegate navDelegate, CustomShellSectionRenderer customShellSectionRenderer)
			{
				this.navDelegate = navDelegate;
				_self = customShellSectionRenderer;
			}

			// This is currently working around a Mono Interpreter bug
			// if you remove this code please verify that hot restart still works
			// https://github.com/xamarin/Xamarin.Forms/issues/10519
			[Export("navigationController:animationControllerForOperation:fromViewController:toViewController:")]
			[Foundation.Preserve(Conditional = true)]
			public new IUIViewControllerAnimatedTransitioning GetAnimationControllerForOperation(UINavigationController navigationController, UINavigationControllerOperation operation, UIViewController fromViewController, UIViewController toViewController)
			{
				return null;
			}

			public override void DidShowViewController(UINavigationController navigationController, [Transient] UIViewController viewController, bool animated)
			{
				navDelegate.DidShowViewController(navigationController, viewController, animated);
			}

			public override void WillShowViewController(UINavigationController navigationController, [Transient] UIViewController viewController, bool animated)
			{
				navDelegate.WillShowViewController(navigationController, viewController, animated);

				// Because the back button title needs to be set on the previous VC
				// We want to set the BackButtonItem as early as possible so there is no flickering
				var currentPage = _self.Context?.Shell?.CurrentPage;
				var trackers = _self._trackers;
				if (currentPage?.Handler is IPlatformViewHandler pvh &&
					pvh.ViewController == viewController &&
					trackers.TryGetValue(currentPage, out var tracker) &&
					tracker is CustomShellPageRendererTracker shellRendererTracker)
				{
					shellRendererTracker.UpdateToolbarItemsInternal(false);
				}
			}
		}
	}

	public class CustomShellPageRendererTracker : ShellPageRendererTracker
	{
		public CustomShellPageRendererTracker(IShellContext context) : base(context)
		{
			Context = context;
		}

		protected override void UpdateTitleView()
		{
			if (!ToolbarReady())
				return;

			if (ViewController == null || ViewController.NavigationItem == null)
			{
				return;
			}

			var titleView = Shell.GetTitleView(Page);
			if (titleView == null)
			{
				var view = ViewController.NavigationItem.TitleView;
				ViewController.NavigationItem.TitleView = null;
				view?.Dispose();
			}
			else
			{
				var view = new CustomTitleViewContainer(titleView);
				ViewController.NavigationItem.TitleView = view;
			}
		}

		internal void UpdateToolbarItemsInternal(bool updateWhenLoaded = true)
		{
			if (updateWhenLoaded && Page.IsLoaded || !updateWhenLoaded)
				UpdateToolbarItems();
		}

		protected override void UpdateToolbarItems()
		{
			base.UpdateToolbarItems();

			if (ViewController?.NavigationItem == null)
			{
				return;
			}

			UpdateBackButtonTitle();
		}

		protected override void UpdateTitle()
		{
			if (!ToolbarReady())
				return;

			base.UpdateTitle();
		}


		Page ToolbarCurrentPage
		{
			get
			{
				var toolBar = (Context.Shell as IToolbarElement).Toolbar;
				var t = toolBar.GetType();
				var property = t.GetField("_currentPage", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				var result = (Page)property.GetValue(toolBar);
				return result;

			}
		}

		public IShellContext Context { get; }

		bool ToolbarReady()
		{
			return ToolbarCurrentPage == Page;
		}

		void UpdateBackButtonTitle()
		{
			var behavior = Shell.GetBackButtonBehavior(Page);
			var text = behavior.GetPropertyIfSet<string>(BackButtonBehavior.TextOverrideProperty, null);

			var navController = ViewController?.NavigationController;

			if (navController != null)
			{
				var viewControllers = ViewController.NavigationController.ViewControllers;
				var count = viewControllers.Length;

				if (count > 1 && viewControllers[count - 1] == ViewController)
				{
					var previousNavItem = viewControllers[count - 2].NavigationItem;
					if (previousNavItem != null)
					{
						if (!String.IsNullOrWhiteSpace(text))
						{
							var barButtonItem = (previousNavItem.BackBarButtonItem ??= new UIBarButtonItem());
							barButtonItem.Title = text;
						}
						else if (previousNavItem.BackBarButtonItem != null)
						{
							previousNavItem.BackBarButtonItem = null;
						}
					}
				}
			}
		}
	}

	public class CustomTitleViewContainer : UIContainerView
	{
		public CustomTitleViewContainer(View view) : base(view)
		{
			TranslatesAutoresizingMaskIntoConstraints = false;
		}

		public override CGSize IntrinsicContentSize => UILayoutFittingExpandedSize;
	}

	public class NoLineAppearanceTracker : IShellNavBarAppearanceTracker
	{
		public void Dispose() { }

		public void ResetAppearance(UINavigationController controller) { }

		public void SetAppearance(UINavigationController controller, ShellAppearance appearance)
		{
			var navBar = controller.NavigationBar;
			var navigationBarAppearance = new UINavigationBarAppearance();
			navigationBarAppearance.ConfigureWithOpaqueBackground();
			navigationBarAppearance.ShadowColor = UIColor.Clear;
			navigationBarAppearance.BackgroundColor = UIColor.White; // Set the background color you want on the Shell NavBar
			navBar.ScrollEdgeAppearance = navBar.StandardAppearance = navigationBarAppearance;
		}

		public void SetHasShadow(UINavigationController controller, bool hasShadow) { }

		public void UpdateLayout(UINavigationController controller) { }
	}
}
#endif
