using Microsoft.Extensions.Options;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Implementation of the subscription service.
/// </summary>
public class SubscriptionService : ISubscriptionService
{
    private readonly Stripe.SubscriptionService _subscriptionService;
    private readonly StripeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubscriptionService"/> class.
    /// </summary>
    /// <param name="stripeClient">The Stripe client.</param>
    /// <param name="options">The Stripe options.</param>
    public SubscriptionService(IStripeClient stripeClient, IOptions<StripeOptions> options)
    {
        ArgumentNullException.ThrowIfNull(stripeClient);
        ArgumentNullException.ThrowIfNull(options);

        _subscriptionService = new Stripe.SubscriptionService(stripeClient);
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<SubscriptionDto> CreateSubscriptionAsync(CreateSubscriptionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = request.CustomerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = request.PriceId,
                        Quantity = request.Quantity
                    }
                },
                DefaultPaymentMethod = request.PaymentMethodId,
                Metadata = request.Metadata
            };

            if (request.TrialDays.HasValue && request.TrialDays.Value > 0)
            {
                options.TrialPeriodDays = request.TrialDays.Value;
            }

            var subscription = await _subscriptionService.CreateAsync(options, cancellationToken: cancellationToken);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionDto> UpdateSubscriptionAsync(string subscriptionId, string? priceId = null, int? quantity = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);

        try
        {
            // First get the current subscription to find the item ID
            var currentSubscription = await _subscriptionService.GetAsync(subscriptionId, cancellationToken: cancellationToken);

            var options = new SubscriptionUpdateOptions();

            if (priceId != null || quantity.HasValue)
            {
                var firstItem = currentSubscription.Items?.Data?.FirstOrDefault();
                if (firstItem != null)
                {
                    options.Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Id = firstItem.Id,
                            Price = priceId ?? firstItem.Price?.Id,
                            Quantity = quantity ?? firstItem.Quantity
                        }
                    };
                }
            }

            var subscription = await _subscriptionService.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, subscriptionId);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionDto> CancelSubscriptionAsync(string subscriptionId, bool cancelAtPeriodEnd = true, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);

        try
        {
            Subscription subscription;

            if (cancelAtPeriodEnd)
            {
                var options = new SubscriptionUpdateOptions
                {
                    CancelAtPeriodEnd = true
                };
                subscription = await _subscriptionService.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);
            }
            else
            {
                subscription = await _subscriptionService.CancelAsync(subscriptionId, cancellationToken: cancellationToken);
            }

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, subscriptionId);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionDto> ResumeSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);

        try
        {
            var options = new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false
            };

            var subscription = await _subscriptionService.UpdateAsync(subscriptionId, options, cancellationToken: cancellationToken);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, subscriptionId);
        }
    }

    /// <inheritdoc/>
    public async Task<SubscriptionDto> GetSubscriptionAsync(string subscriptionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);

        try
        {
            var subscription = await _subscriptionService.GetAsync(subscriptionId, cancellationToken: cancellationToken);

            return MapToSubscriptionDto(subscription);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, subscriptionId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<SubscriptionDto>> ListSubscriptionsAsync(string customerId, string? status = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        try
        {
            var options = new SubscriptionListOptions
            {
                Customer = customerId,
                Status = status
            };

            var subscriptions = await _subscriptionService.ListAsync(options, cancellationToken: cancellationToken);

            return subscriptions.Data.Select(MapToSubscriptionDto).ToList();
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    private static SubscriptionDto MapToSubscriptionDto(Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            CustomerId = subscription.CustomerId,
            Status = MapStatus(subscription.Status),
            Items = subscription.Items?.Data?.Select(item => new SubscriptionItemDto
            {
                Id = item.Id,
                PriceId = item.Price?.Id ?? string.Empty,
                Quantity = (int)item.Quantity
            }).ToList() ?? new List<SubscriptionItemDto>(),
            TrialEnd = subscription.TrialEnd,
            CanceledAt = subscription.CanceledAt,
            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
            DefaultPaymentMethodId = subscription.DefaultPaymentMethodId,
            CollectionMethod = subscription.CollectionMethod ?? "charge_automatically",
            Metadata = subscription.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            CreatedAt = subscription.Created
        };
    }

    private static SubscriptionStatus MapStatus(string status)
    {
        return status switch
        {
            "incomplete" => SubscriptionStatus.Incomplete,
            "incomplete_expired" => SubscriptionStatus.IncompleteExpired,
            "trialing" => SubscriptionStatus.Trialing,
            "active" => SubscriptionStatus.Active,
            "past_due" => SubscriptionStatus.PastDue,
            "canceled" => SubscriptionStatus.Canceled,
            "unpaid" => SubscriptionStatus.Unpaid,
            "paused" => SubscriptionStatus.Paused,
            _ => SubscriptionStatus.Incomplete
        };
    }

    private static Exception MapStripeException(StripeException ex, string? subscriptionId = null)
    {
        if (ex.StripeError?.Code == "resource_missing")
        {
            return new SubscriptionException(
                ex.Message,
                subscriptionId,
                SubscriptionErrorReason.PlanNotFound,
                ex);
        }

        if (ex.StripeError?.Type == "card_error")
        {
            return new SubscriptionException(
                ex.Message,
                subscriptionId,
                SubscriptionErrorReason.PaymentFailed,
                ex);
        }

        return new StripeApiException(
            ex.Message,
            ex.StripeError?.Code,
            ex.StripeError?.Type,
            null,
            ex);
    }
}
