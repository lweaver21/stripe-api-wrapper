using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Service interface for customer operations.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Creates a new customer.
    /// </summary>
    /// <param name="customer">The customer data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created customer.</returns>
    Task<CustomerDto> CreateCustomerAsync(CustomerDto customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="customer">The updated customer data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated customer.</returns>
    Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a customer by ID.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The customer.</returns>
    Task<CustomerDto> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if deleted successfully.</returns>
    Task<bool> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attaches a payment method to a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="paymentMethodId">The payment method ID.</param>
    /// <param name="setAsDefault">Whether to set as the default payment method.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The payment method ID.</returns>
    Task<string> AttachPaymentMethodAsync(string customerId, string paymentMethodId, bool setAsDefault = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Detaches a payment method from a customer.
    /// </summary>
    /// <param name="paymentMethodId">The payment method ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if detached successfully.</returns>
    Task<bool> DetachPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists payment methods for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="type">The payment method type (e.g., "card").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of payment method IDs.</returns>
    Task<IReadOnlyList<string>> ListPaymentMethodsAsync(string customerId, string type = "card", CancellationToken cancellationToken = default);
}
