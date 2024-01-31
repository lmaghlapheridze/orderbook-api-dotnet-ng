using MediatR;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderBook.Orders.Application.Orders.Commands;
using OrderBook.Orders.Infrastructure.Configs;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;

namespace OrderBook.Orders.Subscribers;

public class OrderMatchingEngine : BackgroundService
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly IModel _channel;
    private IConnection _connection;
    private IServiceScopeFactory _serviceScopeFactory;
    public OrderMatchingEngine(IOptions<RabbitMqConfig> rabbitMqConfig, 
                               IServiceScopeFactory serviceScopeFactory)
    {
        _rabbitMqConfig = rabbitMqConfig.Value;

        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfig.Hostname,
            UserName = _rabbitMqConfig.Username,
            Password = _rabbitMqConfig.Password,
            Port = _rabbitMqConfig.Port
        };

        //
        // THIS CODE IS BECAUSE OF DOCKER
        // RETRIES CONNECTION TO RABBITMQ 10 TIMES
        // ITS A BIT SILLY (UCEB DAVWERE)
        //

        for(int i = 0; i < 10; i++)
        {
            try
            {
                _connection = factory.CreateConnection();
                break;
            }
            catch (Exception ex)
            {
                Thread.Sleep(10000);
            }

        }

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _rabbitMqConfig.OrderMessagesExchange,
                                 type: ExchangeType.Direct,
                                 durable: true,
                                 autoDelete: false);

        var queue = _channel.QueueDeclare(queue: _rabbitMqConfig.OrderMessagesQueue,
                                          durable: true,
                                          exclusive: false,
                                          autoDelete: false);

        _channel.QueueBind(_rabbitMqConfig.OrderMessagesQueue, _rabbitMqConfig.OrderMessagesExchange, "");
        _serviceScopeFactory = serviceScopeFactory;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine(" [x] Received {0}", message);


            var command = JsonSerializer.Deserialize<FulfillOrderCommand>(message);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(command);
            }

                
            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _rabbitMqConfig.OrderMessagesQueue, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}
