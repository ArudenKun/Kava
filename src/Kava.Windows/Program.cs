using System;
using Avalonia;
using JetBrains.Annotations;
using Kava.Controls.WebView;
using Kava.Core;
using Kava.Hosting;
using Kava.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Velopack;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.UI.WindowsAndMessaging;

namespace Kava.Windows;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // var host = AvaloniaHost
        //     .Create(args)
        //     .ConfigureAvalonia<App, MainWindow>(builder => builder.UsePlatformDetect().LogToTrace())
        //     .Build();

        var builder = Host.CreateApplicationBuilder(args);

        builder.ConfigureAvaloniaDesktopHosting<App, MainWindow>(appBuilder =>
            appBuilder.UsePlatformDetect().LogToTrace()
        );

        var host = builder.Build();

        try
        {
            VelopackApp.Build().Run(host.Services.GetRequiredService<ILogger<VelopackApp>>());
            NativeWebView.SetWebViewAdapterFactory(
                () => new WebView2Adapter(AppInfo.CachesDir.Path)
            );
            host.RunAvaloniaHosting();
        }
        catch (Exception ex)
        {
            _ = PInvoke.MessageBox(
                (HWND)0,
                ex.ToString(),
                "Kava Unhandled Exception",
                MESSAGEBOX_STYLE.MB_ICONSTOP
            );
            throw;
        }
        finally
        {
            host.Dispose();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    [UsedImplicitly]
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<Application>().UsePlatformDetect().LogToTrace();
}
