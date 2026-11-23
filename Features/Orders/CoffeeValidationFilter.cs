public class CoffeeValidationFilter : IEndpointFilter
{
    private static readonly string[] AllowedDrinks = ["latte", "cappuccino", "espresso"];
    public async ValueTask<object?> InvokeAsync (
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var dto = context.Arguments.OfType<DrinkRequest>().FirstOrDefault();
        var nameOfDrink = dto?.Name?.Trim().ToLowerInvariant();

        if(string.IsNullOrEmpty(nameOfDrink) || !AllowedDrinks.Contains(nameOfDrink))
        {
            var errors = new Dictionary<string, string[]>
            {
                ["Name"] = ["Allowed drinks are only latte cappicion and eespresso"]
            };
            return Results.ValidationProblem(
                errors: errors,
                extensions: new Dictionary<string, object?>
                {
                    ["reason"] = "IVALID_DRINK_TYPE"
                }
            );
        }


        return await next(context);
    }
}