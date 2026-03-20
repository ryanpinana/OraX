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
                    fonts.AddFont("SFNSDisplay-Medium.otf", "font");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            Routing.RegisterRoute("CalendarPageV2", typeof(CalendarPageV2));
            return builder.Build();
        }
    }
}
