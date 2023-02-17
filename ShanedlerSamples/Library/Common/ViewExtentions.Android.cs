#if ANDROID
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AView = Android.Views.View;

namespace Maui.FixesAndWorkarounds.Library.Common
{
    internal static partial class ViewExtentions
    {
        internal static Size GetDesiredSizeFromHandler(this IViewHandler viewHandler, double widthConstraint, double heightConstraint)
        {
            var Context = viewHandler.MauiContext?.Context;
            var platformView = (viewHandler.ContainerView ?? viewHandler.PlatformView) as AView;
            var virtualView = viewHandler.VirtualView;

            if (platformView == null || virtualView == null || Context == null)
            {
                return Size.Zero;
            }

            // Create a spec to handle the native measure
            var widthSpec = Context.CreateMeasureSpec(widthConstraint, virtualView.Width, virtualView.MaximumWidth);
            var heightSpec = Context.CreateMeasureSpec(heightConstraint, virtualView.Height, virtualView.MaximumHeight);

            var packed = MeasureAndGetWidthAndHeight(platformView, widthSpec, heightSpec);
            var measuredWidth = (int)(packed >> 32);
            var measuredHeight = (int)(packed & 0xffffffffL);

            // Convert back to xplat sizes for the return value
            return Context.FromPixels(measuredWidth, measuredHeight);
        }

        internal static long MeasureAndGetWidthAndHeight(AView view, int widthMeasureSpec, int heightMeasureSpec)
        {
            view.Measure(widthMeasureSpec, heightMeasureSpec);
            int width = view.MeasuredWidth;
            int height = view.MeasuredHeight;
            return ((long)width << 32) | (height & 0xffffffffL);
        }
    }
}
#endif