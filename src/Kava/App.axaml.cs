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

public sealed class App : Application
{
    private readonly ILogger<App> _logger;
    private readonly ViewLocator _viewLocator;
    private readonly IServiceProvider _serviceProvider;

    public App(ILogger<App> logger, ViewLocator viewLocator, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _viewLocator = viewLocator;
        _serviceProvider = serviceProvider;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        _logger.LogInformation("App initialized");
        DataTemplates.Add(_viewLocator);
    }

    [RequiresUnreferencedCode("Calls BindingPlugins.DataValidators.RemoveAt")]
#pragma warning disable IL2046
    public override void OnFrameworkInitializationCompleted()
#pragma warning restore IL2046
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT
        BindingPlugins.DataValidators.RemoveAt(0);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow =
                DataTemplates[0].Build(_serviceProvider.GetRequiredService<MainWindowViewModel>())
                as Window;

            _logger.LogInformation("MainWindow Initialized");
        }

        base.OnFrameworkInitializationCompleted();
    }
}
