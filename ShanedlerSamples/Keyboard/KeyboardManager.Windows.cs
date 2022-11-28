#if WINDOWS
using Microsoft.UI.Xaml;
using System;

namespace Microsoft.Maui.Platform
{
    public static partial class KeyboardManager
    {
        public static void HideKeyboard(this FrameworkElement inputView) => {};

        public static void ShowKeyboard(this FrameworkElement inputView) => {};

        public static bool IsSoftKeyboardVisible(this FrameworkElement inputView) => {};
    }
}
#endif