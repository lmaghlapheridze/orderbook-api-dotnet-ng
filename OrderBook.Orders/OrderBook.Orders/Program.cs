using Microsoft.EntityFrameworkCore;
using OrderBook.Orders.Application.Common.Interfaces;
using OrderBook.Orders.Application.Wallet.Service;
using OrderBook.Orders.Infrastructure.Configs;
using OrderBook.Orders.Infrastructure.WalletMessager;
using OrderBook.Orders.Subscribers;
using OrderBook.Wallets.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    //options.UseInMemoryDatabase(builder.Configuration.GetSection("ConnectionString").Value),
    options.UseNpgsql(builder.Configuration.GetSection("ConnectionString").Value),
    ServiceLifetime.Scoped
);

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMqConfig"));
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(IApplicationDbContext).Assembly));
builder.Services.AddSingleton<IWalletMessageService, WalletMessageService>();

builder.Services.AddHostedService<OrderMatchingEngine>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
