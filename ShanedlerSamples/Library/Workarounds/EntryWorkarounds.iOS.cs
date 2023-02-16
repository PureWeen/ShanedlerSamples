#if IOS || MACCATALYST
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Shanedler.Workarounds
{
    internal class CustomEntryHandler : EntryHandler
    {
        protected override bool OnShouldReturn(UITextField view)
        {
            KeyboardAutoManager.GoToNextResponderOrResign(view);
            VirtualView?.Completed();
            return false;
        }
    }
}
#endif