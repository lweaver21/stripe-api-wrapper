namespace StripeApiWrapper.Models;

/// <summary>
/// Result model for payment operations.
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// Gets or sets the Stripe payment intent ID.
    /// </summary>
    public string PaymentIntentId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client secret for client-side confirmation.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    /// Gets or sets the payment status.
    /// </summary>
    public PaymentStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the amount in the smallest currency unit.
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether the payment requires additional action.
    /// </summary>
    public bool RequiresAction { get; set; }

    /// <summary>
    /// Gets or sets the URL for additional action (e.g., 3D Secure).
    /// </summary>
    public string? ActionUrl { get; set; }

    /// <summary>
    /// Gets or sets the charge ID if the payment was successful.
    /// </summary>
    public string? ChargeId { get; set; }

    /// <summary>
    /// Gets or sets the error message if the payment failed.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Gets or sets the error code if the payment failed.
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the payment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether the payment was successful.
    /// </summary>
    public bool IsSuccessful => Status == PaymentStatus.Succeeded;
}

/// <summary>
/// Represents the status of a payment.
/// </summary>
public enum PaymentStatus
{
    /// <summary>Payment requires a payment method.</summary>
    RequiresPaymentMethod,

    /// <summary>Payment requires confirmation.</summary>
    RequiresConfirmation,

    /// <summary>Payment requires additional action (e.g., 3D Secure).</summary>
    RequiresAction,

    /// <summary>Payment is processing.</summary>
    Processing,

    /// <summary>Payment requires capture.</summary>
    RequiresCapture,

    /// <summary>Payment was canceled.</summary>
    Canceled,

    /// <summary>Payment succeeded.</summary>
    Succeeded,

    /// <summary>Payment failed.</summary>
    Failed
}
