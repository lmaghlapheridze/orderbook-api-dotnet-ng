using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Infrastructure.Config;
public class RabbitMqConfig
{
    public string Hostname { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string NotificationsExchange { get; set; }
    public string WalletNotificationsQueue { get; set; }
}
