using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Wallets.Application.Common.Behaviours;
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);


        var validationTasks = _validators
            .Select(async validator => await validator.ValidateAsync(context, cancellationToken))
            .ToArray();

        await Task.WhenAll(validationTasks);

        var failures = validationTasks
            .SelectMany(validationTask => validationTask.Result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Any())
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}