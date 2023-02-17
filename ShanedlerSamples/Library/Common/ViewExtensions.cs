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
    }
}