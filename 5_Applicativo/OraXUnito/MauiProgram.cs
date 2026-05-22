using Microsoft.Extensions.Logging;
using OraX.Services;

namespace OraX;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("SFNSDisplay-Medium.otf", "font");
                fonts.AddFont("SFUIDisplay-Regular.otf", "fontRegular");
            });

        builder.Services.AddSingleton<DatabaseService>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<CalendarPageV2>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}