using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shanedler.Workarounds
{
    public static partial class HostExtensions
    {
        public static MauiAppBuilder ConfigureShanedler(this MauiAppBuilder builder)
        {
            return builder.ConfigureShanedler(true);
        }

        public static MauiAppBuilder ConfigureShanedler(this MauiAppBuilder builder, bool addAllWorkaround)
        {
#if ANDROID
            PageHandler.PlatformViewFactory = (h) => new NotifyingContentViewGroup(h.Context);
#endif


            if (addAllWorkaround)
            {
                builder.ConfigureShellWorkarounds();
                builder.ConfigureTabbedPageWorkarounds();
                builder.ConfigureEntryNextWorkaround();
            }

            return builder;
        }

    }
}
