#if ANDROID
using AView = Android.Views.View;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.Views;
using AndroidX.Core.View;
using Android.Widget;
using Android.Runtime;

namespace Maui.FixesAndWorkarounds
{
    internal class WorkaroundPageHandler : PageHandler
    {
        protected override ContentViewGroup CreatePlatformView()
        {
            var notifying = new NotifyingContentViewGroup(Context);
            return notifying;
        }
    }
}
#endif