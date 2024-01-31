using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.OrderBookUpdates.Dto;
public class FullOrderBookDto
{
    public List<AggregatedOrdersDto> SellOrders { get; set; }
    public List<AggregatedOrdersDto> BuyOrders { get; set; }
}
