using Lab2_oop.Analyzers;
using Lab2_oop.ViewModels;
using Microsoft.Extensions.Logging;

namespace Lab2_oop
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
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // Реєстрація стратегій
            builder.Services.AddSingleton<IXmlAnalyzer, LinqAnalyzer>();
            builder.Services.AddSingleton<IXmlAnalyzer, DomAnalyzer>();
            builder.Services.AddSingleton<IXmlAnalyzer, SaxAnalyzer>();

            // Реєстрація ViewModel та Page
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}