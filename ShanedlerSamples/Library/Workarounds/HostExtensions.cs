
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.LifecycleEvents;

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
				return new MauiScrollView();
			};

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<Entry, CustomEntryHandler>();
			});

			builder.ConfigureLifecycleEvents(events =>
			{
				events.AddiOS((config) =>
				{
					config
					.OnActivated((window) =>
					 {
						 if (KeyboardAutoManagerScroll.TextFieldToken is null)
							 KeyboardAutoManagerScroll.Init();
					 })
					.WillTerminate(app =>
					{
						// By this point if we were a multi window app, the GetWindow would be null anyway
						app.GetWindow()?.Destroying();
						KeyboardAutoManagerScroll.Destroy();
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

		public static MauiAppBuilder ConfigureShellWorkarounds(this MauiAppBuilder builder)
		{
#if ANDROID || IOS

			builder.ConfigureMauiHandlers(handlers =>
			{
				handlers.AddHandler<Shell, ShellWorkarounds>();
			});

#endif
			return builder;
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
