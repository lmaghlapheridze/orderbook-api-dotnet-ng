using Microsoft.Extensions.Options;
using OrderBook.Orders.Application.Wallet.DTO;
using OrderBook.Orders.Application.Wallet.Service;
using OrderBook.Orders.Domain.Entities;
using OrderBook.Orders.Infrastructure.Configs;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderBook.Orders.Infrastructure.WalletMessager;
public class WalletMessageService : IWalletMessageService
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly IModel _channel;
    private IConnection _connection;

    public WalletMessageService(IOptions<RabbitMqConfig> rabbitMqConfig)
    {
        _rabbitMqConfig = rabbitMqConfig.Value;

        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfig.Hostname,
            UserName = _rabbitMqConfig.Username,
            Password = _rabbitMqConfig.Password,
            Port = _rabbitMqConfig.Port
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _rabbitMqConfig.WalletMessagesExchange,
                                 type: ExchangeType.Fanout,
                                 durable: true,
                                 autoDelete: false);

    }
    public async Task WalletUpdate(WalletUpdateDTO update)
    {
        var message = JsonSerializer.Serialize(update);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: _rabbitMqConfig.WalletMessagesExchange,
                              routingKey: string.Empty,
                              basicProperties: null,
                              body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
