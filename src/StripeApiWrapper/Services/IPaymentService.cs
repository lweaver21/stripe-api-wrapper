using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Service interface for payment operations.
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Creates a new payment intent.
    /// </summary>
    /// <param name="request">The payment request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment result.</returns>
    Task<PaymentResult> CreatePaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirms a payment intent after additional authentication.
    /// </summary>
    /// <param name="paymentIntentId">The payment intent ID.</param>
    /// <param name="paymentMethodId">Optional payment method ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment result.</returns>
    Task<PaymentResult> ConfirmPaymentAsync(string paymentIntentId, string? paymentMethodId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Captures an authorized payment.
    /// </summary>
    /// <param name="paymentIntentId">The payment intent ID.</param>
    /// <param name="amountToCapture">Optional amount to capture (for partial capture).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment result.</returns>
    Task<PaymentResult> CapturePaymentAsync(string paymentIntentId, long? amountToCapture = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refunds a payment.
    /// </summary>
    /// <param name="paymentIntentId">The payment intent ID.</param>
    /// <param name="amount">Optional amount to refund (for partial refund).</param>
    /// <param name="reason">Optional reason for the refund.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The refund ID.</returns>
    Task<string> RefundPaymentAsync(string paymentIntentId, long? amount = null, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a payment intent.
    /// </summary>
    /// <param name="paymentIntentId">The payment intent ID.</param>
    /// <param name="cancellationReason">Optional reason for cancellation.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment result.</returns>
    Task<PaymentResult> CancelPaymentAsync(string paymentIntentId, string? cancellationReason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a payment intent by ID.
    /// </summary>
    /// <param name="paymentIntentId">The payment intent ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment result.</returns>
    Task<PaymentResult> GetPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
}
