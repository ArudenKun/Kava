// using System;
// using System.Diagnostics.CodeAnalysis;
// using System.Runtime.Versioning;
// using System.Threading;
// using System.Threading.Tasks;
// using Avalonia;
// using Avalonia.Controls;
// using JetBrains.Annotations;
// using Kava.Hosting.Abstractions;
// using M31.FluentApi.Attributes;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Hosting;
//
// namespace Kava.Hosting;
//
// [PublicAPI]
// [FluentApi("{Name}Builder")]
// [SupportedOSPlatform("windows")]
// [SupportedOSPlatform("linux")]
// [SupportedOSPlatform("macos")]
// public class AvaloniaHost : IDisposable
// {
//     private static HostApplicationBuilder _builder = null!;
//     private IHost _host = null!;
//
//     internal AvaloniaHost() { }
//
//     public IServiceProvider Services => _host.Services;
//
//     public static AvaloniaHostBuilder.IAvaloniaHostBuilder Create(string[] args)
//     {
//         _builder = Host.CreateApplicationBuilder(args);
//         return AvaloniaHostBuilder.InitialStep();
//     }
//
//     [FluentMethod(0)]
//     internal void ConfigureAvalonia<
//         [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
//             TApplication,
//         [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TWindow
//     >(Action<AppBuilder> appBuilderConfiguration)
//         where TApplication : AvaloniaHostApplication
//         where TWindow : Window
//     {
//         var dummyApp = Activator.CreateInstance<TApplication>();
// #pragma warning disable IL2090
//         typeof(TApplication)
//             .GetMethod(nameof(AvaloniaHostApplication.ConfigureServices))
// #pragma warning restore IL2090
//             ?.Invoke(dummyApp, [_builder.Services]);
//
//         _builder.Services.AddSingleton<TApplication>();
//         _builder.Services.AddSingleton<TWindow>();
//         _builder.Services.AddSingleton(sp =>
//         {
//             var appBuilder = AppBuilder.Configure(sp.GetRequiredService<TApplication>);
//             appBuilderConfiguration(appBuilder);
//             return appBuilder;
//         });
//     }
//
//     [FluentMember(1, "Configure{Name}")]
//     [FluentContinueWith(1)]
//     [FluentSkippable]
//     public AvaloniaHostOptions Options { get; private set; } = null!;
//
//     [FluentMethod(1)]
//     [FluentContinueWith(1)]
//     [FluentSkippable]
//     internal void ConfigureServices(Action<IServiceCollection> configureServices) { }
//
//     [FluentMethod(1)]
//     internal void Build()
//     {
//         _host = _builder.Build();
//     }
//
//     public Task RunAsync(CancellationToken cancellationToken = default)
//     {
//         var hostTask = _host.RunAsync(token: cancellationToken);
//         return hostTask;
//     }
//
//     public void Dispose() => _host.Dispose();
// }
