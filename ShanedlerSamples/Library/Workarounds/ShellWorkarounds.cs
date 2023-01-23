#if ANDROID
using Android.OS;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using AndroidX.DrawerLayout.Widget;
using AndroidX.ViewPager2.Widget;
#endif

using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;

namespace ShanedlerSamples
{
    public static partial class HostExtensions
    {
        public static MauiAppBuilder ConfigureShellWorkarounds(this MauiAppBuilder builder)
        {
#if ANDROID

            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<Shell, ShellWorkarounds>();
            });

#endif
            return builder;
        }

#if ANDROID

        public class ShellWorkarounds : ShellRenderer
        {
            protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
            {
                return new ShellSectionRendererWorkaround(this);
            }

            protected override IShellToolbarTracker CreateTrackerForToolbar(AndroidX.AppCompat.Widget.Toolbar toolbar)
            {
                return new ShellToolbarTrackerrWorkaround(this, toolbar, ((IShellContext)this).CurrentDrawerLayout);
            }


            class ShellToolbarTrackerrWorkaround : ShellToolbarTracker
            {
                DrawerLayout _drawerLayout;

                public ShellToolbarTrackerrWorkaround(
                    IShellContext shellContext,
                    AndroidX.AppCompat.Widget.Toolbar toolbar,
                    DrawerLayout drawerLayout) : base(shellContext, toolbar, drawerLayout)
                {
                    _drawerLayout = drawerLayout;
                    _drawerLayout.ViewAttachedToWindow += DrawerLayoutAttached;
                }

                void DrawerLayoutAttached(object sender, Android.Views.View.ViewAttachedToWindowEventArgs e)
                {
                    _drawerLayout.ViewAttachedToWindow -= DrawerLayoutAttached;
                    var rootView = _drawerLayout.GetChildrenOfType<CoordinatorLayout>().FirstOrDefault();
                    var fragment = (ShellSectionRendererWorkaround)AndroidX.Fragment.App.FragmentManager.FindFragment(rootView);
                    fragment.ToolbarTracker = this;
                }

                protected override void OnPageChanged(Page oldPage, Page newPage)
                {
                    base.OnPageChanged(oldPage, newPage);
                }
            }

            class ShellSectionRendererWorkaround : ShellSectionRenderer
            {
                bool _selecting;
                readonly IShellContext _shellContext;
                public ShellToolbarTrackerrWorkaround ToolbarTracker { get; set; }
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

#endif

    }
}
