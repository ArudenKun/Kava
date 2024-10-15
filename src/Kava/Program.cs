using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Avalonia;
using CommunityToolkit.Mvvm.Messaging;
using Humanizer;
using JetBrains.Annotations;
using Kava.AppSettingsGen;
using Kava.Data;
using Kava.Data.Compiled;
using Kava.Services.Abstractions;
using Kava.Services.Hosting;
using Kava.Utilities.Helpers;
using Kava.ViewModels.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceScan.SourceGenerator;
using Utf8StringInterpolation;
using ZLogger;

namespace Kava;

internal static partial class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    [RequiresUnreferencedCode(
        "Calls Microsoft.Extensions.DependencyInjection.EntityFrameworkServiceCollectionExtensions.AddDbContextFactory<TContext>(Action<DbContextOptionsBuilder>, ServiceLifetime)"
    )]
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder();

        builder.Environment.EnvironmentName = IsDebug
            ? Environments.Development
            : Environments.Production;

        builder
            .Configuration.AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables();

        builder
            .Logging.ClearProviders()
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
                    ),
                (int)1.Gigabytes().Kilobytes
            );

        builder
            .Services.AddSingleton<IAppSettingsBinder, AppSettingsBinder>()
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        builder
            .Services.AddGeneratedServices()
            .AddDbContextFactory<AppDbContext>(
                (sp, optionsBuilder) =>
                    optionsBuilder
                        .AddInterceptors(sp.GetServices<IInterceptor>())
                        .UseSqlite(
                            $"Data Source={EnvironmentHelper.AppDataDirectory.JoinPath("data.db")}"
                        )
                        .UseModel(AppDbContextModel.Instance)
            )
            .AddAvaloniauiDesktopApplication<App>(appBuilder =>
                appBuilder.UsePlatformDetect().LogToTrace()
            );

        using var host = builder.Build();

        host.RunAvaloniaApplicationAsync(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    [UsedImplicitly]
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure(() => new Application()).UsePlatformDetect().LogToTrace();

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IViewModel),
        Lifetime = ServiceLifetime.Transient,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(IInterceptor),
        Lifetime = ServiceLifetime.Singleton,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ISingletonService),
        Lifetime = ServiceLifetime.Singleton,
        AsSelf = true,
        AsImplementedInterfaces = true
    )]
    private static partial IServiceCollection AddGeneratedServices(
        this IServiceCollection serviceProvider
    );

    private static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif
}
