using System.Diagnostics.CodeAnalysis;
using AsyncImageLoader;
using AsyncImageLoader.Loaders;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using JetBrains.Annotations;
using Kava.Core;
using Kava.Extensions;
using Kava.Hosting.Abstractions;
using Kava.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Kava;

[UsedImplicitly]
public sealed class App : AvaloniaHostApplication
{
    // private readonly IServiceProvider _serviceProvider;
    // private readonly ILogger<App> _logger;
    //
    // public App(IServiceProvider serviceProvider, ILogger<App> logger)
    // {
    //     _serviceProvider = serviceProvider;
    //     _logger = logger;
    // }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        var diskCachedWebImageLoader = new DiskCachedWebImageLoader(AppInfo.CachesDir.Path);
        ImageLoader.AsyncImageLoader = diskCachedWebImageLoader;
        ImageBrushLoader.AsyncImageLoader = diskCachedWebImageLoader;
        DataTemplates.Add(Services.GetRequiredService<ViewLocator>());

        // Logger.LogInformation("Kava Starting");
    }

    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddKavaServices();
        services.AddKavaViews();
        services.AddKavaViewModels();

        // Logger.LogInformation("Kava Services Configured");
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
                DataTemplates[0].Build(Services.GetRequiredService<MainWindowViewModel>())
                as Window;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
