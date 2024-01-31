using Microsoft.AspNetCore.SignalR;
using OrderBook.NotifierService.Application.OrderBookUpdates.Dto;
using OrderBook.NotifierService.Application.OrderBookUpdates.Services;
using OrderBook.NotifierService.Application.WalletUpdates.Dto;
using OrderBook.NotifierService.Application.WalletUpdates.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Infrastructure.Hubs;

public class UpdatesHub : Hub, IWalletNotifierService, IOrderBookNotifierService
{
    private static readonly ConcurrentDictionary<Guid, string> _clients = new();
    private readonly IHubContext<UpdatesHub> _context;

    public UpdatesHub(IHubContext<UpdatesHub> context)
    {
        _context = context;
    }

    public async Task NotifyOrderBookUpdate(FullOrderBookDto fullOrderBookDto)
    {
        await _context.Clients.All.SendAsync("OrderBookUpdate", fullOrderBookDto);
    }

    public async Task NotifyWalletUpdate(WalletUpdateNotifyDto updateDto)
    {
        if (!_clients.ContainsKey(updateDto.UserId))
        {
            return;
        }

        var connectionId = _clients[updateDto.UserId];
        await _context.Clients.Client(connectionId).SendAsync("WalletUpdate", updateDto);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        var httpCtx = Context.GetHttpContext();
        var userId = httpCtx.Request.Headers["UserId"].ToString();

        _clients.TryRemove(Guid.Parse(userId), out string value);

        return base.OnDisconnectedAsync(exception);
    }

    public override async Task OnConnectedAsync()
    {
        var httpCtx = Context.GetHttpContext();
        var userId = httpCtx.Request.Headers["UserId"].ToString();


        _clients.TryAdd(Guid.Parse(userId), Context.ConnectionId);

        await base.OnConnectedAsync();
    }

}
