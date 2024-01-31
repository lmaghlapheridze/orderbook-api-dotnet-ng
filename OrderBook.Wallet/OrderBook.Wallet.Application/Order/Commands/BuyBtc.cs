using MediatR;
using OrderBook.Wallets.Application.Order.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Order.Commands;
public class BuyBtcCommand : IRequest
{
    public Guid OutGoingWalletId { get; set; }
    public Guid IncomingWalletId { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class BuyBtcCommandHandler(IMediator mediator) : IRequestHandler<BuyBtcCommand>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(BuyBtcCommand request, CancellationToken cancellationToken)
    {

        

        var placeOrderCommand = new PlaceOrderCommand()
        {
            Amount = request.Amount,
            OperationType = Domain.Enums.OperationType.BUY_ORDER,
            OrderType = Domain.Enums.OrderType.Buy,
            Price = request.Price,
            OutGoingWalletId = request.OutGoingWalletId,
            IncomingWalletId = request.IncomingWalletId,
            WalletChange = -request.Amount * request.Price
        };

        await _mediator.Send(placeOrderCommand, cancellationToken);
    }
}

