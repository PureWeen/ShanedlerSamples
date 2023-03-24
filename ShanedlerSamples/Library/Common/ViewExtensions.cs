#nullable enable
using Maui.FixesAndWorkarounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Microsoft.Maui.Controls
{
    public static partial class ControlsExtensions
    {
        public static void FocusAndOpenKeyboard(this VisualElement element)
        {
#if ANDROID
            KeyboardManager.ShowKeyboard(element);
#else
            element.Focus();
#endif
        }
    }
}

namespace Maui.FixesAndWorkarounds.Library.Common
{
    internal static partial class Extensions
    {
        internal static T? FindParentOfType<T>(this Element element, bool includeThis = false)
            where T : IElement
        {
            if (includeThis && element is T view)
                return view;

            foreach (var parent in element.GetParentsPath())
            {
                if (parent is T parentView)
                    return parentView;
            }

            return default;
        }

        internal static IMauiContext RequireMauiContext(this Element element, bool fallbackToAppMauiContext = false)
            => element.FindMauiContext(fallbackToAppMauiContext) ?? throw new InvalidOperationException($"{nameof(IMauiContext)} not found.");

        internal static IMauiContext? FindMauiContext(this Element element, bool fallbackToAppMauiContext = false)
        {
            if (element is IElement fe && fe.Handler?.MauiContext != null)
                return fe.Handler.MauiContext;

            foreach (var parent in element.GetParentsPath())
            {
                if (parent is IElement parentView && parentView.Handler?.MauiContext != null)
                    return parentView.Handler.MauiContext;
            }

            return fallbackToAppMauiContext ? Application.Current?.FindMauiContext() : default;
        }

        internal static IEnumerable<Element> GetParentsPath(this Element self)
        {
            Element current = self;

            while (!IsApplicationOrNull(current.RealParent))
            {
                current = current.RealParent;
                yield return current;
            }
        }

        internal static IFontManager RequireFontManager(this Element element, bool fallbackToAppMauiContext = false)
            => element.RequireMauiContext(fallbackToAppMauiContext).Services.GetRequiredService<IFontManager>();


        internal static bool IsApplicationOrNull(object? element) =>
            element == null || element is IApplication;

        internal static bool IsApplicationOrWindowOrNull(object? element) =>
            element == null || element is IApplication || element is IWindow;

		internal static void Invoke(this CommandMapper mapper, IElementHandler viewHandler, IElement? virtualView, string property, object? args)
		{
			if (virtualView == null)
				return;

			var action = mapper.GetCommand(property);
			action?.Invoke(viewHandler, virtualView, args);
		}

#if WINDOWS || IOS || MACCATALYST || ANDROID
		internal static void SetVirtualView<TElement>(
			this IElement view,
			IPlatformViewHandler nativeViewHandler,
			Action<Microsoft.Maui.Controls.Platform.ElementChangedEventArgs<TElement>> onElementChanged,
			ref TElement? currentVirtualView,
			ref IPropertyMapper _mapper,
			IPropertyMapper _defaultMapper,
			bool autoPackage)
			where TElement : Element, IView
		{
			if (currentVirtualView == view)
				return;

			var oldElement = currentVirtualView;
			currentVirtualView = view as TElement;
			onElementChanged?.Invoke(new Microsoft.Maui.Controls.Platform.ElementChangedEventArgs<TElement>(oldElement, currentVirtualView));

			_ = view ?? throw new ArgumentNullException(nameof(view));

			if (oldElement?.Handler != null)
				oldElement.Handler = null;

			currentVirtualView = (TElement)view;

			if (currentVirtualView.Handler != nativeViewHandler)
				currentVirtualView.Handler = nativeViewHandler;

			_mapper = _defaultMapper;

			if (currentVirtualView is IPropertyMapperView imv)
			{
				var map = imv.GetPropertyMapperOverrides();
				if (map is not null)
				{
					map.Chained = new[] { _defaultMapper };
					_mapper = map;
				}
			}

			if (autoPackage)
			{
				ProcessAutoPackage(view);
			}

			_mapper.UpdateProperties(nativeViewHandler, currentVirtualView);
		}
		static partial void ProcessAutoPackage(IElement element);
#endif

#if IOS || MACCATALYST
		internal static Size GetDesiredSize(this IPlatformViewHandler handler, double widthConstraint, double heightConstraint, Size? minimumSize)
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
#endif
    }
}