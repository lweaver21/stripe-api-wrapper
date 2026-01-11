namespace StripeApiWrapper.Exceptions;

/// <summary>
/// Exception thrown when a subscription operation fails.
/// </summary>
public class SubscriptionException : StripeApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class.
    /// </summary>
    public SubscriptionException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public SubscriptionException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public SubscriptionException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionException"/> class with full details.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="reason">The reason for the failure.</param>
    /// <param name="innerException">The inner exception.</param>
    public SubscriptionException(
        string message,
        string? subscriptionId,
        SubscriptionErrorReason reason,
        Exception? innerException = null)
        : base(message, reason.ToString(), "subscription_error", null, innerException)
    {
        SubscriptionId = subscriptionId;
        Reason = reason;
    }

    /// <summary>
    /// Gets the subscription ID associated with this error.
    /// </summary>
    public string? SubscriptionId { get; }

    /// <summary>
    /// Gets the reason for the subscription error.
    /// </summary>
    public SubscriptionErrorReason Reason { get; }

    /// <inheritdoc/>
    public override bool IsCritical => Reason == SubscriptionErrorReason.InvalidConfiguration;

    /// <summary>
    /// Gets a user-friendly message for the error.
    /// </summary>
    public string UserMessage => Reason switch
    {
        SubscriptionErrorReason.CustomerNotFound => "The customer account was not found.",
        SubscriptionErrorReason.PaymentFailed => "Payment for the subscription failed.",
        SubscriptionErrorReason.InvalidPrice => "The selected plan is not available.",
        SubscriptionErrorReason.AlreadyCanceled => "This subscription has already been canceled.",
        SubscriptionErrorReason.InvalidConfiguration => "There was a configuration error. Please contact support.",
        SubscriptionErrorReason.TrialExpired => "The trial period has expired.",
        SubscriptionErrorReason.PlanNotFound => "The selected subscription plan was not found.",
        _ => "An error occurred with your subscription."
    };
}

/// <summary>
/// Reasons for subscription errors.
/// </summary>
public enum SubscriptionErrorReason
{
    /// <summary>Unknown error.</summary>
    Unknown,

    /// <summary>Customer was not found.</summary>
    CustomerNotFound,

    /// <summary>Payment for the subscription failed.</summary>
    PaymentFailed,

    /// <summary>The price/plan is invalid.</summary>
    InvalidPrice,

    /// <summary>Subscription is already canceled.</summary>
    AlreadyCanceled,

    /// <summary>Invalid configuration.</summary>
    InvalidConfiguration,

    /// <summary>Trial period has expired.</summary>
    TrialExpired,

    /// <summary>Plan was not found.</summary>
    PlanNotFound
}
