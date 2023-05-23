using System;
#if IOS || MACCATALYST
using UIKit;
using CoreGraphics;
#endif
using Maui.FixesAndWorkarounds.Library.Common;
using Microsoft.Maui.Controls.Handlers;
using Microsoft.Maui.Controls.Handlers.Items;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace Maui.FixesAndWorkarounds
{
	public class CustomPageHandler : PageHandler
	{
		public CustomPageHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomLayoutHandler : LayoutHandler
	{
		public CustomLayoutHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomContentViewHandler : ContentViewHandler
	{
		public CustomContentViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomButtonHandler : ButtonHandler
	{
		public CustomButtonHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomLabelHandler : LabelHandler
	{
		public CustomLabelHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomEntryHandler : EntryHandler
	{
		public CustomEntryHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);


		protected override bool OnShouldReturn(UITextField view)
		{
			KeyboardAutoManager.GoToNextResponderOrResign(view);
			VirtualView?.Completed();
			return false;
		}
#endif
	}

	public class CustomBorderHandler : BorderHandler
	{
		public CustomBorderHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomEditorHandler : EditorHandler
	{
		public CustomEditorHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomCollectionViewHandler : CollectionViewHandler
	{
		public CustomCollectionViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomCarouselViewHandler : CarouselViewHandler
	{
		public CustomCarouselViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomActivityIndicatorHandler : ActivityIndicatorHandler
	{
		public CustomActivityIndicatorHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomShapeViewHandler : ShapeViewHandler
	{
		public CustomShapeViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomCheckBoxHandler : CheckBoxHandler
	{
		public CustomCheckBoxHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomDatePickerHandler : DatePickerHandler
	{
		public CustomDatePickerHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomGraphicsViewHandler : GraphicsViewHandler
	{
		public CustomGraphicsViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomImageHandler : ImageHandler
	{
		public CustomImageHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomPickerHandler : PickerHandler
	{
		public CustomPickerHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomProgressBarHandler : ProgressBarHandler
	{
		public CustomProgressBarHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomScrollViewHandler : ScrollViewHandler
	{
		const nint ContentPanelTag = 0x845fed;

		public CustomScrollViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect)
        {
            this.PlatformArrangeHandler(rect);

			var contentView = GetContentView(PlatformView);

			if (contentView == null)
			{
				return;
			}

			var desiredSize = VirtualView.PresentedContent?.DesiredSize ?? Size.Zero;
			var scrollViewPadding = VirtualView.Padding;
			var platformViewBounds = PlatformView.Bounds;

			var contentBounds = new CGRect(0, 0,
				Math.Max(desiredSize.Width + scrollViewPadding.HorizontalThickness, platformViewBounds.Width),
				Math.Max(desiredSize.Height + scrollViewPadding.VerticalThickness, platformViewBounds.Height));

			contentView.Bounds = contentBounds;
			contentView.Center = new CGPoint(contentBounds.GetMidX(), contentBounds.GetMidY());
		}

		static Microsoft.Maui.Platform.ContentView GetContentView(UIScrollView scrollView)
		{
			for (int n = 0; n < scrollView.Subviews.Length; n++)
			{
				if (scrollView.Subviews[n] is Microsoft.Maui.Platform.ContentView contentView)
				{
					if (contentView.Tag is nint tag && tag == ContentPanelTag)
					{
						return contentView;
					}
				}
			}

			return null;
		}
#endif
	}

	public class CustomSearchBarHandler : SearchBarHandler
	{
		public CustomSearchBarHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomSliderHandler : SliderHandler
	{
		public CustomSliderHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomStepperHandler : StepperHandler
	{
		public CustomStepperHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomSwitchHandler : SwitchHandler
	{
		public CustomSwitchHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomTimePickerHandler : TimePickerHandler
	{
		public CustomTimePickerHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomWebViewHandler : WebViewHandler
	{
		public CustomWebViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomLineHandler : LineHandler
	{
		public CustomLineHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomPathHandler : PathHandler
	{
		public CustomPathHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomPolygonHandler : PolygonHandler
	{
		public CustomPolygonHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomPolylineHandler : PolylineHandler
	{
		public CustomPolylineHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomRectangleHandler : RectangleHandler
	{
		public CustomRectangleHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomRoundRectangleHandler : RoundRectangleHandler
	{
		public CustomRoundRectangleHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomImageButtonHandler : ImageButtonHandler
	{
		public CustomImageButtonHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomIndicatorViewHandler : IndicatorViewHandler
	{
		public CustomIndicatorViewHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}

	public class CustomRadioButtonHandler : RadioButtonHandler
	{
		public CustomRadioButtonHandler()
		{
		}

#if IOS || MACCATALYST
        public override void PlatformArrange(Rect rect) =>
            this.PlatformArrangeHandler(rect);
#endif
	}
}

