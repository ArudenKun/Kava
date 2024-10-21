using System.Windows;
using Microsoft.Extensions.Logging;
using R3;

namespace Kava;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private readonly ILogger<App> _logger;

    public App(ILogger<App> logger)
    {
        _logger = logger;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        WpfProviderInitializer.SetDefaultObservableSystem(ex =>
            _logger.LogError(ex, "R3 UnhandledException")
        );
    }
}
