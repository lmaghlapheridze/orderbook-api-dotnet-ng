using OrderBook.NotifierService.Application.OrderBookUpdates.Query;
using OrderBook.NotifierService.Application.OrderBookUpdates.Services;
using OrderBook.NotifierService.Application.WalletUpdates.Services;
using OrderBook.NotifierService.Infrastructure.Config;
using OrderBook.NotifierService.Infrastructure.Hubs;
using OrderBook.NotifierService.Infrastructure.OrderBook;
using OrderBook.NotifierService.Jobs;
using OrderBook.NotifierService.Subscribers;
using OrderBook.Wallets.Infrastructure.Config;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(hubOptions => {
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(10);
    hubOptions.HandshakeTimeout = TimeSpan.FromSeconds(5);
});
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(IWalletNotifierService).Assembly));
builder.Services.Configure<ServiceUrls>(builder.Configuration.GetSection("ServiceUrls"));
builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMqConfig"));
builder.Services.AddSingleton<IWalletNotifierService, UpdatesHub>();
builder.Services.AddSingleton<IOrderBookNotifierService, UpdatesHub>();
builder.Services.AddSingleton<IOrderBookService, OrderBookService>();


builder.Services.AddHostedService<WalletNotificationsSubscriber>();

builder.Services.AddQuartz(q =>
{
    q.ScheduleJob<OrderBookUpdaterJob>
    (
         tg => tg.WithIdentity("OrderBookUpdaterJob", "OrderBookUpdaterJob")
            .WithCronSchedule("0/5 * * * * ?", x => x.InTimeZone(TimeZoneInfo.Utc))
         //.StartNow()
         .ForJob("OrderBookUpdaterJob", "OrderBookUpdaterJob")

    );
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var app = builder.Build();  

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();


app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<UpdatesHub>("/Updates");
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();



app.Run();
