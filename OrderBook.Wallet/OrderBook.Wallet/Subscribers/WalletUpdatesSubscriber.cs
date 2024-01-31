using MediatR;
using Microsoft.Extensions.Options;
using OrderBook.Wallets.Application.Wallets.Dto;
using OrderBook.Wallets.Application.Wallets.Commands;
using OrderBook.Wallets.Infrastructure.Config;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace OrderBook.Wallets.Api.Subscribers;

public class WalletUpdatesSubscriber : BackgroundService
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly IModel _channel;
    private IConnection _connection;
    private IServiceScopeFactory _serviceScopeFactory;

    public WalletUpdatesSubscriber(IOptions<RabbitMqConfig> rabbitMqConfig,
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

        _channel.ExchangeDeclare(exchange: _rabbitMqConfig.WalletMessagesExchange,
                                 type: ExchangeType.Fanout,
                                 durable: true,
                                 autoDelete: false);

        var queue = _channel.QueueDeclare(queue: _rabbitMqConfig.WalletChangesApplierQueue,
                                          durable: true,
                                          exclusive: false,
                                          autoDelete: false);

        _channel.QueueBind(_rabbitMqConfig.WalletChangesApplierQueue, _rabbitMqConfig.WalletMessagesExchange, "");
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


            var dto = JsonSerializer.Deserialize<WalletUpdateDto>(message);

            var command = new WalletUpdateCommand()
            {
                Amount = dto.Amount,
                OperationType = Domain.Enums.OperationType.INCOMING,
                WalletId = dto.WalletId,
            };

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                await mediator.Send(command);
            }


            _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

        _channel.BasicConsume(queue: _rabbitMqConfig.WalletChangesApplierQueue, autoAck: false, consumer: consumer);

        return Task.CompletedTask;
    }
}
