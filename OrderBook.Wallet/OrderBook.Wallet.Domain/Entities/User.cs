using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Domain.Entities;
public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public List<Wallet> Wallets { get; set; }
}
