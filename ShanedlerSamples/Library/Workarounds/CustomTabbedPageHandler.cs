#if ANDROID
using AndroidX.ViewPager2.Widget;
#endif

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Collections.Specialized;
using System.Reflection;

namespace Maui.FixesAndWorkarounds
{
	internal class CustomTabbedPageHandler : TabbedViewHandler
	{
#if ANDROID

		CustomMultiPageFragmentStateAdapter<Page> _adapter;
		ViewPager2 _viewPager;
		TabbedPage Element => (TabbedPage)VirtualView;
		public bool IsBottomTabPlacement =>
			(Element != null) ?
				Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.TabbedPage.GetToolbarPlacement(Element.OnThisPlatform()) == Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.ToolbarPlacement.Bottom : false;
		
		public override void SetVirtualView(IView view)
		{
			((IPageController)view).InternalChildren.CollectionChanged -= OnChildrenCollectionChanged;
			((IPageController)view).InternalChildren.CollectionChanged += OnChildrenCollectionChanged;

			base.SetVirtualView(view);

			var virtualView = (TabbedPage)view;
			_viewPager = PlatformView as ViewPager2;
			_viewPager.Adapter = (_adapter = new CustomMultiPageFragmentStateAdapter<Page>(virtualView, MauiContext.GetFragmentManager(), MauiContext) { CountOverride = virtualView.Children.Count });

		}

		void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			var currentIndex = Element.Children.IndexOf(Element.CurrentPage);

			(VirtualView as BindableObject)
				.Dispatcher.Dispatch(() =>
				{
					_viewPager.Adapter = _adapter;

					if (IsBottomTabPlacement)
					{
						NotifyDataSetChanged(currentIndex);
						InvokeOnTabbedManager("UpdateIgnoreContainerAreas");
					}
					else
					{
						NotifyDataSetChanged(currentIndex);
						InvokeOnTabbedManager("UpdateTabIcons");
						InvokeOnTabbedManager("UpdateIgnoreContainerAreas");
					}
				});
		}

		void InvokeOnTabbedManager(string method)
		{
			var pageManager =
				typeof(TabbedPage)
				.GetField("_tabbedPageManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.GetValue(VirtualView);

			var methods = pageManager.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

			foreach (var thing in methods)
			{
				if (thing.Name == method)
				{
					thing.Invoke(pageManager, null);
					return;
				}
			}
		}

		void NotifyDataSetChanged(int? currentIndex)
		{
			if (_viewPager?.Adapter is CustomMultiPageFragmentStateAdapter<Page> adapter)
			{
				adapter.CountOverride = Element.Children.Count;

				currentIndex = currentIndex ?? Element.Children.IndexOf(Element.CurrentPage);

				// If the modification to the backing collection has changed the position of the current item
				// then we need to update the viewpager so it remains selected
				if (_viewPager.CurrentItem != currentIndex && currentIndex < Element.Children.Count && currentIndex >= 0)
					_viewPager.SetCurrentItem(currentIndex.Value, false);

				adapter.NotifyDataSetChanged();
			}
		}
#endif
	}
}
