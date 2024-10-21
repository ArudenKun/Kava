using JetBrains.Annotations;
using Kava.Hosting.Services.Abstractions;
using Kava.Hosting.Services.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kava.Hosting.Services;

[PublicAPI]
public static class ServiceCollectionExtensions
{
    private const string MutexBuilderKey = "MutexBuilder";

    /// <summary>
    /// Helper method to retrieve the mutex builder
    /// </summary>
    /// <param name="properties">IDictionary</param>
    /// <param name="mutexBuilder">IMutexBuilder out value</param>
    /// <returns>bool if there was a matcher</returns>
    private static bool TryRetrieveMutexBuilder(
        this IDictionary<object, object> properties,
        out IMutexBuilder mutexBuilder
    )
    {
        if (properties.TryGetValue(MutexBuilderKey, out var mutexBuilderObject))
        {
            mutexBuilder = (IMutexBuilder)mutexBuilderObject;
            return true;
        }

        mutexBuilder = new MutexBuilder();
        properties[MutexBuilderKey] = mutexBuilder;
        return false;
    }

    /// <summary>
    /// Prevent that an application runs multiple times
    /// </summary>
    /// <param name="hostBuilder">IHostApplicationBuilder</param>
    /// <param name="configureAction">Action to configure IMutexBuilder</param>
    /// <returns>IHostApplicationBuilder for fluently calling</returns>
    public static IHostApplicationBuilder ConfigureSingleInstance(
        this IHostApplicationBuilder hostBuilder,
        Action<IMutexBuilder>? configureAction
    )
    {
        if (!TryRetrieveMutexBuilder(hostBuilder.Properties, out var mutexBuilder))
        {
            hostBuilder
                .Services.AddSingleton(mutexBuilder)
                .AddHostedService<MutexLifetimeService>();
        }

        configureAction?.Invoke(mutexBuilder);
        return hostBuilder;
    }

    /// <summary>
    /// Prevent that an application runs multiple times
    /// </summary>
    /// <param name="hostBuilder">IHostApplicationBuilder</param>
    /// <param name="mutexId">string</param>
    /// <returns>IHostApplicationBuilder for fluently calling</returns>
    public static IHostApplicationBuilder ConfigureSingleInstance(
        this IHostApplicationBuilder hostBuilder,
        string mutexId
    ) => hostBuilder.ConfigureSingleInstance(builder => builder.MutexId = mutexId);
}
