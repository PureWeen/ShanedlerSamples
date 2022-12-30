#if IOS
using System;
using UIKit;

namespace ShanedlerSamples
{
    public static partial class KeyboardManager
    {
        public static void HideKeyboard(this UIView inputView) => inputView.ResignFirstResponder();

        public static void ShowKeyboard(this UIView inputView) => inputView.BecomeFirstResponder();

        public static bool IsSoftKeyboardVisible(this UIView inputView) => inputView.IsFirstResponder;
    }
}
#endif