namespace PaymentSimulator.Events;

public class PaymentReceivedEvent : IEvent
{
    public Guid PaymentId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; }
    public string PaymentType { get; init; }
    public string CardLast4 { get; init; }
    public string CardBrand { get; init; }
    public string CustomerId { get; init; }
    public string Reference { get; init; }
}
