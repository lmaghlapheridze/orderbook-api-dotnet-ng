using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Infrastructure.Data;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderHistory> OrderHistories { get; set; }
}
