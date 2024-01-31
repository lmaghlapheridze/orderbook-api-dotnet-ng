using Microsoft.Extensions.Options;
using OrderBook.Wallets.Application.Order.Dto;
using OrderBook.Wallets.Application.Wallets.Dto;
using OrderBook.Wallets.Application.Wallets.Services;
using OrderBook.Wallets.Infrastructure.Config;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Infrastructure.UpdateNotifier;
public class WalletUpdateNotifier : IWalletUpdateNotifier
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly IModel _channel;
    private IConnection _connection;

    public WalletUpdateNotifier(IOptions<RabbitMqConfig> rabbitMqConfig)
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

        _channel.ExchangeDeclare(exchange: _rabbitMqConfig.NotificationsExchange,
                                 type: ExchangeType.Direct,
                                 durable: true,
                                 autoDelete: false);

    }

    public async Task NotifyWalletUpdate(WalletUpdateNotifyDto updateDto)
    {
        var message = JsonSerializer.Serialize(updateDto);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;

        _channel.BasicPublish(exchange: _rabbitMqConfig.NotificationsExchange,
                              routingKey: "Wallet",
                              basicProperties: null,
                              body: body);
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}
