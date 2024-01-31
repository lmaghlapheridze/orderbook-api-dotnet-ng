using MediatR;
using OrderBook.NotifierService.Application.OrderBookUpdates.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.OrderBookUpdates.Query;
public class OrderBookUpdateQuery : IRequest
{
}

public class UpdateIrderBookQueryHandler : IRequestHandler<OrderBookUpdateQuery>
{
    private readonly IOrderBookService _ordersService;
    private readonly IOrderBookNotifierService _orderBookNotifierService;

    public UpdateIrderBookQueryHandler(IOrderBookService ordersService,
                                       IOrderBookNotifierService orderBookNotifierService)
    {
        _ordersService = ordersService;
        _orderBookNotifierService = orderBookNotifierService;
    }

    public async Task Handle(OrderBookUpdateQuery request, CancellationToken cancellationToken)
    {
        var fullOrderBook = await _ordersService.GetFullOrderBook();

        await _orderBookNotifierService.NotifyOrderBookUpdate(fullOrderBook);
    }
}
