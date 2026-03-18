namespace PaymentSimulator.Api.Contracts;

public class PaymentRequest
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public PaymentMethodDto PaymentMethod { get; set; }
    public CustomerDto Customer { get; set; }
    public string Reference { get; set; }
}

public class PaymentMethodDto
{
    public string Type { get; set; }
    public string CardLast4 { get; set; }
    public string CardBrand { get; set; }
}

public class CustomerDto
{
    public string CustomerId { get; set; }
    public string Email { get; set; }
}
