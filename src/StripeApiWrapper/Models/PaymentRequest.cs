using System.ComponentModel.DataAnnotations;

namespace StripeApiWrapper.Models;

/// <summary>
/// Request model for creating a payment.
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// Gets or sets the payment amount in the smallest currency unit (e.g., cents for USD).
    /// </summary>
    [Required]
    [Range(1, long.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public long Amount { get; set; }

    /// <summary>
    /// Gets or sets the three-letter ISO currency code (e.g., "usd", "eur").
    /// </summary>
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Gets or sets the Stripe customer ID to associate with this payment.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the payment method ID to use for this payment.
    /// </summary>
    public string? PaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the description for the payment.
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the email address for the receipt.
    /// </summary>
    [EmailAddress]
    public string? ReceiptEmail { get; set; }

    /// <summary>
    /// Gets or sets additional metadata to attach to the payment.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to capture the payment immediately.
    /// </summary>
    /// <remarks>
    /// When false, the payment is authorized but not captured. Use CapturePayment to capture later.
    /// </remarks>
    public bool CaptureImmediately { get; set; } = true;

    /// <summary>
    /// Gets or sets an idempotency key to prevent duplicate payments.
    /// </summary>
    public string? IdempotencyKey { get; set; }
}
