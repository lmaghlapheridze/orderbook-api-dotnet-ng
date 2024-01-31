using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Wallets.Dto;
public class WalletUpdateDto
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
}
