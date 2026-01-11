using Microsoft.AspNetCore.Mvc;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;
using StripeApiWrapper.Services;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentService paymentService, ILogger<PaymentsController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new payment intent.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.CreatePaymentAsync(request, cancellationToken);
            return Ok(result);
        }
        catch (PaymentFailedException ex)
        {
            _logger.LogWarning(ex, "Payment failed: {DeclineCode}", ex.DeclineCode);
            return BadRequest(new
            {
                error = ex.UserMessage,
                code = ex.DeclineCode,
                canRetry = ex.CanRetry
            });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Stripe API error");
            return StatusCode(502, new { error = "Payment service error" });
        }
    }

    /// <summary>
    /// Retrieves a payment by ID.
    /// </summary>
    [HttpGet("{paymentIntentId}")]
    public async Task<IActionResult> GetPayment(string paymentIntentId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.GetPaymentAsync(paymentIntentId, cancellationToken);
            return Ok(result);
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to retrieve payment {PaymentIntentId}", paymentIntentId);
            return NotFound(new { error = "Payment not found" });
        }
    }

    /// <summary>
    /// Confirms a payment after 3D Secure authentication.
    /// </summary>
    [HttpPost("{paymentIntentId}/confirm")]
    public async Task<IActionResult> ConfirmPayment(string paymentIntentId, [FromBody] ConfirmPaymentRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.ConfirmPaymentAsync(paymentIntentId, request?.PaymentMethodId, cancellationToken);
            return Ok(result);
        }
        catch (PaymentFailedException ex)
        {
            return BadRequest(new
            {
                error = ex.UserMessage,
                code = ex.DeclineCode,
                canRetry = ex.CanRetry
            });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to confirm payment {PaymentIntentId}", paymentIntentId);
            return StatusCode(502, new { error = "Payment service error" });
        }
    }

    /// <summary>
    /// Captures an authorized payment.
    /// </summary>
    [HttpPost("{paymentIntentId}/capture")]
    public async Task<IActionResult> CapturePayment(string paymentIntentId, [FromBody] CapturePaymentRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.CapturePaymentAsync(paymentIntentId, request?.Amount, cancellationToken);
            return Ok(result);
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to capture payment {PaymentIntentId}", paymentIntentId);
            return StatusCode(502, new { error = "Payment service error" });
        }
    }

    /// <summary>
    /// Refunds a payment.
    /// </summary>
    [HttpPost("{paymentIntentId}/refund")]
    public async Task<IActionResult> RefundPayment(string paymentIntentId, [FromBody] RefundPaymentRequest? request, CancellationToken cancellationToken)
    {
        try
        {
            var refundId = await _paymentService.RefundPaymentAsync(paymentIntentId, request?.Amount, request?.Reason, cancellationToken);
            return Ok(new { refundId });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to refund payment {PaymentIntentId}", paymentIntentId);
            return StatusCode(502, new { error = "Refund service error" });
        }
    }

    /// <summary>
    /// Cancels a payment.
    /// </summary>
    [HttpPost("{paymentIntentId}/cancel")]
    public async Task<IActionResult> CancelPayment(string paymentIntentId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _paymentService.CancelPaymentAsync(paymentIntentId, null, cancellationToken);
            return Ok(result);
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to cancel payment {PaymentIntentId}", paymentIntentId);
            return StatusCode(502, new { error = "Payment service error" });
        }
    }
}

public record ConfirmPaymentRequest(string? PaymentMethodId);
public record CapturePaymentRequest(long? Amount);
public record RefundPaymentRequest(long? Amount, string? Reason);
