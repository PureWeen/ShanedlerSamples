using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Workarounds
{
    public static partial class HostExtensions
    {
        public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder)
        {
            return builder.ConfigureMauiWorkarounds(true);
        }

        public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder, bool addAllWorkaround)
        {
#if ANDROID
            PageHandler.PlatformViewFactory = (h) => new NotifyingContentViewGroup(h.Context);
#endif


            if (addAllWorkaround)
            {
                builder.ConfigureShellWorkarounds();
                builder.ConfigureTabbedPageWorkarounds();
                builder.ConfigureEntryNextWorkaround();
#if ANDROID
                builder.ConfigureEntryFocusOpensKeyboard();
#endif
            }

            return builder;
        }

    }
}
