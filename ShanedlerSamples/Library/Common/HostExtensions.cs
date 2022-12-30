using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanedlerSamples
{
    public static class HostExtensions
    {

        public static MauiAppBuilder ConfigureShanedler(this MauiAppBuilder builder)
        {
#if ANDROID

            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<Entry, Platforms.Android.MaterialEntryHandler>();
            });

            PageHandler.PlatformViewFactory = (h) => new NotifyingContentViewGroup(h.Context);
#endif
            return builder;
        }

    }
}
