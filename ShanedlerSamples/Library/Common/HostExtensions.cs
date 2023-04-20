using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.FixesAndWorkarounds
{
	public static partial class HostExtensions
	{
		public static MauiAppBuilder ConfigureInputTransparentFixes(this MauiAppBuilder builder)
		{
			builder.ConfigureMauiHandlers(_ =>
			{
				ViewHandler.ViewMapper.ModifyMapping(nameof(ViewHandler.ContainerView), (handler, view, action) =>
				{
					action.Invoke(handler, view);

#if ANDROID
					if (handler.ContainerView is Microsoft.Maui.Platform.WrapperView wrapper)
						wrapper.InputTransparent = false;
#endif
				});

				ViewHandler.ViewMapper.ModifyMapping(nameof(IView.InputTransparent), (handler, view, action) =>
				{
					action.Invoke(handler, view);
#if WINDOWS
					if (handler.PlatformView is Microsoft.Maui.Platform.LayoutPanel lp)
					{
						lp.IsHitTestVisible = true;
					}
#endif

					if (handler is ILayoutHandler && view is Microsoft.Maui.ILayout)
					{

					}
					else
					{
						VisualElement.ControlsVisualElementMapper.UpdateProperty(handler, view, nameof(IView.InputTransparent));
					}
				});

				ViewHandler.ViewMapper.ModifyMapping(nameof(Layout.CascadeInputTransparent), (handler, view, action) =>
				{
					action.Invoke(handler, view);
#if WINDOWS
					if (handler.PlatformView is Microsoft.Maui.Platform.LayoutPanel lp)
					{
						lp.IsHitTestVisible = true;
					}
#endif

					if (handler is ILayoutHandler && view is Microsoft.Maui.ILayout)
					{

					}
					else
					{
						VisualElement.ControlsVisualElementMapper.UpdateProperty(handler, view, nameof(Layout.CascadeInputTransparent));
					}
				});
			});

			return builder.ConfigureMauiWorkarounds(true);
		}

		public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder)
		{
			return builder.ConfigureMauiWorkarounds(true);
		}

		public static MauiAppBuilder ConfigureMauiWorkarounds(this MauiAppBuilder builder, bool addAllWorkaround)
		{
			builder.ConfigureMauiHandlers(handlers =>
			{
#if ANDROID
				handlers.AddHandler(typeof(Page), typeof(WorkaroundPageHandler));
#endif

#if ANDROID || IOS || MACCATALYST || WINDOWS
				handlers.AddHandler(typeof(Frame), typeof(CustomFrameRenderer));
#endif
			});

			if (addAllWorkaround)
			{
				builder.ConfigureInputTransparentFixes();
				builder.ConfigureShellWorkarounds();
				builder.ConfigureTabbedPageWorkarounds();
				builder.ConfigureEntryNextWorkaround();
				builder.ConfigureKeyboardAutoScroll();
				builder.ConfigureFlyoutPageWorkarounds();
#if ANDROID
				builder.ConfigureEntryFocusOpensKeyboard();
#endif
			}

			return builder;
		}

	}
}
