﻿using System.Diagnostics.CodeAnalysis;
using Kava.Services.Abstractions.Factories;
using Kava.Services.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Kava.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFactory<
        TService,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TImplementation
    >(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.TryAddTransient<TService, TImplementation>();
        services.TryAddSingleton<Func<TService>>(sp => sp.GetRequiredService<TService>);
        services.TryAddSingleton<IFactory<TService>, Factory<TService>>();

        return services;
    }

    public static IServiceCollection AddFactory<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            TImplementation
    >(this IServiceCollection services)
        where TImplementation : class
    {
        services.TryAddTransient<TImplementation>();
        services.TryAddSingleton<Func<TImplementation>>(sp =>
            sp.GetRequiredService<TImplementation>
        );
        services.TryAddSingleton<IFactory<TImplementation>, Factory<TImplementation>>();

        return services;
    }

    public static IServiceCollection AddFactory<TImplementation>(
        this IServiceCollection services,
        Func<IServiceProvider, TImplementation> factory
    )
        where TImplementation : class
    {
        services.TryAddTransient(factory);
        services.TryAddSingleton<Func<TImplementation>>(sp =>
            sp.GetRequiredService<TImplementation>
        );
        services.TryAddSingleton<IFactory<TImplementation>, Factory<TImplementation>>();

        return services;
    }
}
