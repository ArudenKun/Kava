using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Kava.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kava;

public class App : Application
{
    private readonly ILogger<App> _logger;
    private readonly IServiceProvider _serviceProvider;

    public App(ILogger<App> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        _logger.LogInformation("App Initialized");
    }

    [RequiresUnreferencedCode("Calls BindingPlugins.DataValidators.RemoveAt")]
#pragma warning disable IL2046
    public override void OnFrameworkInitializationCompleted()
#pragma warning restore IL2046
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow =
                DataTemplates[0].Build(_serviceProvider.GetRequiredService<MainWindowViewModel>())
                as Window;
            _logger.LogInformation("MainWindow Initialized");
        }

        base.OnFrameworkInitializationCompleted();
    }
}
