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

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var parent = Parent;

            while (parent != null && !parent.GetType().Name.Contains("ModalContainer"))
            {
                parent = parent.Parent;
            }

            if (parent != null)
            {
                FixMargins(parent as AView);
            }

            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
        }

        void FixMargins(AView view)
        {
            var decorView = Context?.GetActivity()?.Window?.DecorView;

            if (decorView != null && view.LayoutParameters is ViewGroup.MarginLayoutParams mlp)
            {
                var windowInsets = ViewCompat.GetRootWindowInsets(decorView);
                if (windowInsets != null)
                {
                    var barInsets = windowInsets.GetInsetsIgnoringVisibility(WindowInsetsCompat.Type.SystemBars());

                    if (mlp.TopMargin != barInsets.Top)
                        mlp.TopMargin = barInsets.Top;

                    if (mlp.LeftMargin != barInsets.Left)
                        mlp.LeftMargin = barInsets.Left;

                    if (mlp.RightMargin != barInsets.Right)
                        mlp.RightMargin = barInsets.Right;

                    if (mlp.BottomMargin != barInsets.Bottom)
                        mlp.BottomMargin = barInsets.Bottom;
                }
            }
        }
    }
}
#endif