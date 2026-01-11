using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using StripeApiWrapper.Exceptions;

namespace StripeApiWrapper.Webhooks;

/// <summary>
/// Extension methods for configuring webhook endpoints.
/// </summary>
public static class WebhookEndpointExtensions
{
    /// <summary>
    /// Maps a Stripe webhook endpoint.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder.</param>
    /// <param name="pattern">The URL pattern for the webhook endpoint.</param>
    /// <returns>The route handler builder.</returns>
    public static IEndpointConventionBuilder MapStripeWebhook(
        this IEndpointRouteBuilder endpoints,
        string pattern = "/webhooks/stripe")
    {
        return endpoints.MapPost(pattern, async (HttpContext context) =>
        {
            var processor = context.RequestServices.GetRequiredService<WebhookProcessor>();

            // Read the request body
            using var reader = new StreamReader(context.Request.Body);
            var payload = await reader.ReadToEndAsync();

            // Get the signature header
            var signature = context.Request.Headers["Stripe-Signature"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(signature))
            {
                return Results.BadRequest(new { error = "Missing Stripe-Signature header" });
            }

            try
            {
                var handled = await processor.ProcessWebhookAsync(payload, signature, context.RequestAborted);

                return handled
                    ? Results.Ok(new { received = true })
                    : Results.Ok(new { received = true, handled = false });
            }
            catch (StripeApiException ex) when (ex.ErrorCode == "invalid_signature")
            {
                return Results.BadRequest(new { error = "Invalid signature" });
            }
            catch (StripeApiException ex) when (ex.ErrorCode == "webhook_secret_missing")
            {
                return Results.StatusCode(500);
            }
        });
    }

    /// <summary>
    /// Adds Stripe webhook services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddStripeWebhooks(this IServiceCollection services)
    {
        services.AddScoped<WebhookProcessor>();
        return services;
    }

    /// <summary>
    /// Registers a webhook handler.
    /// </summary>
    /// <typeparam name="THandler">The handler type.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddWebhookHandler<THandler>(this IServiceCollection services)
        where THandler : class, IWebhookHandler
    {
        services.AddScoped<IWebhookHandler, THandler>();
        return services;
    }
}
