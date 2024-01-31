using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Application.Orders.Commands;
using OrderBook.Orders.Application.Wallet.DTO;
using OrderBook.Orders.Application.Wallet.Service;
using OrderBook.Orders.Domain.Enums;
using OrderBook.Wallets.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderBook.Orders.Tests.Orders;
public class FulFillOrderCommandTests
{

    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndNoSellOrdersAtAll_ShouldPlaceNewBuyOrder()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();


        var IncomingWalletId = Guid.NewGuid();
        var OutGoingWalletId = Guid.NewGuid();

        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = 5,
            Price = 10,
            IncomingWalletId = IncomingWalletId,
            OutGoingWalletId = OutGoingWalletId,
            OrderType = Domain.Enums.OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());




        // Assert

        Assert.Equal(1, context.Orders.Count());

        var placedOrder = context.Orders.AsNoTracking().Single();

        Assert.NotNull(placedOrder);
        Assert.Equal(5, placedOrder.Amount);
        Assert.Equal(10, placedOrder.Price);
        Assert.Equal(OutGoingWalletId, placedOrder.OutGoingWalletId);
        Assert.Equal(IncomingWalletId, placedOrder.IncomingWalletId);
        Assert.Equal(OrderType.Buy, placedOrder.OrderType);
        Assert.Equal(0, placedOrder.ParentOrderId);

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Never());

    }


    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndNoMatchingSellOrders_ShouldNotFulfill()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();

        context.Orders.Add(new()
        {
            Amount = 21,
            Price = 27,
            IncomingWalletId = Guid.NewGuid(),
            OutGoingWalletId = Guid.NewGuid(),
            OrderType = OrderType.Sell,
            PlaceDate = DateTime.UtcNow,
        });

        context.SaveChanges();

        var BuyIncomingWalletId = Guid.NewGuid();
        var BuyOutGoingWalletId = Guid.NewGuid();

        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = 5,
            Price = 10,
            IncomingWalletId = BuyIncomingWalletId,
            OutGoingWalletId = BuyOutGoingWalletId,
            OrderType = Domain.Enums.OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());



        // Assert

        Assert.Equal(1, context.Orders.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(1, context.Orders.Where(x => x.OrderType == OrderType.Sell).Count());
        Assert.Equal(0, context.OrderHistories.Count());
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Never());

    }

    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndExactlyMatchingSellOrder_ShouldFullyFulfill()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();


        var SellIncomingWalletId = Guid.NewGuid();
        var SellOutGoingWalletId = Guid.NewGuid();

        context.Orders.Add(new()
        {
            Amount = 20,
            Price = 10,
            IncomingWalletId = SellIncomingWalletId,
            OutGoingWalletId = SellOutGoingWalletId,
            OrderType = OrderType.Sell,
            PlaceDate = DateTime.UtcNow,
        });

        context.SaveChanges();


        var BuyIncomingWalletId = Guid.NewGuid();
        var BuyOutGoingWalletId = Guid.NewGuid();

        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = 20,
            Price = 10,
            IncomingWalletId = BuyIncomingWalletId,
            OutGoingWalletId = BuyOutGoingWalletId,
            OrderType = OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());



        // Assert

        Assert.Equal(0, context.Orders.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(0, context.Orders.Where(x => x.OrderType == OrderType.Sell).Count());

        Assert.Equal(2, context.OrderHistories.Count());

        Assert.Equal(20, context.OrderHistories.Where(x => x.OrderType == OrderType.Sell).Single().FulfilledAmount);
        Assert.Equal(20, context.OrderHistories.Where(x => x.OrderType == OrderType.Buy).Single().FulfilledAmount);

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Exactly(2));
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == SellIncomingWalletId && x.Amount == 20 * 10)), Times.Once());
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == BuyIncomingWalletId && x.Amount == 20)), Times.Once());

    }

    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndPartiallyMatchingSellOrder_ShouldSplitBuyOrderAndPartiallyFulfill()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();

        var price = 10;
        var sellAmount = 17.5m;
        var buyAmount = 20;

        var SellIncomingWalletId = Guid.NewGuid();
        var SellOutGoingWalletId = Guid.NewGuid();


        context.Orders.Add(new()
        {
            Amount = sellAmount,
            Price = price,
            IncomingWalletId = SellIncomingWalletId,
            OutGoingWalletId = SellOutGoingWalletId,
            OrderType = OrderType.Sell,
            PlaceDate = DateTime.UtcNow,
        });

        context.SaveChanges();


        var BuyIncomingWalletId = Guid.NewGuid();
        var BuyOutGoingWalletId = Guid.NewGuid();


        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = buyAmount,
            Price = price,
            IncomingWalletId = BuyIncomingWalletId,
            OutGoingWalletId = BuyOutGoingWalletId,
            OrderType = OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());



        // Assert

        Assert.Equal(1, context.Orders.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(0, context.Orders.Where(x => x.OrderType == OrderType.Sell).Count());


        Assert.Equal(2, context.OrderHistories.Count());

        Assert.Equal(sellAmount, context.OrderHistories.Where(x => x.OrderType == OrderType.Sell).Single().FulfilledAmount);
        Assert.Equal(sellAmount, context.OrderHistories.Where(x => x.OrderType == OrderType.Buy).Single().FulfilledAmount);

        var originalBuyOrder = context.OrderHistories.Single(x => x.OrderType == OrderType.Buy);
        var splitBuyOrder = context.Orders.Single();

        Assert.Equal(buyAmount - sellAmount, splitBuyOrder.Amount);
        Assert.Equal(price , splitBuyOrder.Price);
        Assert.Equal(originalBuyOrder.OrderId , splitBuyOrder.ParentOrderId);
        Assert.Equal(OrderType.Buy , splitBuyOrder.OrderType);
        Assert.Equal(BuyOutGoingWalletId, splitBuyOrder.OutGoingWalletId);
        Assert.Equal(BuyIncomingWalletId, splitBuyOrder.IncomingWalletId);

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Exactly(2));

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == SellIncomingWalletId && x.Amount == sellAmount * price)), Times.Once());
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == BuyIncomingWalletId && x.Amount == sellAmount)), Times.Once());

    }

    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndTwoExactlyMatchingSellOrder_ShouldFulfillSellOrderThatsOlder()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();

        var price = 10;
        var sellAmount = 17.5m;
        var buyAmount = sellAmount;

        var OlderSellIncomingWalletId = Guid.NewGuid();
        var OlderSellOutGoingWalletId = Guid.NewGuid();


        context.Orders.Add(new()
        {
            Amount = sellAmount,
            Price = price,
            IncomingWalletId = OlderSellIncomingWalletId,
            OutGoingWalletId = OlderSellOutGoingWalletId,
            OrderType = OrderType.Sell,
            PlaceDate = DateTime.UtcNow.AddMinutes(-10),
        });

        var NewerSellIncomingWalletId = Guid.NewGuid();
        var NewerSellOutGoingWalletId = Guid.NewGuid();

        context.Orders.Add(new()
        {
            Amount = sellAmount,
            Price = price,
            IncomingWalletId = NewerSellIncomingWalletId,
            OutGoingWalletId = NewerSellIncomingWalletId,
            OrderType = OrderType.Sell,
            PlaceDate = DateTime.UtcNow,
        });

        context.SaveChanges();


        var BuyIncomingWalletId = Guid.NewGuid();
        var BuyOutGoingWalletId = Guid.NewGuid();


        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = buyAmount,
            Price = price,
            IncomingWalletId = BuyIncomingWalletId,
            OutGoingWalletId = BuyOutGoingWalletId,
            OrderType = OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());



        // Assert

        Assert.Equal(0, context.Orders.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(1, context.Orders.Where(x => x.OrderType == OrderType.Sell).Count());

        Assert.Equal(NewerSellIncomingWalletId, context.Orders.Single().IncomingWalletId);

        Assert.Equal(2, context.OrderHistories.Count());

        Assert.Equal(sellAmount, context.OrderHistories.Where(x => x.OrderType == OrderType.Sell).Single().FulfilledAmount);
        Assert.Equal(sellAmount, context.OrderHistories.Where(x => x.OrderType == OrderType.Buy).Single().FulfilledAmount);

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Exactly(2));
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == OlderSellIncomingWalletId && x.Amount == sellAmount * price)), Times.Once());
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == BuyIncomingWalletId && x.Amount == sellAmount)), Times.Once());

    }


    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndTenMatchingSellOrder_ShouldFulfillFiveSellOrdersExactly()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();

        var price = 10;
        var sellAmount = 7;
        var buyAmount = 7 * 5;


        var fulfilledSellOrdersIncomingWalletId = new List<Guid>();

        for (int i = 0; i < 5; i++)
        {
            var incomingWalletId = Guid.NewGuid();

            fulfilledSellOrdersIncomingWalletId.Add(incomingWalletId);

            context.Orders.Add(new()
            {
                Amount = sellAmount,
                Price = price,
                IncomingWalletId = incomingWalletId,
                OutGoingWalletId = Guid.NewGuid(),
                OrderType = OrderType.Sell,
                PlaceDate = DateTime.UtcNow,
            });
        }

        for (int i = 0; i < 5; i++)
        {
            var incomingWalletId = Guid.NewGuid();

            context.Orders.Add(new()
            {
                Amount = sellAmount,
                Price = price,
                IncomingWalletId = incomingWalletId,
                OutGoingWalletId = Guid.NewGuid(),
                OrderType = OrderType.Sell,
                PlaceDate = DateTime.UtcNow.AddMinutes(10),
            });
        }

        context.SaveChanges();


        var BuyIncomingWalletId = Guid.NewGuid();
        var BuyOutGoingWalletId = Guid.NewGuid();


        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = buyAmount,
            Price = price,
            IncomingWalletId = BuyIncomingWalletId,
            OutGoingWalletId = BuyOutGoingWalletId,
            OrderType = OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());



        // Assert

        Assert.Equal(0, context.Orders.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(5, context.Orders.Where(x => x.OrderType == OrderType.Sell).Count());


        Assert.Equal(fulfilledSellOrdersIncomingWalletId.Order(), context.OrderHistories
                                                                                .Where(x => x.OrderType == OrderType.Sell)
                                                                                .Select(x => x.IncomingWalletId)
                                                                                .ToList()
                                                                                .Order());

        Assert.Equal(1, context.OrderHistories.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(5, context.OrderHistories.Where(x => x.OrderType == OrderType.Sell).Count());

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Exactly(6));

        foreach(var walletId in fulfilledSellOrdersIncomingWalletId)
        {
            walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == walletId && x.Amount == sellAmount * price)), Times.Once());
        }

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == BuyIncomingWalletId && x.Amount == buyAmount)), Times.Once());

    }

    [Fact]
    public async Task TestFulfillOrder_WithBuyOrderAndTenMatchingSellOrder_ShouldFulfillFourSellOrdersExactlyAndOnePartially()
    {
        // Arrange

        var context = InMemoryContext();
        var walletMessageServiceMock = new Mock<IWalletMessageService>();

        var time = DateTime.UtcNow;
        var price = 10;
        var sellAmount = 7;
        var partiallySellAmount = sellAmount / 10m;
        var buyAmount = sellAmount * 4 + partiallySellAmount;

        var partiallyFulfilledIncomingWalletId = Guid.NewGuid();

        var fulfilledSellOrdersIncomingWalletId = new List<Guid>();

        for (int i = 0; i < 4; i++)
        {
            var incomingWalletId = Guid.NewGuid();

            fulfilledSellOrdersIncomingWalletId.Add(incomingWalletId);

            context.Orders.Add(new()
            {
                Amount = sellAmount,
                Price = price,
                IncomingWalletId = incomingWalletId,
                OutGoingWalletId = Guid.NewGuid(),
                OrderType = OrderType.Sell,
                PlaceDate = time.AddHours(-i),
            });
        }

        fulfilledSellOrdersIncomingWalletId.Add(partiallyFulfilledIncomingWalletId);

        context.Orders.Add(new()
        {
            Amount = sellAmount,
            Price = price,
            IncomingWalletId = partiallyFulfilledIncomingWalletId,
            OutGoingWalletId = Guid.NewGuid(),
            OrderType = OrderType.Sell,
            PlaceDate = time,
        });

        for (int i = 0; i < 5; i++)
        {
            var incomingWalletId = Guid.NewGuid();

            context.Orders.Add(new()
            {
                Amount = sellAmount,
                Price = price,
                IncomingWalletId = incomingWalletId,
                OutGoingWalletId = Guid.NewGuid(),
                OrderType = OrderType.Sell,
                PlaceDate = time.AddMinutes(10 * i),
            });
        }

        context.SaveChanges();


        var BuyIncomingWalletId = Guid.NewGuid();
        var BuyOutGoingWalletId = Guid.NewGuid();


        var newBuyOrder = new FulfillOrderCommand()
        {
            Amount = buyAmount,
            Price = price,
            IncomingWalletId = BuyIncomingWalletId,
            OutGoingWalletId = BuyOutGoingWalletId,
            OrderType = OrderType.Buy,
        };

        var handler = new FulfillOrderCommandHandler(context, walletMessageServiceMock.Object);




        // Act

        await handler.Handle(newBuyOrder, new());



        // Assert

        Assert.Equal(0, context.Orders.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(6, context.Orders.Where(x => x.OrderType == OrderType.Sell).Count());

        var splitSellOrder = context.Orders.Single(x => x.IncomingWalletId == partiallyFulfilledIncomingWalletId);

        Assert.Equal(sellAmount - partiallySellAmount, splitSellOrder.Amount);


        Assert.Equal(fulfilledSellOrdersIncomingWalletId.Order(), context.OrderHistories
                                                                                .Where(x => x.OrderType == OrderType.Sell)
                                                                                .Select(x => x.IncomingWalletId)
                                                                                .ToList()
                                                                                .Order());

        Assert.Equal(1, context.OrderHistories.Where(x => x.OrderType == OrderType.Buy).Count());
        Assert.Equal(5, context.OrderHistories.Where(x => x.OrderType == OrderType.Sell).Count());

        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.IsAny<WalletUpdateDTO>()), Times.Exactly(6));
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == partiallyFulfilledIncomingWalletId && x.Amount == partiallySellAmount * price)), Times.Once());
        walletMessageServiceMock.Verify(x => x.WalletUpdate(It.Is<WalletUpdateDTO>(x => x.WalletId == BuyIncomingWalletId && x.Amount == buyAmount)), Times.Once());

    }

    private static ApplicationDbContext InMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        var _context = new ApplicationDbContext(options);
        return _context;
    }
}
