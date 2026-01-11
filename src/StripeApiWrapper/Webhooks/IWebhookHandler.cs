using Stripe;

namespace StripeApiWrapper.Webhooks;

/// <summary>
/// Interface for handling Stripe webhook events.
/// </summary>
public interface IWebhookHandler
{
    /// <summary>
    /// Gets the event type this handler processes.
    /// </summary>
    string EventType { get; }

    /// <summary>
    /// Handles the webhook event.
    /// </summary>
    /// <param name="stripeEvent">The Stripe event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task HandleAsync(Event stripeEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Base class for webhook handlers with typed event data.
/// </summary>
/// <typeparam name="T">The type of the event data object.</typeparam>
public abstract class WebhookHandler<T> : IWebhookHandler where T : class, IHasId
{
    /// <inheritdoc/>
    public abstract string EventType { get; }

    /// <inheritdoc/>
    public async Task HandleAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        if (stripeEvent.Data.Object is T data)
        {
            await HandleEventAsync(stripeEvent, data, cancellationToken);
        }
    }

    /// <summary>
    /// Handles the webhook event with typed data.
    /// </summary>
    /// <param name="stripeEvent">The Stripe event.</param>
    /// <param name="data">The typed event data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task HandleEventAsync(Event stripeEvent, T data, CancellationToken cancellationToken);
}
