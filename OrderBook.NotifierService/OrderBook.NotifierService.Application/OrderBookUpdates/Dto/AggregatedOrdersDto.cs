using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.OrderBookUpdates.Dto;
public class AggregatedOrdersDto
{
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public decimal Total { get; set; }
}
