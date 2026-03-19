namespace PaymentSimulator.Events;

public class PaymentSuccessEvent : IEvent
{
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
}
