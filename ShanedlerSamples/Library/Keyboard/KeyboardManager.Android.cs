#if ANDROID
using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using TextView = Android.Widget.TextView;
using EditText = Android.Widget.EditText;
using AndroidX.AppCompat.Widget;
using AndroidX.Core.View;
using Google.Android.Material.TextField;
using Microsoft.Maui.Platform;
using AView = Android.Views.View;

namespace Shanedler.Workarounds
{
    public static partial class KeyboardManager
    {
        internal static void HideKeyboard(this AView inputView, bool overrideValidation = false)
        {
            if (inputView?.Context == null)
                throw new ArgumentNullException(nameof(inputView) + " must be set before the keyboard can be hidden.");

            using (var inputMethodManager = (InputMethodManager)inputView.Context.GetSystemService(Context.InputMethodService)!)
            {
                if (!overrideValidation && !(inputView is EditText || inputView is TextView || inputView is SearchView))
                    throw new ArgumentException("inputView should be of type EditText, SearchView, or TextView");

                var windowToken = inputView.WindowToken;
                if (windowToken != null && inputMethodManager != null)
                    inputMethodManager.HideSoftInputFromWindow(windowToken, HideSoftInputFlags.None);
            }
        }

        internal static void ShowKeyboard(this TextView inputView)
        {
            if (inputView?.Context == null)
                throw new ArgumentNullException(nameof(inputView) + " must be set before the keyboard can be shown.");

            using (var inputMethodManager = (InputMethodManager)inputView.Context.GetSystemService(Context.InputMethodService)!)
            {
                // The zero value for the second parameter comes from 
                // https://developer.android.com/reference/android/view/inputmethod/InputMethodManager#showSoftInput(android.view.View,%20int)
                // Apparently there's no named value for zero in this case
                inputMethodManager?.ShowSoftInput(inputView, 0);
            }
        }

        internal static void ShowKeyboard(this SearchView searchView)
        {
            if (searchView?.Context == null || searchView?.Resources == null)
            {
                throw new ArgumentNullException(nameof(searchView));
            }

            var queryEditor = searchView.GetFirstChildOfType<EditText>();

            if (queryEditor == null)
                return;

            using (var inputMethodManager = (InputMethodManager)searchView.Context.GetSystemService(Context.InputMethodService)!)
            {
                // The zero value for the second parameter comes from 
                // https://developer.android.com/reference/android/view/inputmethod/InputMethodManager#showSoftInput(android.view.View,%20int)
                // Apparently there's no named value for zero in this case
                inputMethodManager?.ShowSoftInput(queryEditor, 0);
            }
        }

        internal static void ShowKeyboard(this AView view)
        {
            switch (view)
            {
                case SearchView searchView:
                    searchView.ShowKeyboard();
                    break;
                case TextView textView:
                    textView.ShowKeyboard();
                    break;
            }
        }

        internal static void PostShowKeyboard(this AView view)
        {
            void ShowKeyboard()
            {
                // Since we're posting this on the queue, it's possible something else will have disposed of the view
                // by the time the looper is running this, so we have to verify that the view is still usable
                if (view.IsDisposed())
                {
                    return;
                }

                view.ShowKeyboard();
            };

            view.Post(ShowKeyboard);
        }

        public static bool IsSoftKeyboardVisible(this AView view)
        {
            var insets = ViewCompat.GetRootWindowInsets(view);
            if (insets == null)
                return false;

            var result = insets.IsVisible(WindowInsetsCompat.Type.Ime());
            return result;
        }
    }
}
#endif