using System.Text.Json;
using Kava.Data.Abstractions;
using Kava.Data.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Kava.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonDataStorage(
        this IServiceCollection services,
        string jsonFolderPath
    )
    {
        services.AddSingleton(sp => new JsonRepository(
            jsonFolderPath,
            sp.GetRequiredService<JsonSerializerOptions>()
        ));
        services.AddSingleton<IRepository>(sp => sp.GetRequiredService<JsonRepository>());
        return services;
    }
}
