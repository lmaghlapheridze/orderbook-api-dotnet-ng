using MediatR;
using OrderBook.NotifierService.Application.OrderBookUpdates.Query;
using Quartz;

namespace OrderBook.NotifierService.Jobs;

[DisallowConcurrentExecution]
public class OrderBookUpdaterJob(IMediator mediator) : IJob
{
    private readonly IMediator _mediator = mediator;

    public async Task Execute(IJobExecutionContext context)
    {
        await _mediator.Send(new OrderBookUpdateQuery());
    }
}
