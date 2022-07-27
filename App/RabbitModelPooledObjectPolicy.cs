using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;

namespace App;

public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
{
    private readonly string _hostName;
    private readonly IConnection _connection;

    public RabbitModelPooledObjectPolicy(IConfiguration configuration)
    {
        _hostName = configuration.GetValue<string>("RabbitMqHost");
        _connection = GetConnection();
    }

    private IConnection GetConnection()
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostName
        };

        return factory.CreateConnection();
    }

    public IModel Create()
    {
        return _connection.CreateModel();
    }

    public bool Return(IModel model)
    {
        if (model.IsOpen)
            return true;

        model.Dispose();

        return false;
    }
}