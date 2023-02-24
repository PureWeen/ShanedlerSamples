#if WINDOWS

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
		const int FrameBorderThickness = 1;
		protected override global::Windows.Foundation.Size MeasureOverride(global::Windows.Foundation.Size availableSize)
		{
			if (Element is IContentView cv)
			{
				// If there's a border specified, include the thickness in our measurements
				// multiplied by 2 to account for both sides (left/right or top/bot)
				var borderThickness = (Element.BorderColor.IsNotDefault() ? FrameBorderThickness : 0) * 2;

				// Measure content but subtract border from available space
				var measureContent = cv.CrossPlatformMeasure(
					availableSize.Width - borderThickness,
					availableSize.Height - borderThickness).ToPlatform();

				// Add the border space to the final calculation
				measureContent = new Size(
					measureContent.Width + borderThickness,
					measureContent.Height + borderThickness).ToPlatform();

				return measureContent;
			}

			return MinimumSize().ToPlatform();
		}
	}
}
#endif