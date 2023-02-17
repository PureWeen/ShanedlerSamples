
namespace Maui.FixesAndWorkarounds
{
    public static partial class HostExtensions
    {
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
