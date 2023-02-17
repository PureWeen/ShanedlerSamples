#if ANDROID
using Android.OS;
using Microsoft.Maui;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
    internal static class EntryWorkarounds
    {
        public static MauiAppBuilder ConfigureEntryFocusOpensKeyboard(this MauiAppBuilder builder)
        {
            ViewHandler.ViewCommandMapper.AppendToMapping(nameof(IEditor.Focus), (handler, view, args) =>
            {
                if (view is ITextInput)
                {
                    var q = Looper.MyLooper();
                    if (q != null)
                        new Handler(q).Post(RequestFocus);
                    else
                        MainThread.InvokeOnMainThreadAsync(RequestFocus);

                    void RequestFocus()
                    {
                        KeyboardManager.ShowKeyboard(view);
                    }
                }
            });

            return builder;
        }
    }
}
#endif