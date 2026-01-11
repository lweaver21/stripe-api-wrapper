using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Service interface for subscription operations.
/// </summary>
public interface ISubscriptionService
{
    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    /// <param name="request">The subscription request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created subscription.</returns>
    Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="priceId">The new price ID.</param>
    /// <param name="quantity">The new quantity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated subscription.</returns>
    Task<SubscriptionDto> UpdateSubscriptionAsync(string subscriptionId, string? priceId = null, int? quantity = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="cancelAtPeriodEnd">Whether to cancel at the end of the current period.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The canceled subscription.</returns>
    Task<SubscriptionDto> CancelSubscriptionAsync(string subscriptionId, bool cancelAtPeriodEnd = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resumes a paused or canceled subscription.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The resumed subscription.</returns>
    Task<SubscriptionDto> ResumeSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a subscription by ID.
    /// </summary>
    /// <param name="subscriptionId">The subscription ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The subscription.</returns>
    Task<SubscriptionDto> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists subscriptions for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="status">Filter by status (optional).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of subscriptions.</returns>
    Task<IReadOnlyList<SubscriptionDto>> ListSubscriptionsAsync(string customerId, string? status = null, CancellationToken cancellationToken = default);
}
