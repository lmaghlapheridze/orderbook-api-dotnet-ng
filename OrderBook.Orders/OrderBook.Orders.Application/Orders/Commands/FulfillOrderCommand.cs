using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Application.Orders.Helpers;
using OrderBook.Orders.Application.Wallet.DTO;
using OrderBook.Orders.Application.Wallet.Service;
using OrderBook.Orders.Domain.Entities;
using OrderBook.Orders.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace OrderBook.Orders.Application.Orders.Commands;
public class FulfillOrderCommand: IRequest
{
    public OrderType OrderType { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }
    public Guid OutGoingWalletId { get; set; }
    public Guid IncomingWalletId { get; set; }
}

public class FulfillOrderCommandHandler : IRequestHandler<FulfillOrderCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IWalletMessageService _walletMessageService;


    public FulfillOrderCommandHandler(IApplicationDbContext context, 
                                       IWalletMessageService walletMessageService)
    {
        _context = context;
        _walletMessageService = walletMessageService;
    }

    public async Task Handle(FulfillOrderCommand request, CancellationToken cancellationToken)
    {
        using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, cancellationToken))
        {
            var newOrder = new Order()
            {
                Amount = request.Amount,
                Price = request.Price,
                PlaceDate = DateTime.UtcNow,
                OrderType = request.OrderType,
                OutGoingWalletId = request.OutGoingWalletId,
                IncomingWalletId = request.IncomingWalletId
            };

            var oppositeType = OrderTypeHelper.GetOpposite(newOrder.OrderType);


            var potentialMatches = await _context.Orders
                                        .Where(x => x.OrderType == oppositeType && x.Price == request.Price)
                                        .ToListAsync(cancellationToken);

            var t = await _context.Orders.ToListAsync();

            if (potentialMatches.Count == 0)
            {
                await _context.Orders.AddAsync(newOrder, cancellationToken);

                await transaction.CommitAsync(cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            var fulfilledOrders = FulfillOrders(newOrder, potentialMatches);

            await SplitPartiallyFulfilledOrder(fulfilledOrders);

            foreach (var fulfilledOrder in fulfilledOrders)
            {
                await _context.OrderHistories.AddAsync(fulfilledOrder, cancellationToken);
            }


            await transaction.CommitAsync(cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            await UpdateWallets(fulfilledOrders);

        }
    }

    private async Task UpdateWallets(List<OrderHistory> fulfilledOrders)
    {
        foreach (var fulfilledOrder in fulfilledOrders)
        {
            var fulfilledAmount = fulfilledOrder.FulfilledAmount;

            if(fulfilledOrder.OrderType == OrderType.Sell)
            {
                fulfilledAmount *= fulfilledOrder.Price;
            }

            await _walletMessageService.WalletUpdate(new WalletUpdateDTO()
            {
                Amount = fulfilledAmount,
                WalletId = fulfilledOrder.IncomingWalletId
            });
        }
    }

    private async Task SplitPartiallyFulfilledOrder(List<OrderHistory> fulfilledOrders)
    {
        var partiallyFulfilled = fulfilledOrders.FirstOrDefault(IsPartiallyFulfilled);

        if (partiallyFulfilled != null)
        {
            await _context.Orders.AddAsync(new()
            {
                Amount = partiallyFulfilled.Amount - partiallyFulfilled.FulfilledAmount,
                OrderType = partiallyFulfilled.OrderType,
                ParentOrderId = partiallyFulfilled.OrderId,
                PlaceDate = partiallyFulfilled.PlaceDate,
                OutGoingWalletId = partiallyFulfilled.OutgoingWalletId,
                IncomingWalletId = partiallyFulfilled.IncomingWalletId,
                Price = partiallyFulfilled.Price,
            });
        }
    }

    private bool IsPartiallyFulfilled(OrderHistory x)
    {
        return x.Amount > x.FulfilledAmount;
    }

    private List<OrderHistory> FulfillOrders(Order newOrder, List<Order> potentialMatches)
    {
        var fulfilledOrders = new List<OrderHistory>();

        var sortedPotentialMatches = potentialMatches.OrderBy(x => x.PlaceDate);

        var amountToFulfill = newOrder.Amount;

        foreach (var potentialMatch in potentialMatches)
        {
            if (potentialMatch.Amount < amountToFulfill)
            {

                fulfilledOrders.Add(new OrderHistory(potentialMatch, potentialMatch.Amount));
                amountToFulfill -= potentialMatch.Amount;

                _context.Orders.Remove(potentialMatch);

            }
            else if (amountToFulfill > 0)
            {
                fulfilledOrders.Add(new OrderHistory(potentialMatch, amountToFulfill));

                amountToFulfill = 0;
                _context.Orders.Remove(potentialMatch);

                break;
            }
        }

        var fullfiledAmount = newOrder.Amount - amountToFulfill;

        fulfilledOrders.Add(new OrderHistory(newOrder, fullfiledAmount));

        return fulfilledOrders;
    }

}
