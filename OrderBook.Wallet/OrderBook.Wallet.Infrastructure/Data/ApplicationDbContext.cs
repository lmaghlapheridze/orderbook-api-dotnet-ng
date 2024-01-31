using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Infrastructure.Data;
public class ApplicationDbContext : DbContext, IApplicationDbContext
{

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallet { get; set; }
    public DbSet<WalletOperation> WalletOperations { get; set; }
    public Task<IDbContextTransaction> StartNewTransaction(CancellationToken cancellations) =>  Database.BeginTransactionAsync(IsolationLevel.RepeatableRead, cancellations);
}
