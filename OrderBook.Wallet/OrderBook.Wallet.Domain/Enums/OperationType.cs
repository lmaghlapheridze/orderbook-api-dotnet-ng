using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Domain.Enums;
public enum OperationType
{
    SELL_ORDER,
	BUY_ORDER,
	INCOMING,
	OUTGOING
}
