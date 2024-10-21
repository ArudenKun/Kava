using System;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Kava;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

#if DEBUG
        builder.Environment.EnvironmentName = Environments.Development;
#endif

        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton(
            new LoggingLevelSwitch(
                builder.Environment.IsDevelopment()
                    ? LogEventLevel.Debug
                    : LogEventLevel.Information
            )
        );

        builder.Services.AddWpfApplication<App>();
        builder.Services.AddSerilog(
            (sp, loggingBuilder) =>
            {
                const string template =
                    "[{Timestamp:yyyy-MM-dd HH:mm:ss} {SourceContext} {Level:u3}] {Message:lj}{NewLine}{Exception}";

                loggingBuilder.MinimumLevel.ControlledBy(
                    sp.GetRequiredService<LoggingLevelSwitch>()
                );
                loggingBuilder.WriteTo.Console(outputTemplate: template);
                loggingBuilder.WriteTo.Async(x => x.PersistentFile("", outputTemplate: template));
                loggingBuilder.Enrich.FromLogContext();
            }
        );

        using var host = builder.Build();
        host.RunWpfApplication<App>();
    }
}
