public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);

        }catch (Exception error)
        {
            _logger.LogError(error, "Unhandled error during request: {Method} {Path}. TraceId: {TraceId}",
            ctx.Request.Method, ctx.Request.Path, ctx.TraceIdentifier);

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new
            {
                Message = $"Internal error occured. TraceId: {ctx.TraceIdentifier}"
            });

        }
    }
}