using Maui.FixesAndWorkarounds;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

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

//		ToolbarHandler.Mapper.AppendToMapping(nameof(Toolbar.BarBackground), (h, v) =>
//		{
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
//		});

		return builder.Build();
	}
}
