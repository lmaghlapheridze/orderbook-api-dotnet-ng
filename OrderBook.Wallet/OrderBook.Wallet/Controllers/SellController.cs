using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderBook.Wallets.Application.Order.Commands;
using OrderBook.Wallets.Application.Users.Commands;
using OrderBook.Wallets.Application.Users.Queries;

namespace OrderBook.Wallets.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class SellController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    [Route("Btc")]
    public async Task<IActionResult> SellBtc(SellBtcCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }
}
