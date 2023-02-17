#if IOS || MACCATALYST
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Maui.FixesAndWorkarounds
{
    internal class CustomEntryHandler : EntryHandler
    {
        protected override bool OnShouldReturn(UITextField view)
        {
            view.InvokeOnMainThread(() =>
            {

            });

            KeyboardAutoManager.GoToNextResponderOrResign(view);
            VirtualView?.Completed();
            return false;
        }
    }
}
#endif