using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Application.Orders.Dto;
using OrderBook.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Application.Orders.Queries;
public class WalletOrdersQuery : IRequest<List<Order>>
{
    public Guid WalletId { get; set; }
}

public class WalletOrdersQueryHandler : IRequestHandler<WalletOrdersQuery, List<Order>>
{
    private readonly IApplicationDbContext _context;

    public WalletOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Order>> Handle(WalletOrdersQuery request, CancellationToken cancellationToken)
    {
        var walletOrders = await _context.Orders
                                .AsNoTracking()
                                .Where(x => x.OutGoingWalletId == request.WalletId)
                                .ToListAsync();


        return walletOrders;

    }
}