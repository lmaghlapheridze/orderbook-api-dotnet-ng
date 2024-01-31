using OrderBook.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Domain.Entities;
public class Order
{
    public long OrderId { get; set; }
    public OrderType OrderType { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
    public DateTime PlaceDate { get; set; }
    public Guid OutGoingWalletId { get; set; }
    public Guid IncomingWalletId { get; set; }
    public long ParentOrderId { get; set; }
}
