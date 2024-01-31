using Microsoft.Extensions.Options;
using OrderBook.Wallets.Infrastructure.Config;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using OrderBook.NotifierService.Application.WalletUpdates.Dto;
using OrderBook.NotifierService.Application.WalletUpdates.Commands;
using MediatR;

namespace OrderBook.NotifierService.Subscribers;

public class WalletNotificationsSubscriber : BackgroundService
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly IModel _channel;
    private IConnection _connection;
    private IServiceScopeFactory _serviceScopeFactory;

    public WalletNotificationsSubscriber(IOptions<RabbitMqConfig> rabbitMqConfig,
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

        for (int i = 0; i < 10; i++)
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

        _channel.ExchangeDeclare(exchange: _rabbitMqConfig.NotificationsExchange,
                                 type: ExchangeType.Direct,
                                 durable: true,
                                 autoDelete: false);

        var queue = _channel.QueueDeclare(queue: _rabbitMqConfig.WalletNotificationsQueue,
                                          durable: true,
                                          exclusive: false,
                                          autoDelete: false);

        _channel.QueueBind(_rabbitMqConfig.WalletNotificationsQueue, _rabbitMqConfig.NotificationsExchange, "Wallet");
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


            var dto = JsonSerializer.Deserialize<WalletUpdateNotifyDto>(message);

            var command = new WalletUpdateQuery()
            {
                CurrentAmount = dto.CurrentAmount,
                UserId = dto.UserId,
                WalletId = dto.WalletId,
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(command);
            }

        };

        _channel.BasicConsume(queue: _rabbitMqConfig.WalletNotificationsQueue, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }
}
