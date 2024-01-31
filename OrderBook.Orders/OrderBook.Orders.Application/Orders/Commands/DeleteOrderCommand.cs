using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Application.Wallet.Service;
using OrderBook.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Application.Orders.Commands;
public class DeleteOrderCommand : IRequest
{
    public long OrderId { get; set; }
}


public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IWalletMessageService _walletMessageService;


    public DeleteOrderCommandHandler(IApplicationDbContext context,
                                     IWalletMessageService walletMessageService)
    {
        _context = context;
        _walletMessageService = walletMessageService;
    }

    public async Task Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
    {
        decimal amountToReturn;
        Guid outGoingWalletId;

        using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, cancellationToken))
        {
            var order = await _context.Orders.SingleAsync(o => o.OrderId == request.OrderId, cancellationToken);
            
            amountToReturn = order.Amount;
            if(order.OrderType == OrderType.Sell)
            {
                amountToReturn *= order.Price;
            }

            outGoingWalletId = order.OutGoingWalletId;

            _context.Orders.Remove(order);

            await transaction.CommitAsync(cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        await _walletMessageService.WalletUpdate(new()
        {
            Amount = amountToReturn,
            WalletId = outGoingWalletId
        });
    }
}
