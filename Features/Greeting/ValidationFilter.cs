public class ValidationFiilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync (
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var dto = context.Arguments.OfType<T>().FirstOrDefault();

        if(dto == null)
        {
            return Results.BadRequest(new CustomProblemDetails
            {
                Title = "Bad Request",
                Status = StatusCodes.Status400BadRequest,
                Detail = "Body of request cannot be null",
                Instance = context.HttpContext.Request.Path,
                TraceId = context.HttpContext.TraceIdentifier
            });
        }


        return await next(context);
    }
}