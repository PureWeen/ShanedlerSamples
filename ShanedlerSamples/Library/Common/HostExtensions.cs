﻿using Microsoft.Maui.Controls;
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
				handlers.AddHandler(typeof(Frame), typeof(CustomFrameRenderer));
            });

				if (addAllWorkaround)
            {
                builder.ConfigureShellWorkarounds();
                builder.ConfigureTabbedPageWorkarounds();
                builder.ConfigureEntryNextWorkaround();
                builder.ConfigureKeyboardAutoScroll();
#if ANDROID
                builder.ConfigureEntryFocusOpensKeyboard();
#endif
			}

            return builder;
        }

    }
}
