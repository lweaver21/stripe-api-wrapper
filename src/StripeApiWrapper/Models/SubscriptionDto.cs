using System.ComponentModel.DataAnnotations;

namespace StripeApiWrapper.Models;

/// <summary>
/// Data transfer object for subscription operations.
/// </summary>
public class SubscriptionDto
{
    /// <summary>
    /// Gets or sets the Stripe subscription ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the Stripe customer ID.
    /// </summary>
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the subscription status.
    /// </summary>
    public SubscriptionStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the subscription items (price IDs and quantities).
    /// </summary>
    public List<SubscriptionItemDto> Items { get; set; } = new();

    /// <summary>
    /// Gets or sets the current billing period start date.
    /// </summary>
    public DateTime? CurrentPeriodStart { get; set; }

    /// <summary>
    /// Gets or sets the current billing period end date.
    /// </summary>
    public DateTime? CurrentPeriodEnd { get; set; }

    /// <summary>
    /// Gets or sets the trial end date.
    /// </summary>
    public DateTime? TrialEnd { get; set; }

    /// <summary>
    /// Gets or sets the cancellation date.
    /// </summary>
    public DateTime? CanceledAt { get; set; }

    /// <summary>
    /// Gets or sets when the subscription will be canceled at period end.
    /// </summary>
    public bool CancelAtPeriodEnd { get; set; }

    /// <summary>
    /// Gets or sets the default payment method ID.
    /// </summary>
    public string? DefaultPaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the collection method (charge_automatically or send_invoice).
    /// </summary>
    public string CollectionMethod { get; set; } = "charge_automatically";

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the subscription was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Represents an item in a subscription.
/// </summary>
public class SubscriptionItemDto
{
    /// <summary>
    /// Gets or sets the subscription item ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the Stripe price ID.
    /// </summary>
    [Required]
    public string PriceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}

/// <summary>
/// Request model for creating a subscription.
/// </summary>
public class CreateSubscriptionRequest
{
    /// <summary>
    /// Gets or sets the Stripe customer ID.
    /// </summary>
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price ID for the subscription.
    /// </summary>
    [Required]
    public string PriceId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of trial days.
    /// </summary>
    [Range(0, 730)]
    public int? TrialDays { get; set; }

    /// <summary>
    /// Gets or sets the payment method ID.
    /// </summary>
    public string? PaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Represents the status of a subscription.
/// </summary>
public enum SubscriptionStatus
{
    /// <summary>Subscription is incomplete.</summary>
    Incomplete,

    /// <summary>Subscription expired during incomplete phase.</summary>
    IncompleteExpired,

    /// <summary>Subscription is in trial period.</summary>
    Trialing,

    /// <summary>Subscription is active.</summary>
    Active,

    /// <summary>Payment is past due.</summary>
    PastDue,

    /// <summary>Subscription was canceled.</summary>
    Canceled,

    /// <summary>Subscription is unpaid.</summary>
    Unpaid,

    /// <summary>Subscription is paused.</summary>
    Paused
}
