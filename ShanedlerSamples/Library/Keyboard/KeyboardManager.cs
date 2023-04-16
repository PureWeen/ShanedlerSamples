using System;

namespace Maui.FixesAndWorkarounds
{
	public static partial class KeyboardManager
	{
#if ANDROID || IOS || MACCATALYST || WINDOWS
		public static void HideKeyboard(this IView inputView) =>
            ((IPlatformViewHandler)inputView.Handler).PlatformView.HideKeyboard();

        public static void ShowKeyboard(this IView inputView)
        {
            if (!inputView.IsFocused)
            {
                (inputView as VisualElement).Focus();
                (inputView as VisualElement).Dispatcher.Dispatch(() =>
                {
                    ((IPlatformViewHandler)inputView.Handler).PlatformView.ShowKeyboard();
                });
            }
            else
                ((IPlatformViewHandler)inputView.Handler).PlatformView.ShowKeyboard();
        }

        public static bool IsSoftKeyboardVisible(this IView view) =>
            ((IPlatformViewHandler)view.Handler).PlatformView.IsSoftKeyboardVisible();
#else

		public static void HideKeyboard(this IView _) { }

		public static void ShowKeyboard(this IView _) { }

		public static bool IsSoftKeyboardVisible(this IView _) => false;
#endif
	}
}