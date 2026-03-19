using PaymentSimulator.Api.Contracts;
using PaymentSimulator.Events;

namespace PaymentSimulator.Api.GraphQL;

[MutationType]
public partial class Mutation
{
    public async Task<string> SubmitPayment(PaymentRequest paymentRequest, [Service] IMessageSession messageSession, [Service] ILogger<Mutation> logger)
    {
        logger.LogInformation("[{PaymentId}] Payment saved to DATABASE (simulated)...", paymentRequest.PaymentId);

        await messageSession.Publish(new PaymentReceivedEvent
        {
            PaymentId = paymentRequest.PaymentId,
            Amount = paymentRequest.Amount,
            Currency = paymentRequest.Currency,
            PaymentType = paymentRequest.PaymentMethod.Type,
            CardLast4 = paymentRequest.PaymentMethod.CardLast4,
            CardBrand = paymentRequest.PaymentMethod.CardBrand,
            CustomerId = paymentRequest.Customer.CustomerId,
            Reference = paymentRequest.Reference
        });

        return "Payment submitted successfully.";
    }
}
