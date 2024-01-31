using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Tests.Helpers;
internal class InMemoryDatabase : IApplicationDbContext
{

    public DbSet<Order> Orders => throw new NotImplementedException();

    public DbSet<OrderHistory> OrderHistories => throw new NotImplementedException();

    public DatabaseFacade Database => throw new NotImplementedException();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
