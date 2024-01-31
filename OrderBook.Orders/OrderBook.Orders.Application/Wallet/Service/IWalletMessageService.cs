using OrderBook.Orders.Application.Wallet.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Orders.Application.Wallet.Service;
public interface IWalletMessageService
{
    public Task WalletUpdate(WalletUpdateDTO update);
}
