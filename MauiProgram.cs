using CommunityToolkit.Maui;
using CoranWarshSynchroniser.ViewModels;
using CoranWarshSynchroniser.Views;
using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using Syncfusion.Maui.Core.Hosting;

namespace CoranWarshSynchroniser;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
            fonts.AddFont("UthmanicWarshV21.ttf", "UthmanicWarsh"); 
        })
        .UseMauiCommunityToolkit()
        .UseMauiCommunityToolkitMediaElement();

        builder.ConfigureSyncfusionCore();

        builder.Services.AddSingleton(AudioManager.Current);

        // ✅ Pages de navigation Shell → obligatoirement Transient
        builder.Services.AddTransient<QuranReaderPage>();
        builder.Services.AddTransient<QuranViewPage>();

        // ✅ ViewModels des pages Transient → aussi Transient
        builder.Services.AddTransient<QuranReaderViewModel>();

        // ✅ Pages/ViewModels statiques → Singleton OK
        builder.Services.AddSingleton<QuranViewModel>();
        builder.Services.AddSingleton<QuranRichTextView>();
        builder.Services.AddSingleton<MyHomePage>();
        builder.Services.AddSingleton<QuranPage>();
        builder.Services.AddSingleton<SouratesViewModel>();
        builder.Services.AddSingleton<SouratesPage>();
        builder.Services.AddSingleton<ReaderSourat>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}