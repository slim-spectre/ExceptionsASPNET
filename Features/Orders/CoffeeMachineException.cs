public class CoffeeMachineException : Exception
{
    public int StatusCode { get; } = StatusCodes.Status503ServiceUnavailable;

    public string Reason { get; } = "OUT_OF_WATER";

    public CoffeeMachineException() { }
    
    public CoffeeMachineException(string message, string reason = "OUT_OF_WATER") 
        : base(message)
    {
        Reason = reason;
    }
}