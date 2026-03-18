namespace PaymentSimulator.ServiceBus.Payment;

public class PaymentSagaPolicyData : ContainSagaData
{
    public Guid PaymentId { get; set; }
    public bool IsPaymentProcessed { get; set; }
}
