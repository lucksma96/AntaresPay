using AntaresPay.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace AntaresPay
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainPage>();

            builder.Services.AddTransient<OperationPage>();
            builder.Services.AddTransient<OperationViewModel>();

            builder.Services.AddTransient<TransactionPage>();
            builder.Services.AddTransient<TransactionViewModel>();

            builder.Services.AddTransient<BalancePage>();
            builder.Services.AddTransient<BalanceViewModel>();

            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<RegisterViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
