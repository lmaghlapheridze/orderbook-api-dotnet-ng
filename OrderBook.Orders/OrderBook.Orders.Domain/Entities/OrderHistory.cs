using OrderBook.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Domain.Entities;
public class OrderHistory
{
    public long OrderHistoryId { get; set; }
    public long OrderId { get; set; }
    public OrderType OrderType { get; set; }
    public decimal Amount { get; set; }
    public decimal FulfilledAmount { get; set; }
    public decimal Price { get; set; }
    public DateTime PlaceDate { get; set; }
    public Guid OutgoingWalletId { get; set; }
    public Guid IncomingWalletId { get; set; }
    public long ParentOrderId { get; set; }

    public OrderHistory()
    {

    }

    public OrderHistory(Order order, decimal fulfilledAmount) 
    {
        OrderId = order.OrderId;
        OrderType = order.OrderType;
        Amount = order.Amount;
        Price = order.Price;
        PlaceDate = order.PlaceDate;
        OutgoingWalletId = order.OutGoingWalletId;
        IncomingWalletId = order.IncomingWalletId;
        ParentOrderId = order.ParentOrderId;
        FulfilledAmount = fulfilledAmount;
    }
}
