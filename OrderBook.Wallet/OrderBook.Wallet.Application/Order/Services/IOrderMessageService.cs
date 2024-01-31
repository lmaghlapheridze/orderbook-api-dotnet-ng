using OrderBook.Wallets.Application.Order.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Order.Services;
public interface IOrderMessageService
{
    public Task PlaceOrder(PlaceOrderDto order);
}
