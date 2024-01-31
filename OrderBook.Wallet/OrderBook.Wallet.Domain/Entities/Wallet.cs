using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Domain.Entities;
public class Wallet
{
    public Guid WalletId { get; set; }
    public Guid UserId { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public decimal CurrentAmount { get; set; }
    public List<WalletOperation> WalletOperations { get; set; }
}
