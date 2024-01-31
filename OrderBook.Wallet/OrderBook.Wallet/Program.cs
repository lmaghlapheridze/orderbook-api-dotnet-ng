using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrderBook.Wallets.Application.Users.Validators;
using OrderBook.Wallets.Application.Common.Interfaces;
using OrderBook.Wallets.Infrastructure.Data;
using MediatR;
using OrderBook.Wallets.Api.Middlewares;
using OrderBook.Wallets.Infrastructure.Config;
using OrderBook.Wallets.Application.Order.Services;
using OrderBook.Wallets.Infrastructure.OrderMessage;
using OrderBook.Wallets.Application.Common.Behaviours;
using OrderBook.Wallets.Application.Wallets.Commands;
using OrderBook.Wallets.Api.Subscribers;
using OrderBook.Wallets.Application.Wallets.Services;
using OrderBook.Wallets.Infrastructure.UpdateNotifier;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
   // options.UseInMemoryDatabase(builder.Configuration.GetSection("ConnectionString").Value),
    options.UseNpgsql(builder.Configuration.GetSection("ConnectionString").Value),
    
    ServiceLifetime.Scoped
);

builder.Services.Configure<RabbitMqConfig>(builder.Configuration.GetSection("RabbitMqConfig"));

builder.Services.AddSingleton<IOrderMessageService, OrderMessageService>();
builder.Services.AddSingleton<IWalletUpdateNotifier, WalletUpdateNotifier>();
builder.Services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(IApplicationDbContext).Assembly));
builder.Services.AddValidatorsFromAssemblyContaining<GetUserValidator>();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddHostedService<WalletUpdatesSubscriber>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
