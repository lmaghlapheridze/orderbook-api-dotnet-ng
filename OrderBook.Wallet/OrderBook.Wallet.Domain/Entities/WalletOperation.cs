using OrderBook.Wallets.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Domain.Entities;
public class WalletOperation
{
    public Guid WalletOperationId { get; set; }
    public OperationType OperationType { get; set; }
    public decimal Amount { get; set; }
    public DateTime Timestamp { get; set; }
}
