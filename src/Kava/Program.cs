using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Avalonia;
using Humanizer;
using JetBrains.Annotations;
using Kava.Core.Helpers;
using Kava.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Utf8StringInterpolation;
using ZLogger;

namespace Kava;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [RequiresDynamicCode("Calls Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder()")]
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();

        // config IConfiguration
        builder
            .Logging.ClearProviders()
            .SetMinimumLevel(LogLevel.Debug)
            .AddZLoggerConsole(options =>
            {
                options.OutputEncodingToUtf8 = false;
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter(
                        $"[{0} {1} {2}] ",
                        (in MessageTemplate template, in LogInfo info) =>
                            template.Format(info.Timestamp, info.LogLevel, info.Category)
                    );
                    formatter.SetExceptionFormatter(
                        (writer, ex) => Utf8String.Format(writer, $"{ex.Message}")
                    );
                });
            })
            .AddZLoggerFile(EnvironmentHelper.AppDataDirectory.JoinPath("logs", "logs.log"))
            .AddZLoggerRollingFile(
                (dt, index) =>
                    EnvironmentHelper.AppDataDirectory.JoinPath(
                        "logs",
                        $"logs-{dt.ToLocalTime():dd-MM-yyyy}-{index}.log"
                    // $"logs-{dt:dd-MM-yyyy}-{index}.log"
                    ),
                (int)1.Gigabytes().Kilobytes
            );

        builder.Services.AddAvaloniauiDesktopApplication<App>(appBuilder =>
            appBuilder.UsePlatformDetect().UseR3().WithInterFont().LogToTrace()
        );

        using var host = builder.Build();
        host.RunAvaloniaApplication(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    [UsedImplicitly]
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder
            .Configure(() => new Application())
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
