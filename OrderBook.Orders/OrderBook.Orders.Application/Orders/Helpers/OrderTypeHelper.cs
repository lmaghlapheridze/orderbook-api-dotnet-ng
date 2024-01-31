using OrderBook.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Application.Orders.Helpers;
public static class OrderTypeHelper
{
    public static readonly Dictionary<OrderType, OrderType> mappings
        = new Dictionary<OrderType, OrderType>
        {
            { OrderType.Sell, OrderType.Buy },
            { OrderType.Buy, OrderType.Sell },
        };

    public static OrderType GetOpposite(OrderType type)
    {
        return mappings[type];
    }
}
