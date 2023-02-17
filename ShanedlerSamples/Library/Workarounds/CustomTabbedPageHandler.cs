#if ANDROID
using Android.Content;
using AndroidX.ViewPager.Widget;
using AndroidX.ViewPager2.Widget;
#endif

using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
    internal class CustomTabbedPageHandler : TabbedViewHandler
    {

#if ANDROID

        MultiPageFragmentStateAdapter<Page> _adapter;
        ViewPager2 _viewPager;
        TabbedPage Element => (TabbedPage)VirtualView;

        public override void SetVirtualView(IView view)
        {

            base.SetVirtualView(view);

            var virtualView = (TabbedPage)view;
            _viewPager = PlatformView as ViewPager2;
            _viewPager.Adapter = (_adapter = new MultiPageFragmentStateAdapter<Page>(virtualView, MauiContext.GetFragmentManager(), MauiContext) { CountOverride = virtualView.Children.Count });

            ((IPageController)view).InternalChildren.CollectionChanged -= OnChildrenCollectionChanged;
        }

        void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            NotifyDataSetChanged();
        }

        void NotifyDataSetChanged()
        {
            if (_viewPager?.Adapter is MultiPageFragmentStateAdapter<Page> adapter)
            {
                var currentIndex = Element.Children.IndexOf(Element.CurrentPage);

                // If the modification to the backing collection has changed the position of the current item
                // then we need to update the viewpager so it remains selected
                if (_viewPager.CurrentItem != currentIndex && currentIndex < Element.Children.Count && currentIndex >= 0)
                    _viewPager.SetCurrentItem(Element.Children.IndexOf(Element.CurrentPage), false);

                adapter.NotifyDataSetChanged();
            }
        }
#endif
    }
}
