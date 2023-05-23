using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
	public static partial class HostExtensions
	{
		public static MauiAppBuilder ConfigureRTLFixes(this MauiAppBuilder builder)
		{
			builder.ConfigureMauiHandlers(handlers =>
			{
#if IOS || MACCATALYST
				handlers.AddHandler(typeof(Microsoft.Maui.ILayout), typeof(CustomLayoutHandler));
				handlers.AddHandler(typeof(Layout), typeof(CustomLayoutHandler));
				handlers.AddHandler(typeof(Page), typeof(CustomPageHandler));
				handlers.AddHandler(typeof(ContentView), typeof(CustomContentViewHandler));
				handlers.AddHandler(typeof(Button), typeof(CustomButtonViewHandler));
				handlers.AddHandler(typeof(Label), typeof(CustomLabelViewHandler));
				handlers.AddHandler(typeof(Entry), typeof(CustomEntryViewHandler));
#endif
			});
			return builder;
		}

		public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder)
		{
			return builder.ConfigureMauiWorkarounds(true);
		}

		public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder, bool addAllWorkaround)
		{
			builder.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler(typeof(Page), typeof(WorkaroundPageHandler));
#endif

#if ANDROID || IOS || MACCATALYST || WINDOWS
				handlers.AddHandler(typeof(Frame), typeof(CustomFrameRenderer));
#endif
			});

			if (addAllWorkaround)
			{
				builder.ConfigureShellWorkarounds();
				builder.ConfigureTabbedPageWorkarounds();
				builder.ConfigureEntryNextWorkaround();
				builder.ConfigureKeyboardAutoScroll();
				builder.ConfigureFlyoutPageWorkarounds();
				builder.ConfigureRTLFixes();
#if ANDROID
				builder.ConfigureEntryFocusOpensKeyboard();
#endif
			}

			return builder;
		}

	}
}
