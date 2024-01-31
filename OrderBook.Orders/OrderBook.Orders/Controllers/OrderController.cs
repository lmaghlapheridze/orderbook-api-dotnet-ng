using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderBook.Orders.Application.Orders.Commands;
using OrderBook.Orders.Application.Orders.Dto;
using OrderBook.Orders.Application.Orders.Queries;

namespace OrderBook.Orders.Controllers;
[ApiController]
[Route("[controller]")]
public class OrderController(IMediator mediator) : BaseController(mediator)
{
    [HttpGet]
    public async Task<IActionResult> GetOrders([FromQuery] AggregatedOrdersQuery request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]
    [Route("Wallet")]
    public async Task<IActionResult> GetOrdersByWallet([FromQuery] WalletOrdersQuery request)
    {
        return Ok(await _mediator.Send(request));
    }


    [HttpGet]
    [Route("Full")]
    public async Task<IActionResult> GetFullOrderBook()
    {
        var sellOrders = await _mediator.Send(new AggregatedOrdersQuery { OrderType = Domain.Enums.OrderType.Sell });
        var BuyOrders = await _mediator.Send(new AggregatedOrdersQuery { OrderType = Domain.Enums.OrderType.Buy });

        var fullOrderBook = new FullOrderBookDto()
        {
            SellOrders = sellOrders,
            BuyOrders = BuyOrders
        };

        return Ok(fullOrderBook);

    }

    [HttpDelete]
    public async Task<IActionResult> DeleteOrder([FromQuery] DeleteOrderCommand request)
    {

        await _mediator.Send(request);

        return Ok();

    }
}
