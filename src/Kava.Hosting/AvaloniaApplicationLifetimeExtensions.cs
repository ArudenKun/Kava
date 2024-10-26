using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IApplicationLifetime = Avalonia.Controls.ApplicationLifetimes.IApplicationLifetime;

namespace Kava.Hosting;

public static class AvaloniaApplicationLifetimeExtensions
{
    /// <summary>
    /// Configures the host to use avaloniaui application lifetime.
    /// Also configures the <typeparamref name="TApplication"/> as a singleton.
    /// Also configures the <typeparamref name="TWindow"/> as a singleton.
    /// </summary>
    /// <typeparam name="TApplication">The type of avaloniaui application <see cref="Application"/> to manage.</typeparam>
    /// <typeparam name="TWindow">The  window to use</typeparam>
    /// <param name="services"><see cref="IServiceCollection"/></param>
    /// <param name="appBuilderConfiguration"><see cref="AppBuilder.Configure{TApplication}()"/></param>
    /// <param name="optionsConfiguration">Options to configure the lifetime</param>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static IServiceCollection AddAvaloniaDesktopApplication<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TApplication,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TWindow
    >(
        this IServiceCollection services,
        Func<IServiceProvider, AppBuilder, AppBuilder> appBuilderConfiguration,
        Action<AvaloniaApplicationLifetimeOptions> optionsConfiguration
    )
        where TApplication : Application
        where TWindow : Window, IShellView =>
        services
            .AddSingleton(_ =>
            {
                var options = new AvaloniaApplicationLifetimeOptions();
                optionsConfiguration(options);
                return options;
            })
            .AddSingleton<TApplication>()
            .AddSingleton(sp =>
            {
                var appBuilder = AppBuilder.Configure(sp.GetRequiredService<TApplication>);
                appBuilder = appBuilderConfiguration(sp, appBuilder);
                appBuilder.SetupWithLifetime(
                    new ClassicDesktopStyleApplicationLifetime
                    {
                        Args = sp.GetRequiredService<AvaloniaApplicationLifetimeOptions>().Args,
                        ShutdownMode =
                            sp.GetRequiredService<AvaloniaApplicationLifetimeOptions>().ShutdownMode,
                    }
                );
                return appBuilder;
            })
            .AddSingleton<IApplicationLifetime>(sp =>
                sp.GetRequiredService<AppBuilder>().Instance?.ApplicationLifetime
                ?? throw new InvalidOperationException("No application lifetime is set")
            )
            .AddSingleton<IDispatcher>(_ => Dispatcher.UIThread)
            .AddSingleton<TWindow>()
            .AddSingleton<IShellView>(sp => sp.GetRequiredService<TWindow>())
            .AddSingleton<TopLevel>(sp => sp.GetRequiredService<TWindow>())
            .AddSingleton(sp => sp.GetRequiredService<TopLevel>().StorageProvider)
            .AddSingleton(sp => sp.GetRequiredService<TopLevel>().Launcher)
            .AddSingleton(sp => sp.GetRequiredService<TopLevel>().Clipboard!)
            .AddSingleton<IHostLifetime, AvaloniaApplicationLifetime<TApplication>>();

    /// <summary>
    ///  Runs the avalonia application along with the .NET generic host.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [SupportedOSPlatform("windows")]
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macos")]
    public static Task RunAvaloniaApplicationAsync(
        this IHost host,
        CancellationToken cancellationToken = default
    ) => RunAvaloniauiApplicationCore(host, cancellationToken);

    private static Task RunAvaloniauiApplicationCore(
        IHost host,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(host);
        var builder = host.Services.GetRequiredService<AppBuilder>();

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
            // ReSharper disable once InvalidXmlDocComment
            ///https://github.com/AvaloniaUI/Avalonia/pull/16167
            Environment.ExitCode = classicDesktop.Start(classicDesktop.Args ?? []);
        }
        else
        {
            throw new InvalidOperationException("Generic host support classic desktop only!");
        }

        return hostTask;
    }
}
