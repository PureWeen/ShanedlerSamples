using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
