using Microsoft.Extensions.Logging;
using OraX.Services; // importiamo il servizio database

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
               
                fonts.AddFont("SFUIDisplay-Regular.ttf", "font");
            });

        
        builder.Services.AddSingleton<DatabaseService>();

       
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}