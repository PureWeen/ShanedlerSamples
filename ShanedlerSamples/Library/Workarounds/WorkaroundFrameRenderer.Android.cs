#if ANDROID
using Android.Content;
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
    internal class WorkaroundFrameRenderer : FrameRenderer, IViewHandler
    {
        const double LegacyMinimumFrameSize = 20;

        public WorkaroundFrameRenderer(Context context) : base(context)
        {
        }

        Size IViewHandler.GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var virtualView = (this as IViewHandler)?.VirtualView;
            if (virtualView is null)
            {
                return Size.Zero;
            }

            var minWidth = virtualView.MinimumWidth;
            var minHeight = virtualView.MinimumHeight;

            if (!Microsoft.Maui.Primitives.Dimension.IsExplicitSet(minWidth))
            {
                minWidth = LegacyMinimumFrameSize;
            }

            if (!Microsoft.Maui.Primitives.Dimension.IsExplicitSet(minHeight))
            {
                minHeight = LegacyMinimumFrameSize;
            }

            return GetDesiredSize(this, widthConstraint, heightConstraint,
                new Size(minWidth, minHeight));
        }

        internal static Size GetDesiredSize(IPlatformViewHandler handler, double widthConstraint, double heightConstraint, Size? minimumSize)
        {
            var size = handler.GetDesiredSizeFromHandler(widthConstraint, heightConstraint);

            if (minimumSize != null)
            {
                var minSize = minimumSize.Value;

                if (size.Height < minSize.Height || size.Width < minSize.Width)
                {
                    return new Size(
                            size.Width < minSize.Width ? minSize.Width : size.Width,
                            size.Height < minSize.Height ? minSize.Height : size.Height
                        );
                }
            }

            return size;
        }
    }
}
#endif