using System.Diagnostics;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using R3;

namespace Kava;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        WpfProviderInitializer.SetDefaultObservableSystem(ex =>
            Trace.WriteLine($"R3 UnhandledException:{ex}")
        );
    }

    private static void Main2()
    {
        var builder = new HostApplicationBuilder([]);

        builder.Services.Scan(s =>
            s.FromAssemblyOf<App>()
                .AddClasses(c => c.AssignableTo<FrameworkElement>())
                .AsSelfWithInterfaces(type =>
                {
                    if (type == typeof(IHostedService))
                    {
                        return true;
                    }

                    return false;
                })
        );
    }
}
