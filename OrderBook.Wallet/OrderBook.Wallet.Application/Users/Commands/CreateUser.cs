using MediatR;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Users.Commands;
public class CreateUserCommand : IRequest<User>
{
    public string UserName { get; set; }
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
{
    private readonly IApplicationDbContext _context;

    public CreateUserCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var btcWallet = new Wallet()
        {
            CurrencyType = Domain.Enums.CurrencyType.BTC,
            CurrentAmount = 100,
        };

        var usdtWallet = new Wallet()
        {
            CurrencyType = Domain.Enums.CurrencyType.USDT,
            CurrentAmount = 100,
        };

        var user = new User()
        {
            UserName = request.UserName,
            Wallets = new List<Wallet>() { btcWallet, usdtWallet }
        };

        await _context.Users.AddAsync(user);

        await _context.SaveChangesAsync(cancellationToken);

        return user;
    }
}
