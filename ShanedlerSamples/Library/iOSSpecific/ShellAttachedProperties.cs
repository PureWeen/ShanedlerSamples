using Maui.FixesAndWorkarounds;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Handlers;

namespace Maui.FixesAndWorkarounds.iOSSpecific;

public static class ShellAttachedProperties
{
    public static readonly BindableProperty PrefersLargeTitlesProperty =
        BindableProperty.Create("PrefersLargeTitles", typeof(bool), typeof(ShellAttachedProperties), false);

    public static bool GetPrefersLargeTitles(BindableObject element)
    {
        return (bool)element.GetValue(PrefersLargeTitlesProperty);
    }

    public static void SetPrefersLargeTitles(BindableObject element, bool value)
    {
        element.SetValue(PrefersLargeTitlesProperty, value);
    }

    public static IPlatformElementConfiguration<iOS, Shell> SetPrefersLargeTitles(this IPlatformElementConfiguration<iOS, Shell> config, bool value)
    {
        SetPrefersLargeTitles(config.Element, value);
        return config;
    }

    public static bool PrefersLargeTitles(this IPlatformElementConfiguration<iOS, Shell> config)
    {
        return GetPrefersLargeTitles(config.Element);
    }
}