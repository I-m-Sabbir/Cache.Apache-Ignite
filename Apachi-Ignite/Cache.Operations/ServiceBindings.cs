using Microsoft.Extensions.DependencyInjection;

namespace Cache.Operations;

public static class ServiceBindings
{
    public static IServiceCollection UseApache(this IServiceCollection services, params string[] endpoints)
    {
        services.AddSingleton<IClient>(new IgniteConnection(endpoints));
        services.AddScoped<ICacheServicce, CacheService>();

        return services;
    }
}
