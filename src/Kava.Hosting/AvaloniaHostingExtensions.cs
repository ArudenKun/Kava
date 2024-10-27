using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Kava.Hosting.Abstractions;
using Kava.Hosting.Internals;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IApplicationLifetime = Avalonia.Controls.ApplicationLifetimes.IApplicationLifetime;

namespace Kava.Hosting;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("linux")]
[SupportedOSPlatform("macos")]
public static class AvaloniaHostingExtensions
{
    /// <summary>
    /// Configures the host to use avalonia hosting application lifetime.
    /// Also configures the <typeparamref name="TApplication"/> as a singleton.
    /// </summary>
    /// <typeparam name="TApplication">The type of hosting application <see cref="Application"/> to manage.</typeparam>
    /// <typeparam name="TWindow">The type of window to manage</typeparam>
    /// <param name="builder"><see cref="IHostApplicationBuilder"/></param>
    /// <param name="appBuilderConfiguration"><see cref="AppBuilder.Configure{TApplication}()"/></param>
    /// <param name="avaloniaHostingOptionsConfiguration"><see cref="AvaloniaHostOptions"/></param>
    public static IHostApplicationBuilder ConfigureAvaloniaDesktopHosting<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TApplication,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TWindow
    >(
        this IHostApplicationBuilder builder,
        Action<AppBuilder> appBuilderConfiguration,
        Action<AvaloniaHostOptions>? avaloniaHostingOptionsConfiguration = null
    )
        where TApplication : AvaloniaHostApplication, new()
        where TWindow : Window
    {
        var options = new AvaloniaHostOptions();
        avaloniaHostingOptionsConfiguration?.Invoke(options);
        options.MainWindowType = typeof(TWindow);

        var appBuilder = AppBuilder.Configure<TApplication>();
        appBuilderConfiguration(appBuilder);
        appBuilder = appBuilder.SetupWithLifetime(
            new ClassicDesktopStyleApplicationLifetime
            {
                Args = options.Args,
                ShutdownMode = options.ShutdownMode,
            }
        );

        if (appBuilder.Instance is AvaloniaHostApplication instance)
        {
            instance.ConfigureServices(builder.Services);
        }

        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton<TApplication>();
        builder.Services.AddSingleton<TWindow>();
        builder.Services.AddSingleton(appBuilder);

        builder.Services.AddSingleton<IApplicationLifetime>(sp =>
            sp.GetRequiredService<AppBuilder>().Instance?.ApplicationLifetime
            ?? throw new InvalidOperationException("No application lifetime is set")
        );
        builder.Services.AddSingleton<IDispatcher>(_ => Dispatcher.UIThread);
        builder.Services.AddSingleton<TopLevel>(sp => sp.GetRequiredService<TWindow>());
        builder.Services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().StorageProvider);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().Launcher);
        builder.Services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().Clipboard!);
        builder.Services.AddSingleton<IHostLifetime, AvaloniaHostingLifetime<TApplication>>();

        return builder;
    }

    /// <summary>
    ///  Runs the avalonia application along with the .NET generic host.
    /// </summary>
    /// <param name="host"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static Task RunAvaloniaHosting(
        this IHost host,
        CancellationToken cancellationToken = default
    ) => RunAvaloniaHostingCore(host, cancellationToken);

    private static Task RunAvaloniaHostingCore(
        IHost host,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(host);
        var options = host.Services.GetRequiredService<AvaloniaHostOptions>();
        var builder = host.Services.GetRequiredService<AppBuilder>();

        if (builder.Instance is not AvaloniaHostApplication instance)
        {
            throw new InvalidOperationException("AppBuilder has not been initialized yet!");
        }

        instance.InternalServices = host.Services;

        var hostTask = host.RunAsync(token: cancellationToken);

        if (
            builder.Instance.ApplicationLifetime
            is ClassicDesktopStyleApplicationLifetime classicDesktop
        )
        {
            if (options.SetMainWindow)
            {
                classicDesktop.MainWindow =
                    host.Services.GetRequiredService(options.MainWindowType) as Window;
            }

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
