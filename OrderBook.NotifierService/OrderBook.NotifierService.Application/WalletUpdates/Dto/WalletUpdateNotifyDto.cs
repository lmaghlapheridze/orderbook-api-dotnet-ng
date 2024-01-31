using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Application.WalletUpdates.Dto;
public class WalletUpdateNotifyDto
{
    public decimal CurrentAmount { get; set; }
    public Guid WalletId { get; set; }
    public Guid UserId { get; set; }
}
