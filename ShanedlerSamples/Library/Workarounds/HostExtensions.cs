
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Controls;
#if ANDROID || IOS || MACCATALYST || WINDOWS
using Microsoft.Maui.Controls.Handlers.Compatibility;
#endif

using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;
using System.Collections;
using System.Reflection;

namespace Maui.FixesAndWorkarounds
{
	public static partial class HostExtensions
	{
		/// <summary>
		/// This currently doesn't work with ListView or TableView
		/// </summary>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static MauiAppBuilder ConfigureKeyboardAutoScroll(this MauiAppBuilder builder)
		{
#if IOS
			ScrollViewHandler.PlatformViewFactory = (_) =>
			{
				return new WorkaroundMauiScrollView();
			};

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<TableView, CustomTableViewRenderer>();
			});

			ListViewRenderer.Mapper.Add("RemoveKeyboardInset", (handler, view) =>
			{
				var property = typeof(ListViewRenderer).GetField("_insetTracker", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				var result = property?.GetValue(handler) as IDisposable;

				if (result != null)
				{
					result.Dispose();
					property.SetValue(handler, null);
				}
			});

			builder.ConfigureLifecycleEvents(events =>
			{
				events.AddiOS((config) =>
				{
					config
					.OnActivated((window) =>
					 {
						 KeyboardAutoManagerScroll.Connect();
					 })
					.WillTerminate(app =>
					{
						KeyboardAutoManagerScroll.Disconnect();
					});
				});
			});

#endif
			return builder;
		}

		public static MauiAppBuilder ConfigureEntryNextWorkaround(this MauiAppBuilder builder)
		{
#if IOS || MACCATALYST

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<Entry, CustomEntryHandler>();
			});

#endif
			return builder;
		}

#if !ANDROID
		public static MauiAppBuilder ConfigureFlyoutPageWorkarounds(this MauiAppBuilder builder)
		{
#if IOS || MACCATALYST

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<FlyoutPage, CustomPhoneFlyoutPageRenderer>();
			});
#endif
			return builder;
		}
#endif

		static bool toolbarWorkaroundsConfigured = false;
		public static MauiAppBuilder ConfigureToolbarWorkarounds(this MauiAppBuilder builder)
		{
			if (toolbarWorkaroundsConfigured)
				return builder;

			toolbarWorkaroundsConfigured = true;

#if ANDROID || IOS || MACCATALYST

			builder.ConfigureMauiHandlers(handlers =>
			{
				bool fixing = false;
				ToolbarHandler.Mapper.ModifyMapping("ToolbarItems", (handler, toolbar, action) =>
				{
					if (Shell.Current is null)
					{
						action.Invoke(handler, toolbar);

#if ANDROID
						if (toolbar.Handler?.PlatformView is Google.Android.Material.AppBar.MaterialToolbar mt)
						{
							var tracker =
								toolbar
									.GetType()
									.GetField("_toolbarTracker", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
									.GetValue(toolbar);

							var toolbarItems =
								tracker
									.GetType()
									.GetProperty("ToolbarItems", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
									.GetValue(tracker) as IList;

							for (int i = 0; i < toolbarItems.Count && i < mt.Menu.Size(); i++)
							{
								if (((ToolbarItem)toolbarItems[i]).Order != ToolbarItemOrder.Secondary)
									continue;

								var menuItem = mt.Menu.GetItem(i);
								menuItem.SetShowAsAction(Android.Views.ShowAsAction.Never);
							}
						}
#endif
						return;
					}

					if (fixing)
					{
						action?.Invoke(handler, toolbar);
					}
					else
					{
						fixing = true;
						var bar = (Shell.Current as IToolbarElement).Toolbar;
						bar.GetType().GetMethod("ApplyChanges", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
						.Invoke(bar, null);
						fixing = false;
					}
				});
			});

#endif
			return builder;
		}

		public static MauiAppBuilder ConfigureShellWorkarounds(this MauiAppBuilder builder)
		{
#if ANDROID || IOS || MACCATALYST

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<Shell, ShellWorkarounds>();
			});

#endif
			return builder.ConfigureToolbarWorkarounds();
		}

		public static MauiAppBuilder ConfigureTabbedPageWorkarounds(this MauiAppBuilder builder)
		{
#if ANDROID

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<TabbedPage, CustomTabbedPageHandler>();
			});

#endif
			return builder;
		}
	}

}
