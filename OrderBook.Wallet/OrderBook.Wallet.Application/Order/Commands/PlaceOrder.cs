using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Application.Order.Dto;
using OrderBook.Wallets.Application.Order.Services;
using OrderBook.Wallets.Application.Wallets.Commands;
using OrderBook.Wallets.Domain.Entities;
using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Order.Commands;
public class PlaceOrderCommand : IRequest
{
    public Guid OutGoingWalletId { get; set; }
    public Guid IncomingWalletId { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
    public OrderType OrderType { get; set; }
    public OperationType OperationType { get; set; }
    public decimal WalletChange { get; set; }
}

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IOrderMessageService _orderMessageService;
    private readonly IMediator _mediator;

    public PlaceOrderCommandHandler(IApplicationDbContext context,
                                    IOrderMessageService orderMessageService,
                                    IMediator mediator)
    {
        _context = context;
        _orderMessageService = orderMessageService;
        _mediator = mediator;
    }

    public async Task Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {

        var walletUpdate = new WalletUpdateCommand()
        {
            Amount = request.WalletChange,
            OperationType = request.OperationType,
            WalletId = request.OutGoingWalletId,
        };

        await _mediator.Send(walletUpdate, cancellationToken);

        var orderDto = new PlaceOrderDto()
        {
            Amount = request.Amount,
            PlaceDate = DateTime.UtcNow,
            OrderType = request.OrderType,
            Price = request.Price,
            IncomingWalletId = request.IncomingWalletId,
            OutGoingWalletId = request.OutGoingWalletId,
        };

        await _orderMessageService.PlaceOrder(orderDto);
    }
}
