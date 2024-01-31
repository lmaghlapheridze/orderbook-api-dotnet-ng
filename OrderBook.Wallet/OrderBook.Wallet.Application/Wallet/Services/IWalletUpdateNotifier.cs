using OrderBook.Wallets.Application.Wallets.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Wallets.Services;
public interface IWalletUpdateNotifier
{
    public Task NotifyWalletUpdate(WalletUpdateNotifyDto updateDto);
}
