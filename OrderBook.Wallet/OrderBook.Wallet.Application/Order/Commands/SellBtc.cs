using MediatR;
using OrderBook.Wallets.Application.Order.Commands;
using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Order.Commands;
public class SellBtcCommand : IRequest
{
    public Guid OutGoingWalletId { get; set; }
    public Guid IncomingWalletId { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class SellBtcCommandHandler(IMediator mediator) : IRequestHandler<SellBtcCommand>
{
    private readonly IMediator _mediator = mediator;

    public async Task Handle(SellBtcCommand request, CancellationToken cancellationToken)
    {
        var placeOrderCommand = new PlaceOrderCommand()
        {
            Amount = request.Amount,
            OperationType = Domain.Enums.OperationType.SELL_ORDER,
            OrderType = OrderType.Sell,
            Price = request.Price,
            OutGoingWalletId = request.OutGoingWalletId,
            IncomingWalletId = request.IncomingWalletId,
            WalletChange = -request.Amount
        };

        await _mediator.Send(placeOrderCommand, cancellationToken);
    }
}

