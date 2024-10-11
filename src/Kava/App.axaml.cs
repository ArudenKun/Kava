using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Kava.Data;
using Kava.Helpers;
using Kava.ViewModels;
using Kava.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kava;

public sealed class App : Application, IDisposable, IAsyncDisposable
{
    private readonly ServiceProvider _services;
    private readonly ILogger<App> _logger;

    [RequiresUnreferencedCode("AddDbContext requires UnreferencedCode")]
    public App()
    {
        SQLitePCL.Batteries_V2.Init();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(builder =>
            builder.UseSqlite(
                $"Data Source={EnvironmentHelper.AppDataDirectory.JoinPath("data.db")}"
            )
        );

        _services = services.BuildServiceProvider(true);

        _logger = _services.GetRequiredService<ILogger<App>>();
    }

    [RequiresDynamicCode("DatabaseFacade Migrate Method")]
#pragma warning disable IL3051
    public override void Initialize()
#pragma warning restore IL3051
    {
        AvaloniaXamlLoader.Load(this);

        using var scope = _services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }

    [RequiresUnreferencedCode("Calls BindingPlugins.DataValidators.RemoveAt")]
#pragma warning disable IL2046
    public override void OnFrameworkInitializationCompleted()
#pragma warning restore IL2046
    {
        BindingPlugins.DataValidators.RemoveAt(0);
        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                // Line below is needed to remove Avalonia data validation.
                // Without this line you will get duplicate validations from both Avalonia and CT
                desktop.MainWindow = new MainWindow { DataContext = new MainViewModel() };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView { DataContext = new MainViewModel() };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void Dispose()
    {
        _services.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _services.DisposeAsync();
    }
}
