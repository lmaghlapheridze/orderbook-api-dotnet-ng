using FluentValidation;
using System.Text.Json;

namespace OrderBook.Wallets.Api.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException validationEx)
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            var errors = JsonSerializer.Serialize(validationEx.Errors.Select(x => x.ErrorMessage));
            await context.Response.WriteAsync(errors);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync("Unhandled error occurred.");
        }
    }
}
