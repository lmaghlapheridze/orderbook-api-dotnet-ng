using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace OrderBook.Orders.Controllers;
public abstract class BaseController(IMediator mediator) : ControllerBase
{
    protected IMediator _mediator { get; set; } = mediator;
}
