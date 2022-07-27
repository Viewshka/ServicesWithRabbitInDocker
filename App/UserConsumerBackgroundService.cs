using System.Text;
using System.Text.Json;
using App.Db;
using App.Db.Entities;
using App.Models;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace App;

public class UserConsumerBackgroundService : BackgroundService
{
    private readonly AppDbContext _context;
    private readonly DefaultObjectPool<IModel> _objectPool;

    public UserConsumerBackgroundService(IPooledObjectPolicy<IModel> objectPolicy, AppDbContext context)
    {
        _context = context;
        _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount * 2);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var channel = _objectPool.Get();

        try
        {
            channel.ExchangeDeclare("user-service", ExchangeType.Fanout, true, false, null);

            var queue = channel.QueueDeclare("app.user-service", true, false, false);

            channel.QueueBind(queue.QueueName, "user-service", "");

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (sender, args) =>
            {
                var body = args.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var user = JsonSerializer.Deserialize<UserDto>(message);

                UpdateUser(user);
            };

            channel.BasicConsume(queue.QueueName, true, consumer);

            return Task.CompletedTask;
        }
        finally
        {
            _objectPool.Return(channel);
        }
    }

    private void UpdateUser(UserDto user)
    {
        var existedUser = _context.Users.SingleOrDefault(u => u.Id == user.Id);

        if (existedUser is null)
        {
            var entity = new User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                IsFired = user.IsFired,
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            _context.Users.Add(entity);
            _context.SaveChanges();

            return;
        }

        if (user.IsFired)
        {
            existedUser.IsFired = user.IsFired;
            existedUser.LastModified = DateTime.Now;

            _context.SaveChanges();

            return;
        }

        existedUser.FirstName = user.FirstName;
        existedUser.MiddleName = user.MiddleName;
        existedUser.LastName = user.LastName;

        _context.SaveChanges();
    }
}