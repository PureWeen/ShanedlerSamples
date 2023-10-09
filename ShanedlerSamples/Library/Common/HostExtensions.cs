using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using ShanedlerSamples.Library.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapes = Microsoft.Maui.Controls.Shapes;

namespace Maui.FixesAndWorkarounds
{
	public static partial class HostExtensions
	{
		public static MauiAppBuilder ConfigureRTLFixes(this MauiAppBuilder builder)
		{
			builder.ConfigureMauiHandlers(handlers =>
			{
#if IOS || MACCATALYST

				handlers.AddHandler(typeof(Microsoft.Maui.ILayout), typeof(CustomLayoutHandler));


				handlers.AddHandler<CollectionView, CustomCollectionViewHandler>();
				handlers.AddHandler<CarouselView, CustomCarouselViewHandler>();
				handlers.AddHandler<ActivityIndicator, CustomActivityIndicatorHandler>();
				handlers.AddHandler<BoxView, CustomShapeViewHandler>();
				handlers.AddHandler<Button, CustomButtonHandler>();
				handlers.AddHandler<CheckBox, CustomCheckBoxHandler>();
				handlers.AddHandler<DatePicker, CustomDatePickerHandler>();
				handlers.AddHandler<Editor, CustomEditorHandler>();
				handlers.AddHandler<Entry, CustomEntryHandler>();
				handlers.AddHandler<GraphicsView, CustomGraphicsViewHandler>();
				handlers.AddHandler<Image, CustomImageHandler>();
				handlers.AddHandler<Label, CustomLabelHandler>();
				handlers.AddHandler<Layout, CustomLayoutHandler>();
				handlers.AddHandler<Picker, CustomPickerHandler>();
				handlers.AddHandler<ProgressBar, CustomProgressBarHandler>();
				handlers.AddHandler<ScrollView, CustomScrollViewHandler>();
				handlers.AddHandler<SearchBar, CustomSearchBarHandler>();
				handlers.AddHandler<Slider, CustomSliderHandler>();
				handlers.AddHandler<Stepper, CustomStepperHandler>();
				handlers.AddHandler<Switch, CustomSwitchHandler>();
				handlers.AddHandler<TimePicker, CustomTimePickerHandler>();
				handlers.AddHandler<Page, CustomPageHandler>();
				handlers.AddHandler<WebView, CustomWebViewHandler>();
				handlers.AddHandler<Border, CustomBorderHandler>();
				handlers.AddHandler<IContentView, CustomContentViewHandler>();
				handlers.AddHandler<Shapes.Ellipse, CustomShapeViewHandler>();
				handlers.AddHandler<Shapes.Line, CustomLineHandler>();
				handlers.AddHandler<Shapes.Path, CustomPathHandler>();
				handlers.AddHandler<Shapes.Polygon, CustomPolygonHandler>();
				handlers.AddHandler<Shapes.Polyline, CustomPolylineHandler>();
				handlers.AddHandler<Shapes.Rectangle, CustomRectangleHandler>();
				handlers.AddHandler<Shapes.RoundRectangle, CustomRoundRectangleHandler>();
				handlers.AddHandler<ImageButton, CustomImageButtonHandler>();
				handlers.AddHandler<IndicatorView, CustomIndicatorViewHandler>();
				handlers.AddHandler<RadioButton, CustomRadioButtonHandler>();

				handlers.AddHandler(typeof(Layout), typeof(CustomLayoutHandler));
				handlers.AddHandler(typeof(Page), typeof(CustomPageHandler));
				handlers.AddHandler(typeof(ContentView), typeof(CustomContentViewHandler));
#endif
			});
			return builder;
		}

		public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder)
		{
			return builder.ConfigureMauiWorkarounds(true);
		}

		public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder, bool addAllWorkaround)
		{
			ShellToolbarExtensions.Init();
			builder.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler(typeof(Page), typeof(WorkaroundPageHandler));
#endif

#if ANDROID || IOS || MACCATALYST || WINDOWS
				handlers.AddHandler(typeof(Frame), typeof(CustomFrameRenderer));
#endif

#if IOS || MACCATALYST
				PageHandler.PlatformViewFactory = (handler) =>
				{
					var vc = new KeyboardPageViewController(handler.VirtualView, handler.MauiContext);
					handler.ViewController = vc;
					return (Microsoft.Maui.Platform.ContentView)vc.View.Subviews[0];
				};
#endif
            });

			if (addAllWorkaround)
			{
				builder.ConfigureShellWorkarounds();
				builder.ConfigureTabbedPageWorkarounds();
				builder.ConfigureEntryNextWorkaround();
				builder.ConfigureKeyboardAutoScroll();
				builder.ConfigureFlyoutPageWorkarounds();
				builder.ConfigureRTLFixes();
#if ANDROID
				builder.ConfigureEntryFocusOpensKeyboard();
#endif
			}

			return builder;
		}

	}
}
