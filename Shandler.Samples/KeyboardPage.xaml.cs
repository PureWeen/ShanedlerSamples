using Maui.FixesAndWorkarounds;
using Microsoft.Maui.Platform;

namespace Shandler.Samples;

public partial class KeyboardPage : ContentPage
{
    public KeyboardPage()
    {
        InitializeComponent();
    }

    private void OnToggleKeyboard(object sender, EventArgs e)
    {
        if (!KeyboardManager.IsSoftKeyboardVisible(inputField))
        {
            KeyboardManager.ShowKeyboard(inputField);
        }
        else
        {
            KeyboardManager.HideKeyboard(inputField);
        }
    }

    private void OnEntryLoaded(object sender, EventArgs e)
    {
        KeyboardManager.ShowKeyboard(firstFocusMe);
    }
}