using MediatR;
using OrderBook.NotifierService.Application.WalletUpdates.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.WalletUpdates.Commands;
public class WalletUpdateQuery : IRequest
{
    public decimal CurrentAmount { get; set; }
    public Guid WalletId { get; set; }
    public Guid UserId { get; set; }
}


public class WalletUpdateQueryHandler : IRequestHandler<WalletUpdateQuery>
{
    private readonly IWalletNotifierService _walletNotifierService;

    public WalletUpdateQueryHandler(IWalletNotifierService walletNotifierService)
    {
        _walletNotifierService = walletNotifierService;
    }

    public async Task Handle(WalletUpdateQuery request, CancellationToken cancellationToken)
    {
        await _walletNotifierService.NotifyWalletUpdate(new()
        {
            CurrentAmount = request.CurrentAmount,
            UserId = request.UserId,
            WalletId = request.WalletId,
        });
    }
}
