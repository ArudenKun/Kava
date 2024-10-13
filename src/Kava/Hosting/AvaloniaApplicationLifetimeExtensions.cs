using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kava.Hosting;

public static class AvaloniaApplicationLifetimeExtensions
{
    /// <summary>
    /// Configures the host to use avaloniaui application lifetime.
    /// Also configures the <typeparamref name="TApplication"/> as a singleton.
    /// </summary>
    /// <typeparam name="TApplication">The type of avaloniaui application <see cref="Application"/> to manage.</typeparam>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="appBuilderConfiguration"><see cref="AppBuilder.Configure{TApplication}()"/></param>
    public static IServiceCollection AddAvaloniauiDesktopApplication<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TApplication
    >(this IServiceCollection services, Func<AppBuilder, AppBuilder> appBuilderConfiguration)
        where TApplication : Application =>
        services
            .AddSingleton<TApplication>()
            .AddSingleton(provider =>
            {
                var appBuilder = AppBuilder.Configure(provider.GetRequiredService<TApplication>);
                appBuilder = appBuilderConfiguration(appBuilder);
                return appBuilder;
            })
            .AddSingleton<IHostLifetime, AvaloniaApplicationLifetime<TApplication>>();

    /// <summary>
    /// Add MainWindow&MainWindowViewModel to ServiceCollection.Note:Support native AOT with rd.xml
    /// </summary>
    /// <typeparam name="TMainWindow"><see cref="Window"/></typeparam>
    /// <typeparam name="TViewModel">MainWindowViewModel</typeparam>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <returns></returns>
    [SuppressMessage("ReSharper", "InvalidXmlDocComment")]
    public static IServiceCollection AddMainWindow<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMainWindow,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TViewModel
    >(this IServiceCollection services)
        where TMainWindow : Window
        where TViewModel : class, INotifyPropertyChanged =>
        services
            .AddSingleton<TViewModel>()
            .AddSingleton(sp =>
            {
                var viewmodel = sp.GetRequiredService(typeof(TViewModel));
                var window = ActivatorUtilities.CreateInstance<TMainWindow>(sp);
                window.DataContext = viewmodel;
                return window;
            });

    /// <summary>
    /// Runs the avaloniaui application along with the .NET generic host.
    /// Note:
    /// 1.Host will set the ShutdownMode with ShutdownMode.OnMainWindowClose
    /// 2.Support native AOT with rd.xml
    /// </summary>
    /// <typeparam name="TMainWindow">The type of the window <see cref="Window"/> to run.</typeparam>
    /// <param name="host"><see cref="IHost"/></param>
    /// <param name="commandArgs">commandLine args</param>
    /// <param name="shutdownMode"><see cref="ShutdownMode"/></param>
    /// <param name="cancellationToken">cancellationToken</param>
    public static Task RunAvaloniaApplicationAsync<TMainWindow>(
        this IHost host,
        string[] commandArgs,
        ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose,
        CancellationToken cancellationToken = default
    )
        where TMainWindow : Window =>
        RunAvaloniauiApplicationCore(
            host,
            commandArgs,
            shutdownMode,
            host.Services.GetRequiredService<TMainWindow>,
            cancellationToken
        );

    /// <summary>
    ///  Runs the avaloniaui application along with the .NET generic host.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="commandArgs"></param>
    /// <param name="shutdownMode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAvaloniaApplicationAsync(
        this IHost host,
        string[] commandArgs,
        ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose,
        CancellationToken cancellationToken = default
    ) => RunAvaloniauiApplicationCore(host, commandArgs, shutdownMode, null, cancellationToken);

    private static Task RunAvaloniauiApplicationCore(
        IHost host,
        string[] commandArgs,
        ShutdownMode shutdownMode = ShutdownMode.OnLastWindowClose,
        Func<Window>? mainWindowFactory = null,
        CancellationToken cancellationToken = default
    )
    {
        _ = host ?? throw new ArgumentNullException(nameof(host));
        var builder = host.Services.GetRequiredService<AppBuilder>();
        builder = builder.SetupWithLifetime(
            new ClassicDesktopStyleApplicationLifetime
            {
                Args = commandArgs,
                ShutdownMode = shutdownMode,
            }
        );

        if (builder.Instance is null)
        {
            throw new InvalidOperationException("AppBuilder has not been initialized yet!");
        }

        var hostTask = host.RunAsync(token: cancellationToken);

        if (
            builder.Instance.ApplicationLifetime
            is ClassicDesktopStyleApplicationLifetime classicDesktop
        )
        {
            if (mainWindowFactory != null)
            {
                classicDesktop.MainWindow =
                    mainWindowFactory()
                    ?? throw new InvalidOperationException(
                        "The MainWindow must been registered in Services before running"
                    );
            }

            // ReSharper disable once InvalidXmlDocComment
            ///https://github.com/AvaloniaUI/Avalonia/pull/16167
            Environment.ExitCode = classicDesktop.Start(classicDesktop.Args ?? []);
#if DEBUG
            Console.WriteLine($"Process has been exited:{Environment.ExitCode}");
#endif
        }
        else
        {
            throw new InvalidOperationException("Generic host support classic desktop only!");
        }

        return hostTask;
    }
}
