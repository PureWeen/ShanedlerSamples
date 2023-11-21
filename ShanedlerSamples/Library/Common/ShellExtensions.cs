using Microsoft.Maui.Handlers;

namespace ShanedlerSamples.Library.Common;
public static partial class ShellToolbarExtensions
{
    public static readonly BindableProperty ToolbarBackgroundColorProperty =
       BindableProperty.Create("ToolbarBackgroundColor", typeof(Color), typeof(Shell), null, propertyChanged: OnBackgroundColorChanged);



    static bool TryGetToolbar(Shell shell, out Toolbar toolbar)
    {
        toolbar = null;
        if (((IToolbarElement)shell).Toolbar is not Toolbar tb)
            return false;

        toolbar = tb;
        return true;
    }

    static void OnBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var shell = Shell.Current;

        if (!shell.IsLoaded)
        {
            shell.Loaded += OnShellLoaded;

            void OnShellLoaded(object sender, EventArgs e)
            {
                var shell = (Shell)sender;
                shell.Loaded -= OnShellLoaded;

                if (!TryGetToolbar(shell, out var toolbar))
                    return;

                toolbar.BarBackground = (Color)newValue;

                shell.Navigated += OnShellNavigated;
            }

            return;
        }


        if (!TryGetToolbar(shell, out var toolbar))
            return;

        toolbar.BarBackground = (Color)newValue;
    }

    static void OnShellNavigated(object sender, ShellNavigatedEventArgs e)
    {
        var shell = (Shell)sender;
        if (!TryGetToolbar(shell, out var toolbar))
            return;

        toolbar.Handler.UpdateValue("ToolbarBackgroundColor");
    }

    public static Color GetToolbarBackgroundColor(BindableObject element)
    {
        return (Color)element.GetValue(ToolbarBackgroundColorProperty);
    }

    public static void SetToolbarBackgroundColor(BindableObject element, Color value)
    {
        element.SetValue(ToolbarBackgroundColorProperty, value);
    }

    internal static void Init()
    {
        ToolbarHandler.Mapper.Add("ToolbarBackgroundColor", OnValueChanged);
        // (h, v) =>
        //{
        //#if WINDOWS
        //			((Toolbar)v).BarBackground = Colors.Fuchsia;
        //#endif
        //#if ANDROID
        //			var materialToolbar = h.PlatformView;
        //			materialToolbar.SetBackgroundColor(Colors.Fuchsia.ToPlatform());
        //			materialToolbar.TextAlignment = Android.Views.TextAlignment.TextStart;
        //			var p = materialToolbar.Title;
        //			var z = materialToolbar.TitleFormatted;
        //#endif
        //});

        ToolbarHandler.Mapper.AppendToMapping("Title", (h, v) =>
        {
            OnValueChanged(h, v);
        });
    }

    static async void OnValueChanged(IToolbarHandler handler, IToolbar toolbar)
    {
        var currentPage = Shell.Current.CurrentPage;

        if (currentPage is null)
        {
            return;
        }

        var color = GetToolbarBackgroundColor(currentPage);

        if (color is null || toolbar is not Toolbar tb)
        {
            return;
        }

        await Task.Delay(50);
        tb.BarBackground = color;
    }
}