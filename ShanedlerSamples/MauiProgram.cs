using Maui.FixesAndWorkarounds;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Hosting;
using System.Diagnostics.CodeAnalysis;

namespace ShanedlerSamples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureMauiWorkarounds()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddTransient<MainPage>();
        Routing.RegisterRoute("Tab1", typeof(MainPage));
        Routing.RegisterRoute("Tab2", typeof(MainPage));

        return builder.Build();
    }
}
