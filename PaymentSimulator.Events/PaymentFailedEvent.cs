namespace PaymentSimulator.Events;

public class PaymentFailedEvent : IEvent
{
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public string FailureReason { get; init; }
}
