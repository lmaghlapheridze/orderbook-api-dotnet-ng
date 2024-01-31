using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Application.Wallets.Services;
using OrderBook.Wallets.Domain.Entities;
using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Wallets.Commands;
public class WalletUpdateCommand : IRequest
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
}

public class WalletUpdateCommandHandler : IRequestHandler<WalletUpdateCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IWalletUpdateNotifier _notifier;

    public WalletUpdateCommandHandler(IApplicationDbContext context, 
                                      IWalletUpdateNotifier notifier)
    {
        _context = context;
        _notifier = notifier;
    }

    public async Task Handle(WalletUpdateCommand request, CancellationToken cancellationToken)
    {

        using (var transaction = await _context.StartNewTransaction(cancellationToken))
        {
            var walletOperation = new WalletOperation()
            {
                Amount = request.Amount,
                OperationType = request.OperationType,
                Timestamp = DateTime.UtcNow,
            };

            var wallet = await _context.Wallet.Include(w => w.WalletOperations).FirstAsync(x => x.WalletId == request.WalletId, cancellationToken);

            wallet.CurrentAmount += request.Amount;

            wallet.WalletOperations.Add(walletOperation);

            await transaction.CommitAsync(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await _notifier.NotifyWalletUpdate(new()
            {
                CurrentAmount = wallet.CurrentAmount,
                UserId = wallet.UserId,
                WalletId = wallet.WalletId,
            });
        }
        
    }
}
