namespace StripeApiWrapper.Configuration;

/// <summary>
/// Configuration options for Stripe API integration.
/// </summary>
public class StripeOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Stripe";

    /// <summary>
    /// Gets or sets the Stripe secret API key.
    /// </summary>
    /// <remarks>
    /// Use test keys (sk_test_*) in development and live keys (sk_live_*) in production.
    /// </remarks>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Stripe webhook signing secret.
    /// </summary>
    /// <remarks>
    /// Used to verify webhook signatures. Found in the Stripe Dashboard under Webhooks.
    /// </remarks>
    public string WebhookSecret { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Stripe publishable API key.
    /// </summary>
    /// <remarks>
    /// Safe to expose in client-side code. Used for Stripe.js and Elements.
    /// </remarks>
    public string? PublishableKey { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Stripe test mode.
    /// </summary>
    /// <remarks>
    /// When true, enables additional logging and validation for development.
    /// </remarks>
    public bool EnableTestMode { get; set; }

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for failed API calls.
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when required options are missing.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(SecretKey))
        {
            throw new InvalidOperationException("Stripe SecretKey is required.");
        }
    }
}
