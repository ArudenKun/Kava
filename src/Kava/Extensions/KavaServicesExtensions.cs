using System.Text.Json.Serialization.Metadata;
using CommunityToolkit.Mvvm.Messaging;
using Kava.Core;
using Kava.Core.Helpers;
using Kava.Services;
using Kava.ViewModels.Abstractions;
using Kava.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using ServiceScan.SourceGenerator;

namespace Kava.Extensions;

public static partial class KavaServicesExtensions
{
    public static void AddKavaServices(this IServiceCollection services)
    {
        services.AddSingleton(AppJsonContext.Default.Options);
        services.AddSingleton<IJsonTypeInfoResolver>(AppJsonContext.Default);
        services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        services.AddSingleton<FileAccessService>();
        services.AddSingleton<ConfigService>();
        services.AddSingleton<ViewLocator>();

        const string template =
            "[{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} {Level:u3}] {Message:lj}{NewLine}{Exception}";
        var loggingLevelSwitch = new LoggingLevelSwitch(
            EnvironmentHelper.IsDebug ? LogEventLevel.Debug : LogEventLevel.Information
        );

        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Version", AppInfo.AppVersion)
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            .WriteTo.Console(outputTemplate: template)
            .WriteTo.Async(lc =>
                lc.PersistentFile(
                    AppInfo.LogsDir.Path.JoinPath("logs.txt"),
                    outputTemplate: template,
                    persistentFileRollingInterval: PersistentFileRollingInterval.Day,
                    rollOnFileSizeLimit: true,
                    retainedFileCountLimit: 61
                )
            )
            .CreateLogger();

        services.AddSingleton(loggingLevelSwitch);
        services.AddLogging(builder => builder.AddSerilog(dispose: true));
    }

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ISingletonView),
        Lifetime = ServiceLifetime.Singleton,
        AsSelf = true,
        TypeNameFilter = "*Window,*View,*Page"
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ITransientView),
        Lifetime = ServiceLifetime.Transient,
        AsSelf = true,
        TypeNameFilter = "*Window,*View,*Page"
    )]
    public static partial void AddKavaViews(this IServiceCollection services);

    [GenerateServiceRegistrations(
        AssignableTo = typeof(ISingletonViewModel),
        Lifetime = ServiceLifetime.Singleton,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ITransientViewModel),
        Lifetime = ServiceLifetime.Transient,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    public static partial void AddKavaViewModels(this IServiceCollection services);
}
