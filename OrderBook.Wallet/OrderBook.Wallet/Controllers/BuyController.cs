using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderBook.Wallets.Application.Order.Commands;
using OrderBook.Wallets.Application.Users.Commands;
using OrderBook.Wallets.Application.Users.Queries;

namespace OrderBook.Wallets.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class BuyController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    [Route("Btc")]
    public async Task<IActionResult> BuyBtc(BuyBtcCommand request)
    {
        await _mediator.Send(request);
        return Ok();
    }
}
