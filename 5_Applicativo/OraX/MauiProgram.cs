using Microsoft.Extensions.Logging;

namespace OraX
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    //fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    //fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("SFNSDisplay-Medium.otf", "font");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            Routing.RegisterRoute("Statistiche", typeof(Statistiche));
            Routing.RegisterRoute("Modifiche", typeof(Modifiche));
            return builder.Build();
        }
    }
}
