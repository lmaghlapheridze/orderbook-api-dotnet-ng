using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OrderBook.Wallets.Api.Controllers;
public abstract class BaseController(IMediator mediator) : ControllerBase
{
    protected IMediator _mediator { get; set; } = mediator;
}
