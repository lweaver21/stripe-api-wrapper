namespace StripeApiWrapper.Exceptions;

/// <summary>
/// Exception thrown when a customer is not found.
/// </summary>
public class CustomerNotFoundException : StripeApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerNotFoundException"/> class.
    /// </summary>
    public CustomerNotFoundException()
        : base("Customer not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerNotFoundException"/> class with a customer ID.
    /// </summary>
    /// <param name="customerId">The customer ID that was not found.</param>
    public CustomerNotFoundException(string customerId)
        : base($"Customer with ID '{customerId}' was not found.")
    {
        CustomerId = customerId;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerNotFoundException"/> class with a customer ID and inner exception.
    /// </summary>
    /// <param name="customerId">The customer ID that was not found.</param>
    /// <param name="innerException">The inner exception.</param>
    public CustomerNotFoundException(string customerId, Exception innerException)
        : base($"Customer with ID '{customerId}' was not found.", innerException)
    {
        CustomerId = customerId;
    }

    /// <summary>
    /// Gets the customer ID that was not found.
    /// </summary>
    public string? CustomerId { get; }

    /// <inheritdoc/>
    public override bool IsCritical => false;

    /// <inheritdoc/>
    public override int? RetryAfterSeconds => null;
}
