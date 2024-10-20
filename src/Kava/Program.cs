using System;
using System.Runtime.Versioning;
using Avalonia;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using JetBrains.Annotations;
using Kava.Data;
using Kava.Services.Abstractions;
using Kava.Services.Hosting;
using Kava.Utilities.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ServiceScan.SourceGenerator;

namespace Kava;

internal static partial class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("macos")]
    [SupportedOSPlatform("linux")]
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Environment.EnvironmentName = IsDebug
            ? Environments.Development
            : Environments.Production;

        builder.Services.Configure<HostOptions>(options =>
            options.ServicesStartConcurrently = true
        );

        builder.Services.AddGeneratedServices();
        builder.Services.AddAvaloniaDesktopApplication<App>(appBuilder =>
            appBuilder.UsePlatformDetect().LogToTrace()
        );
        builder.Services.AddJsonDataStorage(EnvironmentHelper.AppDataDirectory.JoinPath("data"));

        builder.Services.AddSingleton(AppJsonContext.Default);
        builder.Services.AddSingleton(AppJsonContext.Default.Options);
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton(
            new LoggingLevelSwitch(
                builder.Environment.IsDevelopment()
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information
            )
        );

        builder.Services.AddSerilog(
            (sp, configuration) =>
            {
                const string template =
                    "[{Timestamp:HH:mm:ss} {Level:u3} {SourceContext}] {Message:lj}{NewLine}{Exception}";
                var logPath = EnvironmentHelper.AppDataDirectory.JoinPath("logs", "logs.log");
                configuration
                    .MinimumLevel.ControlledBy(sp.GetRequiredService<LoggingLevelSwitch>())
                    .Enrich.FromLogContext()
                    .WriteTo.Console(outputTemplate: template)
                    .WriteTo.Async(x =>
                        x.PersistentFile(
                            logPath,
                            outputTemplate: template,
                            rollOnFileSizeLimit: true,
                            persistentFileRollingInterval: PersistentFileRollingInterval.Day,
                            preserveLogFilename: true,
                            fileSizeLimitBytes: (long?)1.Gigabytes().Bytes
                        )
                    );
            }
        );

        using var host = builder.Build();

        try
        {
            host.RunAvaloniaApplicationAsync(args);
        }
        catch (Exception e)
        {
            host.Services.GetRequiredService<ILogger<App>>().LogCritical(e, "An error occured");

            if (OperatingSystem.IsWindows())
            {
                _ = OSNativeHelper.Windows.ErrorMessageBox("Kava Fatal Error", e.ToString());
            }

            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    [UsedImplicitly]
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure(() => new Application()).UsePlatformDetect().LogToTrace();

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ISingleton),
        Lifetime = ServiceLifetime.Singleton,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IScoped),
        Lifetime = ServiceLifetime.Scoped,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ITransient),
        Lifetime = ServiceLifetime.Transient,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    private static partial void AddGeneratedServices(this IServiceCollection serviceProvider);

    private static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif
}
