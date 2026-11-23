
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
public static class PostGreetingEndpoint
{
    public static void MapPostGreetingEndpoint(this IEndpointRouteBuilder app)
    {
        
        app.MapPost("/greet", ([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] UserRequest? user) =>
        {
            var message = $"Hello, {user!.Name}. I know that you are {user.Age} years old";
            return Results.Ok(new { Message = message });
        }).AddEndpointFilter<ValidationFiilter<UserRequest>>();
    }
}