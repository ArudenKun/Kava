using System;
using System.Net;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using AsyncImageLoader;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Core.Extensions;
using Core.Helpers;
using Desktop.Models;
using Desktop.Services;
using Desktop.Services.Abstractions;
using Desktop.Services.Caching;
using Desktop.ViewModels;
using Desktop.ViewModels.Abstractions;
using Humanizer;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ServiceScan.SourceGenerator;
using SukiUI;
using Utf8StringInterpolation;
using WebViewControl;
using ZiggyCreatures.Caching.Fusion;
using ZLogger;

namespace Desktop;

public sealed partial class App : Application, IDisposable
{
    private readonly ServiceProvider _services;
    private readonly SettingsService _settingsService;

    private readonly CompositeDisposable _subscriptions = new();

    private static SukiTheme SukiTheme => SukiTheme.GetInstance();

    public App()
    {
        var services = new ServiceCollection();

        AddServices(services);

        services.AddSingleton(
            new LiteDatabase(
                new ConnectionString
                {
                    Filename = EnvironmentHelper.AppDataDirectory.JoinPath("data.dat"),
                }
            )
        );
        services.AddSingleton<ILiteDatabase>(sp => sp.GetRequiredService<LiteDatabase>());
        services.AddLiteDbCache();
        services
            .AddFusionCache()
            .WithDefaultEntryOptions(options =>
                options
                    .SetDuration(TimeSpan.FromMinutes(5))
                    .SetFailSafe(true, TimeSpan.FromHours(1), TimeSpan.FromMinutes(1))
                    .SetFactoryTimeouts(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(30))
            )
            .TryWithAutoSetup();

        services.AddLogging(builder =>
            builder
                .ClearProviders()
                .SetMinimumLevel(IsDebug ? LogLevel.Debug : LogLevel.Information)
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
                            $"logs-{dt:dd-MM-yyyy}-{index}.log"
                        ),
                    (int)1.Gigabytes().Kilobytes
                )
        );

        _services = services.BuildServiceProvider(true);
        _settingsService = _services.GetRequiredService<SettingsService>();
    }

    public override void Initialize()
    {
        // Increase maximum concurrent connections
        ServicePointManager.DefaultConnectionLimit = 20;

        var liteDbCacheImageLoader = _services.GetRequiredService<LiteDbCacheImageLoader>();

        ImageLoader.AsyncImageLoader = liteDbCacheImageLoader;
        ImageBrushLoader.AsyncImageLoader = liteDbCacheImageLoader;

        var webViewSettings = WebView.Settings;
        webViewSettings.OsrEnabled = false;
        webViewSettings.LogFile = EnvironmentHelper.AppDataDirectory.JoinPath("logs", "cef.log");
        webViewSettings.CachePath = EnvironmentHelper.AppDataDirectory.JoinPath("cef");

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow =
                DataTemplates[0].Build(_services.GetRequiredService<MainWindowViewModel>())
                as Window;
        }

        Observable
            .FromEvent<ThemeVariant>(
                handler => SukiTheme.OnBaseThemeChanged += handler,
                handler => SukiTheme.OnBaseThemeChanged -= handler
            )
            .Subscribe(variant =>
            {
                if (variant == ThemeVariant.Dark)
                {
                    _settingsService.Theme = Theme.Dark;
                    return;
                }

                _settingsService.Theme = variant == ThemeVariant.Light ? Theme.Light : Theme.System;
            })
            .DisposeWith(_subscriptions);

        _settingsService.Load();

        SukiTheme.ChangeBaseTheme(_settingsService.ThemeVariant);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
        _services.Dispose();
    }

    private static bool IsDebug
#if DEBUG
        => true;
#else
        => false;
#endif

    [GenerateServiceRegistrations(
        AssignableTo = typeof(IViewModel),
        AsSelf = true,
        AsImplementedInterfaces = true,
        Lifetime = ServiceLifetime.Transient
    )]
    [GenerateServiceRegistrations(
        AssignableTo = typeof(ISingleton),
        AsSelf = true,
        AsImplementedInterfaces = true,
        Lifetime = ServiceLifetime.Singleton
    )]
    private static partial void AddServices(IServiceCollection services);
}
