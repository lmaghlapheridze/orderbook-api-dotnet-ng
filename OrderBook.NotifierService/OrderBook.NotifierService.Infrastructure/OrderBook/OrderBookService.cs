using Microsoft.Extensions.Options;
using OrderBook.NotifierService.Application.OrderBookUpdates.Dto;
using OrderBook.NotifierService.Application.OrderBookUpdates.Services;
using OrderBook.NotifierService.Infrastructure.Config;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.NotifierService.Infrastructure.OrderBook;
public class OrderBookService : IOrderBookService   
{
    private readonly ServiceUrls _serviceUrls;

    public OrderBookService(IOptions<ServiceUrls> serviceUrls)
    {
        _serviceUrls = serviceUrls.Value;
    }

    public async Task<FullOrderBookDto> GetFullOrderBook()
    {
        var client = new RestClient(_serviceUrls.OrderBookApi);
        var request = new RestRequest("Order/Full");
        var response = await client.GetAsync<FullOrderBookDto>(request);
        
        return response;
    }
}
