using Microsoft.Extensions.Options;
using OrderBook.Wallets.Application.Order.Dto;
using OrderBook.Wallets.Application.Order.Services;
using OrderBook.Wallets.Infrastructure.Config;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Infrastructure.OrderMessage;
public class OrderMessageService : IOrderMessageService, IDisposable
{
    private readonly RabbitMqConfig _rabbitMqConfig;
    private readonly IModel _channel;
    private IConnection _connection;

    public OrderMessageService(IOptions<RabbitMqConfig> rabbitMqConfig)
    {
        _rabbitMqConfig = rabbitMqConfig.Value;

        var factory = new ConnectionFactory()
        {
            HostName = _rabbitMqConfig.Hostname,
            UserName = _rabbitMqConfig.Username,
            Password = _rabbitMqConfig.Password,
            Port     = _rabbitMqConfig.Port
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _rabbitMqConfig.OrderMessagesExchange,
                                 type: ExchangeType.Direct,
                                 durable: true,
                                 autoDelete: false);

    }

    public async Task PlaceOrder(PlaceOrderDto order)
    {
        var message = JsonSerializer.Serialize(order);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = _channel.CreateBasicProperties();
        properties.Persistent = true;
        
        _channel.BasicPublish(exchange: _rabbitMqConfig.OrderMessagesExchange,
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
