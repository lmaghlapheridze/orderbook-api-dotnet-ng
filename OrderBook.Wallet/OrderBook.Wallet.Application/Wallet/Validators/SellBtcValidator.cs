using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Order.Commands;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Wallets.Validators;
public class SellBtcValidator : AbstractValidator<SellBtcCommand>
{
    private readonly IApplicationDbContext _context;

    public SellBtcValidator(IApplicationDbContext context)
    {
        _context = context;


        RuleFor(x => x.OutGoingWalletId)
            .MustAsync(ExistWallet)
            .WithMessage("Wallet Does Not Exist");

        RuleFor(x => x.OutGoingWalletId)
            .MustAsync(BeBTCWallet)
            .WithMessage("Cant Sell BTC With From This Wallet");

        RuleFor(x => x.IncomingWalletId)
            .MustAsync(BeUSDTWallet)
            .WithMessage("Cant Sell BTC With With This Wallet");

        RuleFor(x => new { x.OutGoingWalletId, x.Amount })
            .MustAsync(async (x, cancellation) => await HaveEnoughBalance(x.OutGoingWalletId, x.Amount, cancellation))
            .WithMessage("Not Enough Balance");


    }

    public async Task<bool> BeUSDTWallet(Guid walletId, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallet.AsNoTracking().FirstAsync(x => x.WalletId == walletId, cancellationToken);

        return wallet.CurrencyType == CurrencyType.USDT;
    }

    public async Task<bool> BeBTCWallet(Guid walletId, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallet.AsNoTracking().FirstAsync(x => x.WalletId == walletId, cancellationToken);

        return wallet.CurrencyType == CurrencyType.BTC;
    }

    public async Task<bool> HaveEnoughBalance(Guid walletId, decimal amount, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallet.AsNoTracking().FirstAsync(x => x.WalletId == walletId, cancellationToken);

        return wallet.CurrentAmount > amount;
    }

    public async Task<bool> ExistWallet(Guid walletId, CancellationToken cancellationToken)
    {
        return await _context.Wallet.AsNoTracking().AnyAsync(x => x.WalletId == walletId, cancellationToken);

    }
}
