using Microsoft.AspNetCore.Mvc;
using OrderBook.NotifierService.Application.WalletUpdates.Services;

namespace OrderBook.NotifierService.Controllers;
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly IWalletNotifierService walletNotifierService;
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWalletNotifierService walletNotifierService)
    {
        _logger = logger;
        this.walletNotifierService = walletNotifierService;
    }

    [HttpGet()]
    public async Task TEST()
    {
        await walletNotifierService.NotifyWalletUpdate(new()
        {
            CurrentAmount = 11,
            UserId = Guid.Parse("0263dd21-d064-4c68-803d-d2480fe2155c"),
            WalletId = Guid.NewGuid(),
        });
    }
}
