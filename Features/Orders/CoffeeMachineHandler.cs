using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class CoffeeMachineExceptionHandler : IExceptionHandler
{
    private readonly ILogger<CoffeeMachineExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public CoffeeMachineExceptionHandler(ILogger<CoffeeMachineExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not CoffeeMachineException coffeEx)
        {
            return false;
        }

        _logger.LogWarning(coffeEx, "503 error on: {Method} {Path}. TraceId: {TraceId}", ctx.Request.Method, ctx.Request.Path, ctx.TraceIdentifier);

        ctx.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = ctx,
            Exception = coffeEx,
            ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Service Unavailable",
                    Detail = coffeEx.Message
                }
        });
        
    }
}