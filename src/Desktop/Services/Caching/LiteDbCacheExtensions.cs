using System;
using System.Linq;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization;

namespace Desktop.Services.Caching;

public static class LiteDbCacheExtensions
{
    /// <summary>
    /// Registers all LiteDbCache services
    /// </summary>
    /// <param name="services">services</param>
    public static IServiceCollection AddLiteDbCache(this IServiceCollection services) =>
        services.AddLiteDbCache(_ => { });

    /// <summary>
    /// Registers all LiteDbCache services
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="setupAction">setup</param>
    public static IServiceCollection AddLiteDbCache(
        this IServiceCollection services,
        Action<LiteDbCacheOptions> setupAction
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(setupAction);

        services.AddOptions();
        services.TryConfigure(setupAction);

        services.TryAddSingleton<LiteDbCache>();
        services.TryAddSingleton<LiteDbCacheImageLoader>();
        services.TryAddSingleton<IDistributedCache>(s => s.GetRequiredService<LiteDbCache>());
        services.TryAddSingleton<ILiteDbCache>(s => s.GetRequiredService<LiteDbCache>());

        services.AddFusionCacheLiteDbCacheSerializer();

        return services;
    }

    /// <summary>
    /// Registers all LiteDbCache services
    /// </summary>
    /// <param name="services">services</param>
    /// <param name="collectionName">name of the collection to use</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Style",
        "IDE0046:Convert to conditional expression",
        Justification = "Too much nesing"
    )]
    public static IServiceCollection AddLiteDbCache(
        this IServiceCollection services,
        string collectionName
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(collectionName);

        return AddLiteDbCache(services, options => options.CollectionName = collectionName);
    }

    /// <summary>
    /// Adds an implementation of <see cref="IFusionCacheSerializer"/> which uses LiteDb serializer.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddFusionCacheLiteDbCacheSerializer(
        this IServiceCollection services
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton<IFusionCacheSerializer>(_ => new LiteDbCacheSerializer());
        return services;
    }

    /// <summary>
    /// Adds an <see cref="IFusionCacheSerializer"/> which uses LiteDb serializer.
    /// </summary>
    /// <param name="builder">The <see cref="IFusionCacheBuilder" /> to add the serializer to.</param>
    /// <returns>The <see cref="IFusionCacheBuilder"/> so that additional calls can be chained.</returns>
    public static IFusionCacheBuilder WithLiteDbCacheSerializer(this IFusionCacheBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        return builder.WithSerializer(new LiteDbCacheSerializer());
    }

    private static IServiceCollection TryConfigure<TOptions>(
        this IServiceCollection services,
        Action<TOptions> setup
    )
        where TOptions : class
    {
        if (services.Any(d => d.ServiceType == typeof(IConfigureOptions<TOptions>)))
            return services;

        services.Configure(setup);

        return services;
    }
}
