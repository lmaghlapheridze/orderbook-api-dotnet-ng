using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Users.Queries;
public class GetUserQuery : IRequest<User>
{
    public string UserName { get; set; }
}

public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
{
    private readonly IApplicationDbContext _context;

    public GetUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
                            .Include(u => u.Wallets)
                            .ThenInclude(x => x.WalletOperations)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
        return user;
    }
}
