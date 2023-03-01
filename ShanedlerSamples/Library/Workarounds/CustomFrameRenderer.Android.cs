#if ANDROID
using Android.Content;
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
	internal class CustomFrameRenderer : FrameRenderer, IViewHandler
	{
		const double LegacyMinimumFrameSize = 20;
		const int FrameBorderThickness = 3;

		public CustomFrameRenderer(Context context) : base(context)
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

		protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
		{
			if (Element?.Handler is IPlatformViewHandler pvh &&
				Element is IContentView cv)
			{
				var borderThickness = (Element.BorderColor.IsNotDefault() ? FrameBorderThickness : 0) * 2;

				var size = pvh.MeasureVirtualView(
					widthMeasureSpec - borderThickness,
					heightMeasureSpec - borderThickness,
					cv.CrossPlatformMeasure);
				SetMeasuredDimension((int)size.Width + borderThickness, (int)size.Height + borderThickness);
			}
			else
				base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
		}

		bool _isDispoed;
		protected override void Dispose(bool disposing)
		{
			if (_isDispoed)
				return;

			_isDispoed = true;

			if (disposing)
			{
				// remove all the children so the parent can't dispose of them
				while (ChildCount > 0)
				{
					var child = GetChildAt(0);
					child?.RemoveFromParent();
				}
			}

			// https://github.com/dotnet/maui/issues/12644
			base.Dispose(disposing);
		}
	}
}
#endif