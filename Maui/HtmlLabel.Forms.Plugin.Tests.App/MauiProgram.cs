using HyperTextLabel.Maui.Hosting;

namespace HyperTextLabel.Maui.Tests.App;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureHyperTextLabel()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Allura-Regular.otf", "AlluraRegular");
			});

		return builder.Build();
	}
}
