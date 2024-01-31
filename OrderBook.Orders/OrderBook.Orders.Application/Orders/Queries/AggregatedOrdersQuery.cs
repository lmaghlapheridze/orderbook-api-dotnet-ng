using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Application.Orders.Dto;
using OrderBook.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.XPath;

namespace OrderBook.Orders.Application.Orders.Queries;
public class AggregatedOrdersQuery : IRequest<List<AggregatedOrdersDto>>
{
    public OrderType OrderType { get; set; }
}

public class AggregatedOrdersQueryHandler : IRequestHandler<AggregatedOrdersQuery, List<AggregatedOrdersDto>>
{
    private readonly IApplicationDbContext _context;

    public AggregatedOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AggregatedOrdersDto>> Handle(AggregatedOrdersQuery request, CancellationToken cancellationToken)
    {
        var aggregated = await _context.Orders
                                .AsNoTracking()
                                .Where(x => x.OrderType == request.OrderType)
                                .GroupBy(o => o.Price)
                                .Select(o => new
                                {
                                    Price = o.Key,
                                    Amount = o.Sum(x => x.Amount)
                                })
                                .ToListAsync();

        var result = aggregated.Select(x => new AggregatedOrdersDto()
        {
            Amount = x.Amount,
            Price = x.Price,
            Total = x.Amount * x.Price
        });

        return result.ToList();

    }
}
