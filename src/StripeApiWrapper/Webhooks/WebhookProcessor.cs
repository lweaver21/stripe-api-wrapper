using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;

namespace StripeApiWrapper.Webhooks;

/// <summary>
/// Processes Stripe webhook events.
/// </summary>
public class WebhookProcessor
{
    private readonly IServiceProvider _serviceProvider;
    private readonly StripeOptions _options;
    private readonly ILogger<WebhookProcessor>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebhookProcessor"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <param name="options">The Stripe options.</param>
    /// <param name="logger">The logger.</param>
    public WebhookProcessor(
        IServiceProvider serviceProvider,
        IOptions<StripeOptions> options,
        ILogger<WebhookProcessor>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(options);

        _serviceProvider = serviceProvider;
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>
    /// Validates and parses a webhook payload.
    /// </summary>
    /// <param name="payload">The raw request body.</param>
    /// <param name="signature">The Stripe-Signature header value.</param>
    /// <returns>The parsed Stripe event.</returns>
    /// <exception cref="StripeApiException">Thrown when signature validation fails.</exception>
    public Event ValidateAndParseWebhook(string payload, string signature)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(payload);
        ArgumentException.ThrowIfNullOrWhiteSpace(signature);

        if (string.IsNullOrWhiteSpace(_options.WebhookSecret))
        {
            throw new StripeApiException(
                "Webhook secret is not configured.",
                "webhook_secret_missing",
                "configuration_error",
                null);
        }

        try
        {
            var stripeEvent = EventUtility.ConstructEvent(
                payload,
                signature,
                _options.WebhookSecret);

            _logger?.LogInformation(
                "Received webhook event {EventType} with ID {EventId}",
                stripeEvent.Type,
                stripeEvent.Id);

            return stripeEvent;
        }
        catch (StripeException ex)
        {
            _logger?.LogWarning(ex, "Webhook signature validation failed");

            throw new StripeApiException(
                "Webhook signature validation failed.",
                "invalid_signature",
                "webhook_error",
                null,
                ex);
        }
    }

    /// <summary>
    /// Processes a webhook event by routing it to the appropriate handler.
    /// </summary>
    /// <param name="stripeEvent">The Stripe event.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the event was handled, false otherwise.</returns>
    public async Task<bool> ProcessEventAsync(Event stripeEvent, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stripeEvent);

        using var scope = _serviceProvider.CreateScope();
        var handlers = scope.ServiceProvider.GetServices<IWebhookHandler>();

        var matchingHandlers = handlers
            .Where(h => h.EventType == stripeEvent.Type || h.EventType == "*")
            .ToList();

        if (matchingHandlers.Count == 0)
        {
            _logger?.LogDebug(
                "No handler registered for event type {EventType}",
                stripeEvent.Type);
            return false;
        }

        foreach (var handler in matchingHandlers)
        {
            try
            {
                _logger?.LogDebug(
                    "Invoking handler {HandlerType} for event {EventType}",
                    handler.GetType().Name,
                    stripeEvent.Type);

                await handler.HandleAsync(stripeEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger?.LogError(
                    ex,
                    "Handler {HandlerType} failed for event {EventId}",
                    handler.GetType().Name,
                    stripeEvent.Id);
                throw;
            }
        }

        return true;
    }

    /// <summary>
    /// Validates, parses, and processes a webhook in one operation.
    /// </summary>
    /// <param name="payload">The raw request body.</param>
    /// <param name="signature">The Stripe-Signature header value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the event was handled, false otherwise.</returns>
    public async Task<bool> ProcessWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default)
    {
        var stripeEvent = ValidateAndParseWebhook(payload, signature);
        return await ProcessEventAsync(stripeEvent, cancellationToken);
    }
}
