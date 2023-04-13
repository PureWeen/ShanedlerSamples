#if ANDROID
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.View;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AView = Android.Views.View;

namespace Maui.FixesAndWorkarounds
{
    public class NotifyingContentViewGroup : ContentViewGroup
    {
        public event EventHandler<MotionEvent> DispatchTouch;

        public NotifyingContentViewGroup(Context context) : base(context)
        {
            SetClipChildren(false);
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            var result = base.DispatchTouchEvent(e);

            DispatchTouch?.Invoke(this, e);

            return result;
        }
    }
}
#endif