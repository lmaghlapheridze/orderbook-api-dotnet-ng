using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Application.Wallet.DTO;
public class WalletUpdateDTO
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
}
