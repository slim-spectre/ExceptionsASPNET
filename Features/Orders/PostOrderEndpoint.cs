public static class PostOrderEndpoint
{
    public static void MapPostOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/order", (DrinkRequest dto) =>
        {
            if (dto.Name == "espresso")
            {
                throw new CoffeeMachineException("Water is empty....");
            }
            if (dto.Name == "cappuccino")
            {
                throw new TimeoutException("Machine did not respond in time.");
            }
            
            return Results.Ok(new {Message = $"Your dto:{dto.Name} is ready"});
        }).AddEndpointFilter<CoffeeValidationFilter>();
    }
}


public record DrinkRequest(string Name);