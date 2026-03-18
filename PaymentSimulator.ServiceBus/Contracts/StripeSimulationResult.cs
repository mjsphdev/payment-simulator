namespace PaymentSimulator.ServiceBus.Contracts;

public class StripeSimulationResult
{
    public bool IsSuccess { get; private set; }
    public string FailureReason { get; private set; }

    public static StripeSimulationResult Success()
        => new() { IsSuccess = true };

    public static StripeSimulationResult Failure(string reason)
        => new() { IsSuccess = false, FailureReason = reason };
}
