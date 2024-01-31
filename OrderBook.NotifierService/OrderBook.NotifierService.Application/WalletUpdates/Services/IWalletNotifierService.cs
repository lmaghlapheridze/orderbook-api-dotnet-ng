using OrderBook.NotifierService.Application.WalletUpdates.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.WalletUpdates.Services;
public interface IWalletNotifierService
{
    public Task NotifyWalletUpdate(WalletUpdateNotifyDto updateDto);
}
