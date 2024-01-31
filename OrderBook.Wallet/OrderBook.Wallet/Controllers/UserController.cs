using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderBook.Wallets.Api.Controllers;
using OrderBook.Wallets.Application.Users.Commands;
using OrderBook.Wallets.Application.Users.Queries;

namespace OrderBook.Wallets.Controllers;
[ApiController]
[Route("[controller]")]
public class UserController(IMediator mediator) : BaseController(mediator)
{
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserCommand request)
    {
        return Ok(await _mediator.Send(request));
    }

    [HttpGet]

    public async Task<IActionResult> GetUser([FromQuery] GetUserQuery request)
    {
        return Ok(await _mediator.Send(request));
    }
}