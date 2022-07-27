using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace App.Extensions;

public static class ServicesExtension
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();

        return services;
    }
}