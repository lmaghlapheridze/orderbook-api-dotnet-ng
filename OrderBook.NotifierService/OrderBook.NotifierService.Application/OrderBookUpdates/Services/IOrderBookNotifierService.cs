using OrderBook.NotifierService.Application.OrderBookUpdates.Dto;
using OrderBook.NotifierService.Application.WalletUpdates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.OrderBookUpdates.Services;
public interface IOrderBookNotifierService
{
    public Task NotifyOrderBookUpdate(FullOrderBookDto fullOrderBookDto);
}
