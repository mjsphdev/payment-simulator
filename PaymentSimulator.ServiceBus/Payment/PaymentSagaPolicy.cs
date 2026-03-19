using Microsoft.Extensions.Logging;
using PaymentSimulator.Events;
using PaymentSimulator.ServiceBus.Contracts;

namespace PaymentSimulator.ServiceBus.Payment;

public class PaymentSagaPolicy : Saga<PaymentSagaPolicyData>,
    IAmStartedByMessages<PaymentReceivedEvent>,
    IHandleMessages<PaymentSuccessEvent>,
    IHandleMessages<PaymentFailedEvent>
{
    private readonly ILogger<PaymentSagaPolicy> _logger;

    public PaymentSagaPolicy(ILogger<PaymentSagaPolicy> logger)
    {
        _logger = logger;
    }

    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PaymentSagaPolicyData> mapper)
    {
        mapper.MapSaga(sagaData => sagaData.PaymentId)
            .ToMessage<PaymentReceivedEvent>(message => message.PaymentId)
            .ToMessage<PaymentSuccessEvent>(message => message.PaymentId)
            .ToMessage<PaymentFailedEvent>(message => message.PaymentId);
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

            await context.Publish(new PaymentSuccessEvent
            {
                PaymentId = message.PaymentId,
                Amount = message.Amount,
                Currency = message.Currency
            });
        }
        else
        {
            _logger.LogError("[{PaymentId}] Payment processing failed: {Reason}", message.PaymentId, result.FailureReason);

            await context.Publish(new PaymentFailedEvent
            {
                PaymentId = message.PaymentId,
                Amount = message.Amount,
                Currency = message.Currency,
                FailureReason = "Sample failed reason: Card was locked."
            });
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

    public Task Handle(PaymentSuccessEvent message, IMessageHandlerContext context)
    {
        _logger.LogInformation("[{PaymentId}] Payment successfully cleared!", message.PaymentId);

        _logger.LogInformation("[{PaymentId}] Marking payment as COMPLETED", message.PaymentId);

        _logger.LogInformation("[{PaymentId}] Sending receipt email (simulated)", message.PaymentId);

        MarkAsComplete();

        return Task.CompletedTask;
    }

    public Task Handle(PaymentFailedEvent message, IMessageHandlerContext context)
    {
        _logger.LogWarning("[{PaymentId}] Notifying customer of failure... [{FailureReason}]", message.PaymentId, message.FailureReason);

        return Task.CompletedTask;
    }
}
