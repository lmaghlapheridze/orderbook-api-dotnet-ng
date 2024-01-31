using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Infrastructure.Configs;
public class RabbitMqConfig
{
    public string Hostname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string OrderMessagesExchange { get; set; }
    public string OrderMessagesQueue { get; set; }
    public string WalletMessagesExchange { get; set; }
}
