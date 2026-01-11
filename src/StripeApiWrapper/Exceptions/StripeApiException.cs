namespace StripeApiWrapper.Exceptions;

/// <summary>
/// Base exception for Stripe API errors.
/// </summary>
public class StripeApiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StripeApiException"/> class.
    /// </summary>
    public StripeApiException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StripeApiException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public StripeApiException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StripeApiException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public StripeApiException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StripeApiException"/> class with full details.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The Stripe error code.</param>
    /// <param name="errorType">The Stripe error type.</param>
    /// <param name="requestId">The Stripe request ID.</param>
    /// <param name="innerException">The inner exception.</param>
    public StripeApiException(
        string message,
        string? errorCode,
        string? errorType,
        string? requestId,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        ErrorType = errorType;
        RequestId = requestId;
    }

    /// <summary>
    /// Gets the Stripe error code.
    /// </summary>
    public string? ErrorCode { get; }

    /// <summary>
    /// Gets the Stripe error type.
    /// </summary>
    public string? ErrorType { get; }

    /// <summary>
    /// Gets the Stripe request ID for debugging.
    /// </summary>
    public string? RequestId { get; }

    /// <summary>
    /// Gets a value indicating whether this is a critical error that should not be retried.
    /// </summary>
    public virtual bool IsCritical => ErrorType == "authentication_error" || ErrorType == "api_error";

    /// <summary>
    /// Gets the suggested retry delay in seconds, or null if retry is not recommended.
    /// </summary>
    public virtual int? RetryAfterSeconds => IsCritical ? null : 1;
}
