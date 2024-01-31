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
public class BuyBtcValidator : AbstractValidator<BuyBtcCommand>
{
    private readonly IApplicationDbContext _context;

    public BuyBtcValidator(IApplicationDbContext context)
    {
        _context = context;


        RuleFor(x => x.OutGoingWalletId)
            .MustAsync(ExistWallet)
            .WithMessage("Wallet Does Not Exist");

        RuleFor(x => x.OutGoingWalletId)
            .MustAsync(BeUSDTWallet)
            .WithMessage("Cant Buy BTC With This Currency");

        RuleFor(x => x.IncomingWalletId)
            .MustAsync(BeBTCWallet)
            .WithMessage("Cant Buy BTC With This Wallet");

        RuleFor(x => new { x.OutGoingWalletId, x.Amount, x.Price })
            .MustAsync(async (x, cancellation) => await HaveEnoughBalance(x.OutGoingWalletId, x.Amount, x.Price, cancellation))
            .WithMessage("Not Enough Balance");


    }

    public async Task<bool> BeBTCWallet(Guid walletId, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallet.AsNoTracking().FirstAsync(x => x.WalletId == walletId, cancellationToken);

        return wallet.CurrencyType == CurrencyType.BTC;
    }

    public async Task<bool> BeUSDTWallet(Guid walletId, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallet.AsNoTracking().FirstAsync(x => x.WalletId == walletId, cancellationToken);

        return wallet.CurrencyType == CurrencyType.USDT;
    }

    public async Task<bool> HaveEnoughBalance(Guid walletId, decimal amount, decimal price, CancellationToken cancellationToken)
    {
        var wallet = await _context.Wallet.AsNoTracking().FirstAsync(x => x.WalletId == walletId,cancellationToken);

        return wallet.CurrentAmount > amount * price;
    }

    public async Task<bool> ExistWallet(Guid walletId, CancellationToken cancellationToken)
    {
        return await _context.Wallet.AsNoTracking().AnyAsync(x => x.WalletId == walletId, cancellationToken);

    }
}
