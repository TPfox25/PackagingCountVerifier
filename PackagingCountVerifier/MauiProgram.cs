using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using PackagingCountVerifier.Data;
using Microsoft.Maui.Storage;
using System.IO;
using System.Diagnostics;  

namespace PackagingCountVerifier
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

            // ✅ DATABASE PATH
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "packing.db");

            // ✅ PRINT DATABASE LOCATION TO DEBUG OUTPUT
            Debug.WriteLine("📦 DATABASE FILE LOCATION:");
            Debug.WriteLine(dbPath);

            // ✅ REGISTER DATABASE SERVICE
            builder.Services.AddSingleton<AppDatabase>(
                new AppDatabase(dbPath));

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
