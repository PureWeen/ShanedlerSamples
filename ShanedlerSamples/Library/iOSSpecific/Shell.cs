#if IOS
using Maui.FixesAndWorkarounds;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;
using FormsElement = Microsoft.Maui.Controls.Shell;

namespace ShanedlerSamples.Library.iOSSpecific;

public static class Shell
{
    public static readonly BindableProperty PrefersLargeTitlesProperty =
        BindableProperty.Create("PrefersLargeTitles", typeof(bool), typeof(Shell), false);

    public static bool GetPrefersLargeTitles(BindableObject element)
    {
        return (bool)element.GetValue(PrefersLargeTitlesProperty);
    }

    public static void SetPrefersLargeTitles(BindableObject element, bool value)
    {
        element.SetValue(PrefersLargeTitlesProperty, value);
    }

    public static IPlatformElementConfiguration<iOS, FormsElement> SetPrefersLargeTitles(this IPlatformElementConfiguration<iOS, FormsElement> config, bool value)
    {
        SetPrefersLargeTitles(config.Element, value);
        return config;
    }

    public static bool PrefersLargeTitles(this IPlatformElementConfiguration<iOS, FormsElement> config)
    {
        return GetPrefersLargeTitles(config.Element);
    }
}
#endif