using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Order.Dto;
public class PlaceOrderDto
{
    public Guid IncomingWalletId { get; set; }
    public Guid OutGoingWalletId { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
    public OrderType OrderType { get; set; }
    public DateTime PlaceDate { get; set; }
}
