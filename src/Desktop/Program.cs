using System;
using Avalonia;
using Avalonia.Logging;
using Avalonia.ReactiveUI;

namespace Desktop;

public static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        var builder = BuildAvaloniaApp();

        try
        {
            return builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Logger.Sink?.Log(LogEventLevel.Error, "Init", null, "Int Unhandled Exception {0}", ex);
            throw;
        }
        finally
        {
            if (builder.Instance is IDisposable disposableApp)
            {
                disposableApp.Dispose();
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>().UsePlatformDetect().UseReactiveUI().LogToTrace();
}
