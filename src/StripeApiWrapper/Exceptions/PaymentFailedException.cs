namespace StripeApiWrapper.Exceptions;

/// <summary>
/// Exception thrown when a payment fails due to card issues.
/// </summary>
public class PaymentFailedException : StripeApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentFailedException"/> class.
    /// </summary>
    public PaymentFailedException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentFailedException"/> class with a message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public PaymentFailedException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentFailedException"/> class with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    public PaymentFailedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentFailedException"/> class with full details.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="declineCode">The card decline code.</param>
    /// <param name="userMessage">The user-friendly error message.</param>
    /// <param name="paymentIntentId">The payment intent ID.</param>
    /// <param name="innerException">The inner exception.</param>
    public PaymentFailedException(
        string message,
        string? declineCode,
        string? userMessage,
        string? paymentIntentId,
        Exception? innerException = null)
        : base(message, declineCode, "card_error", null, innerException)
    {
        DeclineCode = declineCode;
        UserMessage = userMessage ?? GetDefaultUserMessage(declineCode);
        PaymentIntentId = paymentIntentId;
    }

    /// <summary>
    /// Gets the card decline code.
    /// </summary>
    public string? DeclineCode { get; }

    /// <summary>
    /// Gets a user-friendly error message suitable for display.
    /// </summary>
    public string UserMessage { get; } = "Your payment could not be processed. Please try again.";

    /// <summary>
    /// Gets the payment intent ID associated with this error.
    /// </summary>
    public string? PaymentIntentId { get; }

    /// <summary>
    /// Gets a value indicating whether the payment can be retried with the same card.
    /// </summary>
    public bool CanRetry => DeclineCode switch
    {
        "insufficient_funds" => true,
        "processing_error" => true,
        "try_again_later" => true,
        "expired_card" => false,
        "incorrect_cvc" => true,
        "card_declined" => true,
        "stolen_card" => false,
        "lost_card" => false,
        _ => true
    };

    /// <inheritdoc/>
    public override bool IsCritical => false;

    private static string GetDefaultUserMessage(string? declineCode)
    {
        return declineCode switch
        {
            "insufficient_funds" => "Your card has insufficient funds. Please use a different payment method.",
            "expired_card" => "Your card has expired. Please use a different card.",
            "incorrect_cvc" => "The security code (CVC) is incorrect. Please check and try again.",
            "processing_error" => "An error occurred while processing your card. Please try again.",
            "card_declined" => "Your card was declined. Please contact your bank or use a different card.",
            "stolen_card" or "lost_card" => "This card cannot be used. Please use a different payment method.",
            "invalid_card_number" => "The card number is invalid. Please check and try again.",
            "card_not_supported" => "This card type is not supported. Please use a different card.",
            _ => "Your payment could not be processed. Please try again or use a different payment method."
        };
    }
}
