using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OrderBook.Wallets.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace OrderBook.Wallets.Application.Common.Interfaces;
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Wallet> Wallet { get; }
    DbSet<WalletOperation> WalletOperations { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<IDbContextTransaction> StartNewTransaction(CancellationToken cancellations);
    DatabaseFacade Database { get; }
}
