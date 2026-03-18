using Microsoft.Extensions.Logging;
using PaymentSimulator.Events;
using PaymentSimulator.ServiceBus.Contracts;

namespace PaymentSimulator.ServiceBus.Payment;

public class PaymentSagaPolicy : Saga<PaymentSagaPolicyData>,
    IAmStartedByMessages<PaymentReceivedEvent>
{
    private readonly ILogger<PaymentSagaPolicy> _logger;

    public PaymentSagaPolicy(ILogger<PaymentSagaPolicy> logger)
    {
        _logger = logger;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PaymentSagaPolicyData> mapper)
    {
        mapper.MapSaga(sagaData => sagaData.PaymentId)
            .ToMessage<PaymentReceivedEvent>(message => message.PaymentId);
    }
    public async Task Handle(PaymentReceivedEvent message, IMessageHandlerContext context)
    {
        if(Data.IsPaymentProcessed)
        {
            _logger.LogWarning("[{PaymentId}] Duplicate request skipped (idempotent)", message.PaymentId);

            return;
        }

        var result = await SimulateStripeAsync(message);

        if(result.IsSuccess)
        {
            Data.IsPaymentProcessed = true;
            _logger.LogInformation("[{PaymentId}] Payment processed successfully", message.PaymentId);


        }
        else
        {
            _logger.LogError("[{PaymentId}] Payment processing failed: {Reason}", message.PaymentId, result.FailureReason);
        }
        return;
    }

    private async Task<StripeSimulationResult> SimulateStripeAsync(PaymentReceivedEvent message)
    {
        // Simulate network delay (realistic)
        await Task.Delay(1000);

        // Random success/failure
        var isSuccess = Random.Shared.Next(0, 10) > 2;

        if (isSuccess)
        {
            _logger.LogInformation(
                "[{PaymentId}] Stripe simulation SUCCESS for {Amount} {Currency} ({CardBrand} ****{Last4})",
                message.PaymentId,
                message.Amount,
                message.Currency,
                message.CardBrand,
                message.CardLast4
            );

            return StripeSimulationResult.Success();
        }

        var reason = "Insufficient funds";

        _logger.LogWarning(
            "[{PaymentId}] Stripe simulation FAILED: {Reason}",
            message.PaymentId,
            reason
        );

        return StripeSimulationResult.Failure(reason);
    }
}
