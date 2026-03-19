using Microsoft.AspNetCore.Mvc;
using PaymentSimulator.Api.Contracts;
using PaymentSimulator.Events;

namespace PaymentSimulator.API.Controllers;

[Route("api/payment")]
[ApiController]
public class PaymentController : ControllerBase
{
    private readonly IMessageSession _messageSession;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(IMessageSession messageSession, ILogger<PaymentController> logger)
    {
        _messageSession = messageSession;
        _logger = logger;
    }

    [HttpPost("submit")]
    public async Task<IActionResult> SubmitPaymentAsync([FromBody] PaymentRequest paymentRequest)
    {
        _logger.LogInformation("[{PaymentId}] Payment saved to DATABASE (simulated)...", paymentRequest.PaymentId);

        await _messageSession.Publish(new PaymentReceivedEvent
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

        return Ok("Payment submitted successfully.");
    }
}
