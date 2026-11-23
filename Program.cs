using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";

        context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow;

        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        switch (context.Exception)
        {
            case TimeoutException:
                context.HttpContext.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                context.ProblemDetails.Status = StatusCodes.Status504GatewayTimeout;
                context.ProblemDetails.Title = "Gateway Timeout";
                context.ProblemDetails.Detail = "time out.";
                context.ProblemDetails.Extensions["reason"] = "COFFEE_MACHINE_TIMEOUT";
                break;

            case ArgumentException argEx:
                context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.ProblemDetails.Status = StatusCodes.Status400BadRequest;
                context.ProblemDetails.Title = "Bad Request";
                context.ProblemDetails.Detail = argEx.Message;
                context.ProblemDetails.Extensions["reason"] = "INVALID_ARGUMENT";
                break;

            case CoffeeMachineException coffeeEx:
                context.ProblemDetails.Extensions["reason"] = coffeeEx.Reason;
                break;

            default:
                context.ProblemDetails.Extensions.TryAdd("reason", "INTERNAL_ERROR");
                break;
        }
        
        
    };
});


builder.Services.AddExceptionHandler<CoffeeMachineExceptionHandler>();

var app = builder.Build();


app.UseExceptionHandler(); 
// app.UseMiddleware<ExceptionMiddleware>(); - убрав через конфлікт з іншими хендлерами



app.UseStatusCodePages(async statusCodeContext =>
{
    var response = statusCodeContext.HttpContext.Response;
    if (response.StatusCode == StatusCodes.Status404NotFound)
    {
        await response.WriteAsJsonAsync(new
        {
            Status = 404,
            Message = "Route was not found please reas documentaton",
            TraceId = statusCodeContext.HttpContext.TraceIdentifier
        });
    }
});

app.MapPostGreetingEndpoint();
app.MapPostOrderEndpoint();
app.MapGet("/boom", () => { throw new Exception("test of erro of bd"); });

app.Run();
