using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Kava.Controls.WebView;
using Kava.Data;
using Kava.Utilities.Helpers;
using Kava.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Kava;

[RequiresUnreferencedCode("App requires unreferenced code")]
[RequiresDynamicCode("App requires dynamic code")]
public class App : Application
{
    private readonly ILogger<App> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public App(
        ILogger<App> logger,
        IServiceProvider serviceProvider,
        IDbContextFactory<AppDbContext> dbContextFactory
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _dbContextFactory = dbContextFactory;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        NativeWebView.SetFactory(() => new WebView2Adapter(EnvironmentHelper.AppDataDirectory));

        // using var db = _dbContextFactory.CreateDbContext();
        // db.Database.Migrate();
        // _logger.ZLogInformation($"Database Migration Complete");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow =
                DataTemplates[0].Build(_serviceProvider.GetRequiredService<MainWindowViewModel>())
                as Window;
            _logger.ZLogInformation($"MainWindow Initialized");
        }

        base.OnFrameworkInitializationCompleted();
    }
}
