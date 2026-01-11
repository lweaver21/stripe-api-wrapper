using Microsoft.Extensions.Options;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Implementation of the payment service.
/// </summary>
public class PaymentService : IPaymentService
{
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;
    private readonly StripeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaymentService"/> class.
    /// </summary>
    /// <param name="stripeClient">The Stripe client.</param>
    /// <param name="options">The Stripe options.</param>
    public PaymentService(IStripeClient stripeClient, IOptions<StripeOptions> options)
    {
        ArgumentNullException.ThrowIfNull(stripeClient);
        ArgumentNullException.ThrowIfNull(options);

        _paymentIntentService = new PaymentIntentService(stripeClient);
        _refundService = new RefundService(stripeClient);
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<PaymentResult> CreatePaymentAsync(PaymentRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = request.Amount,
                Currency = request.Currency.ToLowerInvariant(),
                Customer = request.CustomerId,
                PaymentMethod = request.PaymentMethodId,
                Description = request.Description,
                ReceiptEmail = request.ReceiptEmail,
                Metadata = request.Metadata,
                CaptureMethod = request.CaptureImmediately ? "automatic" : "manual",
                Confirm = request.PaymentMethodId != null
            };

            var requestOptions = CreateRequestOptions(request.IdempotencyKey);
            var paymentIntent = await _paymentIntentService.CreateAsync(options, requestOptions, cancellationToken);

            return MapToPaymentResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<PaymentResult> ConfirmPaymentAsync(string paymentIntentId, string? paymentMethodId = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentIntentId);

        try
        {
            var options = new PaymentIntentConfirmOptions
            {
                PaymentMethod = paymentMethodId
            };

            var paymentIntent = await _paymentIntentService.ConfirmAsync(paymentIntentId, options, cancellationToken: cancellationToken);

            return MapToPaymentResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<PaymentResult> CapturePaymentAsync(string paymentIntentId, long? amountToCapture = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentIntentId);

        try
        {
            var options = new PaymentIntentCaptureOptions
            {
                AmountToCapture = amountToCapture
            };

            var paymentIntent = await _paymentIntentService.CaptureAsync(paymentIntentId, options, cancellationToken: cancellationToken);

            return MapToPaymentResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<string> RefundPaymentAsync(string paymentIntentId, long? amount = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentIntentId);

        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
                Amount = amount,
                Reason = reason
            };

            var refund = await _refundService.CreateAsync(options, cancellationToken: cancellationToken);

            return refund.Id;
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<PaymentResult> CancelPaymentAsync(string paymentIntentId, string? cancellationReason = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentIntentId);

        try
        {
            var options = new PaymentIntentCancelOptions
            {
                CancellationReason = cancellationReason
            };

            var paymentIntent = await _paymentIntentService.CancelAsync(paymentIntentId, options, cancellationToken: cancellationToken);

            return MapToPaymentResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<PaymentResult> GetPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentIntentId);

        try
        {
            var paymentIntent = await _paymentIntentService.GetAsync(paymentIntentId, cancellationToken: cancellationToken);

            return MapToPaymentResult(paymentIntent);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    private static RequestOptions? CreateRequestOptions(string? idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return null;
        }

        return new RequestOptions
        {
            IdempotencyKey = idempotencyKey
        };
    }

    private static PaymentResult MapToPaymentResult(PaymentIntent paymentIntent)
    {
        return new PaymentResult
        {
            PaymentIntentId = paymentIntent.Id,
            ClientSecret = paymentIntent.ClientSecret,
            Status = MapStatus(paymentIntent.Status),
            Amount = paymentIntent.Amount,
            Currency = paymentIntent.Currency,
            RequiresAction = paymentIntent.Status == "requires_action",
            ActionUrl = paymentIntent.NextAction?.RedirectToUrl?.Url,
            ChargeId = paymentIntent.LatestChargeId,
            CreatedAt = paymentIntent.Created,
            ErrorMessage = paymentIntent.LastPaymentError?.Message,
            ErrorCode = paymentIntent.LastPaymentError?.Code
        };
    }

    private static PaymentStatus MapStatus(string status)
    {
        return status switch
        {
            "requires_payment_method" => PaymentStatus.RequiresPaymentMethod,
            "requires_confirmation" => PaymentStatus.RequiresConfirmation,
            "requires_action" => PaymentStatus.RequiresAction,
            "processing" => PaymentStatus.Processing,
            "requires_capture" => PaymentStatus.RequiresCapture,
            "canceled" => PaymentStatus.Canceled,
            "succeeded" => PaymentStatus.Succeeded,
            _ => PaymentStatus.Failed
        };
    }

    private static Exception MapStripeException(StripeException ex)
    {
        if (ex.StripeError?.Type == "card_error")
        {
            return new PaymentFailedException(
                ex.Message,
                ex.StripeError.DeclineCode ?? ex.StripeError.Code,
                ex.StripeError.Message,
                ex.StripeError.PaymentIntent?.Id,
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
