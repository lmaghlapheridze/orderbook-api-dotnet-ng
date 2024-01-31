using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OrderBook.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OrderBook.Orders.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderHistory> OrderHistories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);

    DatabaseFacade Database { get; }
}
