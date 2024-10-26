using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using Avalonia;
using CommunityToolkit.Mvvm.Messaging;
using FreeSql;
using JetBrains.Annotations;
using Kava.Hosting;
using Kava.Services;
using Kava.Utilities.Helpers;
using Kava.ViewModels.Abstractions;
using Kava.Views;
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
    [RequiresUnreferencedCode(
        "Calls Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory<TContext>(Action<DbContextOptionsBuilder>, ServiceLifetime)"
    )]
    public static void Main(string[] args)
    {
        var builder = new HostApplicationBuilder(args);

#if DEBUG
        builder.Environment.EnvironmentName = Environments.Development;
#endif

        builder.Services.AddAvaloniaDesktopApplication<App, MainWindow>(
            (sp, appBuilder) =>
                appBuilder
                    .UsePlatformDetect()
                    .UseR3(ex =>
                        sp.GetRequiredService<ILogger<App>>().LogError(ex, "R3 Unhandled Exception")
                    )
                    .LogToTrace(),
            options =>
            {
                options.Args = args;
            }
        );
        builder.Services.AddSerilog(
            (sp, loggingConfiguration) =>
            {
                const string template =
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} {Level:u3}] {Message:lj}{NewLine}{Exception}";

                loggingConfiguration.MinimumLevel.ControlledBy(
                    sp.GetRequiredService<LoggingLevelSwitch>()
                );
                loggingConfiguration.WriteTo.Console(outputTemplate: template);
                loggingConfiguration.WriteTo.Async(x =>
                    x.PersistentFile(
                        EnvironmentHelper.AppDataDirectory.JoinPath("logs", "logs.log"),
                        outputTemplate: template,
                        persistentFileRollingInterval: PersistentFileRollingInterval.Day,
                        rollOnFileSizeLimit: true,
                        retainedFileCountLimit: 61
                    )
                );
                loggingConfiguration.Enrich.FromLogContext();
            }
        );
        builder.Services.AddScannedViewModels();

        builder.Services.AddHostedService<StartupService>();

        builder.Services.AddSingleton(sp =>
            new FreeSqlBuilder()
                .UseConnectionString(
                    DataType.Sqlite,
                    $"Data Source={EnvironmentHelper.AppDataDirectory.JoinPath("data.db")}"
                )
                .UseAutoSyncStructure(true)
                .UseMonitorCommand(command =>
                    sp.GetRequiredService<ILogger<IFreeSql>>()
                        .LogDebug("Sql: {Sql}", command.CommandText)
                )
                .Build()
        );
        builder.Services.AddSingleton(
            new LoggingLevelSwitch(
                builder.Environment.IsDevelopment()
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information
            )
        );
        builder.Services.AddSingleton(KavaJsonContext.Default.Options);
        builder.Services.AddSingleton<IJsonTypeInfoResolver>(KavaJsonContext.Default);
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<FileAccessService>();
        builder.Services.AddSingleton<SettingsService>();
        builder.Services.AddSingleton<ViewLocator>();

        var host = builder.Build();

        try
        {
            host.RunAvaloniaApplicationAsync();
        }
        catch (Exception ex)
        {
            host.Services.GetRequiredService<ILogger<App>>().LogError(ex, "Unhandled Exception");

            if (OperatingSystem.IsWindows())
            {
                _ = OSNativeHelper.Windows.ErrorMessageBox("Kava Fatal Error", ex.ToString());
            }

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

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IViewModel),
        Lifetime = ServiceLifetime.Transient,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    private static partial void AddScannedViewModels(this IServiceCollection services);
}
