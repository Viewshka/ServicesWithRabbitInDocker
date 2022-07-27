using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using UserService.Interfaces;

namespace UserService.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitConfig = configuration.GetSection("rabbit");

        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();

        services.AddSingleton<IRabbitManager, RabbitManager>();

        return services;
    }
}