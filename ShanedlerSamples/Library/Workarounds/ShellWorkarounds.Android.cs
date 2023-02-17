#if ANDROID
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.DrawerLayout.Widget;
using AndroidX.ViewPager2.Widget;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls;

using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;
using System.ComponentModel;

namespace Maui.FixesAndWorkarounds
{

    public class ShellWorkarounds : ShellRenderer
    {
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {
            return new ShellSectionRendererWorkaround(this);
        }

        protected override IShellToolbarTracker CreateTrackerForToolbar(AndroidX.AppCompat.Widget.Toolbar toolbar)
        {
            return new ShellToolbarTrackerWorkaround(this, toolbar, ((IShellContext)this).CurrentDrawerLayout);
        }

        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        {
            return new ShellItemRendererWorkaround(this);
        }

        class ShellItemRendererWorkaround : ShellItemRenderer
        {
            IMauiContext MauiContext => ShellContext.Shell.Handler.MauiContext;

            BottomNavigationView _bottomView;
            public ShellItemRendererWorkaround(IShellContext shellContext) : base(shellContext)
            {
            }

            public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = base.OnCreateView(inflater, container, savedInstanceState);
                _bottomView = (view as ViewGroup).GetChildrenOfType<BottomNavigationView>().FirstOrDefault();
                return view;
            }

            protected override void OnShellSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == BaseShellItem.IconProperty.PropertyName ||
                    e.PropertyName == BaseShellItem.TitleProperty.PropertyName)
                {
                    var content = (ShellSection)sender;
                    var index = ((IShellItemController)ShellItem).GetItems().IndexOf(content);

                    var itemCount = ((IShellItemController)ShellItem).GetItems().Count;
                    var maxItems = _bottomView.MaxItemCount;

                    if (itemCount > maxItems && index > maxItems - 2)
                        return;

                    var menuItem = _bottomView.Menu.FindItem(index);

                    if (e.PropertyName == BaseShellItem.IconProperty.PropertyName)
                    {
                        UpdateShellSectionIcon(content, menuItem);
                    }
                    else
                        UpdateShellSectionTitle(content, menuItem);
                }
                else
                {
                    base.OnShellSectionPropertyChanged(sender, e);
                }
            }

            async void UpdateShellSectionIcon(ShellSection shellSection, IMenuItem menuItem)
            {
                await SetMenuItemIcon(menuItem, shellSection.Icon, MauiContext);
            }

            void UpdateShellSectionTitle(ShellSection shellSection, IMenuItem menuItem)
            {
                using (var title = new Java.Lang.String(shellSection.Title))
                {
                    menuItem.SetTitle(title);
                }
            }

            internal static async Task SetMenuItemIcon(IMenuItem menuItem, ImageSource source, IMauiContext context)
            {
                if (menuItem.Handle == IntPtr.Zero)
                    return;

                if (source == null)
                    return;

                var services = context.Services;
                var provider = services.GetRequiredService<IImageSourceServiceProvider>();
                var imageSourceService = provider.GetRequiredImageSourceService(source);

                var result = await imageSourceService.GetDrawableAsync(
                    source,
                    context.Context);

                if (menuItem.Handle != IntPtr.Zero)
                    menuItem.SetIcon(result.Value);
            }
        }

        class ShellToolbarTrackerWorkaround : ShellToolbarTracker
        {
            DrawerLayout _drawerLayout;

            public static ShellToolbarTrackerWorkaround Current { get; set; }

            public ShellToolbarTrackerWorkaround(
                IShellContext shellContext,
                AndroidX.AppCompat.Widget.Toolbar toolbar,
                DrawerLayout drawerLayout) : base(shellContext, toolbar, drawerLayout)
            {
                Current = this;
                _drawerLayout = drawerLayout;
            }
        }

        class ShellSectionRendererWorkaround : ShellSectionRenderer
        {
            bool _selecting;
            readonly IShellContext _shellContext;
            public ShellToolbarTrackerWorkaround ToolbarTracker { get; set; }
            ViewPager2 _viewPager;
            IShellSectionController SectionController => (IShellSectionController)ShellSection;
            IShellController ShellController => _shellContext.Shell;

            public ShellSectionRendererWorkaround(IShellContext shellContext) : base(shellContext)
            {
                _shellContext = shellContext;
            }

            public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = base.OnCreateView(inflater, container, savedInstanceState);
                ToolbarTracker = ShellToolbarTrackerWorkaround.Current;
                ShellToolbarTrackerWorkaround.Current = null;
                _viewPager = (view as ViewGroup).GetChildrenOfType<ViewPager2>().FirstOrDefault();
                return view;
            }

            protected override void OnPageSelected(int position)
            {
                if (_selecting)
                    return;

                var shellSection = ShellSection;
                var visibleItems = SectionController.GetItems();

                // This mainly happens if all of the items that are part of this shell section 
                // vanish. Android calls `OnPageSelected` with position zero even though the view pager is
                // empty
                if (position >= visibleItems.Count)
                    return;

                var shellContent = visibleItems[position];

                if (shellContent == shellSection.CurrentItem)
                    return;

                var stack = shellSection.Stack.ToList();
                bool result = ShellController.ProposeNavigation(ShellNavigationSource.ShellContentChanged,
                    (ShellItem)shellSection.Parent, shellSection, shellContent, stack, true);

                if (result)
                {
                    UpdateCurrentItem(shellContent);
                }
                else if (shellSection?.CurrentItem != null)
                {
                    var currentPosition = visibleItems.IndexOf(shellSection.CurrentItem);
                    _selecting = true;

                    // Android doesn't really appreciate you calling SetCurrentItem inside a OnPageSelected callback.
                    // It wont crash but the way its programmed doesn't really anticipate re-entrancy around that method
                    // and it ends up going to the wrong location. Thus we must invoke.

                    _viewPager.Post(() =>
                    {
                        if (currentPosition < _viewPager.ChildCount && ToolbarTracker != null)
                        {
                            _viewPager.SetCurrentItem(currentPosition, false);
                            UpdateCurrentItem(shellSection.CurrentItem);
                        }

                        _selecting = false;
                    });
                }
            }

            void UpdateCurrentItem(ShellContent content)
            {
                if (ToolbarTracker == null)
                    return;

                var page = ((IShellContentController)content).GetOrCreateContent();
                if (page == null)
                    throw new ArgumentNullException(nameof(page), "Shell Content Page is Null");

                ShellSection.SetValueFromRenderer(ShellSection.CurrentItemProperty, content);
                ToolbarTracker.Page = page;
            }
        }

    }
}

#endif
