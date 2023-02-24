#if IOS || MACCATALYST
using CoreGraphics;
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIKit;

namespace Maui.FixesAndWorkarounds
{
	public class CustomFrameRenderer : FrameRenderer
	{
		const int FrameBorderThickness = 1;
		public override CGSize SizeThatFits(CGSize size)
		{
			var view = Subviews.FirstOrDefault();
			if (view is null)
				return CGSize.Empty;

			var borderThickness = (Element.BorderColor.IsNotDefault() ? FrameBorderThickness : 0) * 2;

			var availableSize = new CGSize(size.Width - borderThickness, size.Height - borderThickness);
			var result = view.SizeThatFits(availableSize);

			return new CGSize(result.Width + borderThickness, result.Height + borderThickness);
		}
	}
}
#endif