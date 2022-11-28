using System;

namespace Microsoft.Maui.Platform
{
    public static partial class KeyboardManager
    {
        public static void HideKeyboard(this IView inputView) =>
            ((IPlatformViewHandler)inputView.Handler).PlatformView.HideKeyboard();

        public static void ShowKeyboard(this IView inputView) =>
            ((IPlatformViewHandler)inputView.Handler).PlatformView.ShowKeyboard();

        public static bool IsSoftKeyboardVisible(this IView view) =>
            ((IPlatformViewHandler)view.Handler).PlatformView.IsSoftKeyboardVisible();
    }
}