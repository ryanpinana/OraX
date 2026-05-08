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

        // Singleton: una sola istanza condivisa del DB
        builder.Services.AddSingleton<DatabaseService>();

        // Transient: nuova istanza ad ogni navigazione
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<CalendarPageV2>();
        builder.Services.AddTransient<App>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
