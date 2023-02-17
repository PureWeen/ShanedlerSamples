#if ANDROID
using Android.Content;
using Android.Views;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Workarounds
{
    public class NotifyingContentViewGroup : ContentViewGroup
    {
        public event EventHandler<MotionEvent> DispatchTouch;

        public NotifyingContentViewGroup(Context context) : base(context)
        {
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            var result = base.DispatchTouchEvent(e);

            DispatchTouch?.Invoke(this, e);
            System.Diagnostics.Debug.WriteLine($"Dispatch Touch post override {e}");

            return result;
        }
    }
}
#endif