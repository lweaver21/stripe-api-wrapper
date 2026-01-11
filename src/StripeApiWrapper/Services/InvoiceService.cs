using Microsoft.Extensions.Options;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Internal;
using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Implementation of the invoice service.
/// </summary>
public class InvoiceService : IInvoiceService
{
    private readonly Stripe.InvoiceService _invoiceService;
    private readonly StripeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="InvoiceService"/> class.
    /// </summary>
    /// <param name="stripeClient">The Stripe client.</param>
    /// <param name="options">The Stripe options.</param>
    public InvoiceService(IStripeClient stripeClient, IOptions<StripeOptions> options)
    {
        ThrowHelper.ThrowIfNull(stripeClient);
        ThrowHelper.ThrowIfNull(options);

        _invoiceService = new Stripe.InvoiceService(stripeClient);
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNull(request);

        try
        {
            var options = new InvoiceCreateOptions
            {
                Customer = request.CustomerId,
                Description = request.Description,
                CollectionMethod = request.CollectionMethod,
                DaysUntilDue = request.DaysUntilDue,
                Metadata = request.Metadata,
                AutoAdvance = request.AutoAdvance
            };

            var invoice = await _invoiceService.CreateAsync(options, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<InvoiceDto> FinalizeInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(invoiceId);

        try
        {
            var invoice = await _invoiceService.FinalizeInvoiceAsync(invoiceId, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, invoiceId);
        }
    }

    /// <inheritdoc/>
    public async Task<InvoiceDto> PayInvoiceAsync(string invoiceId, string? paymentMethodId = null, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(invoiceId);

        try
        {
            var options = new InvoicePayOptions
            {
                PaymentMethod = paymentMethodId
            };

            var invoice = await _invoiceService.PayAsync(invoiceId, options, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, invoiceId);
        }
    }

    /// <inheritdoc/>
    public async Task<InvoiceDto> VoidInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(invoiceId);

        try
        {
            var invoice = await _invoiceService.VoidInvoiceAsync(invoiceId, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, invoiceId);
        }
    }

    /// <inheritdoc/>
    public async Task<InvoiceDto> GetInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(invoiceId);

        try
        {
            var invoice = await _invoiceService.GetAsync(invoiceId, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, invoiceId);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<InvoiceDto>> ListInvoicesAsync(string customerId, string? status = null, int limit = 10, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(customerId);

        try
        {
            var options = new InvoiceListOptions
            {
                Customer = customerId,
                Status = status,
                Limit = limit
            };

            var invoices = await _invoiceService.ListAsync(options, cancellationToken: cancellationToken);

            return invoices.Data.Select(MapToInvoiceDto).ToList();
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<InvoiceDto> SendInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default)
    {
        ThrowHelper.ThrowIfNullOrWhiteSpace(invoiceId);

        try
        {
            var invoice = await _invoiceService.SendInvoiceAsync(invoiceId, cancellationToken: cancellationToken);

            return MapToInvoiceDto(invoice);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, invoiceId);
        }
    }

    private static InvoiceDto MapToInvoiceDto(Invoice invoice)
    {
        return new InvoiceDto
        {
            Id = invoice.Id,
            Number = invoice.Number,
            CustomerId = invoice.CustomerId,
            Status = MapStatus(invoice.Status),
            AmountDue = invoice.AmountDue,
            AmountPaid = invoice.AmountPaid,
            AmountRemaining = invoice.AmountRemaining,
            Subtotal = invoice.Subtotal,
            Total = invoice.Total,
            Currency = invoice.Currency,
            CollectionMethod = invoice.CollectionMethod,
            DueDate = invoice.DueDate,
            LineItems = invoice.Lines?.Data?.Select(line => new InvoiceLineItemDto
            {
                Id = line.Id,
                Description = line.Description,
                Quantity = (int)(line.Quantity ?? 1),
                UnitAmount = line.Amount / Math.Max((int)(line.Quantity ?? 1), 1),
                Amount = line.Amount,
                Currency = line.Currency
            }).ToList() ?? new List<InvoiceLineItemDto>(),
            HostedInvoiceUrl = invoice.HostedInvoiceUrl,
            InvoicePdfUrl = invoice.InvoicePdf,
            Metadata = invoice.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            CreatedAt = invoice.Created,
            PaidAt = invoice.StatusTransitions?.PaidAt
        };
    }

    private static InvoiceStatus MapStatus(string? status)
    {
        return status switch
        {
            "draft" => InvoiceStatus.Draft,
            "open" => InvoiceStatus.Open,
            "paid" => InvoiceStatus.Paid,
            "uncollectible" => InvoiceStatus.Uncollectible,
            "void" => InvoiceStatus.Void,
            _ => InvoiceStatus.Draft
        };
    }

    private static Exception MapStripeException(StripeException ex, string? invoiceId = null)
    {
        if (ex.StripeError?.Code == "resource_missing")
        {
            return new StripeApiException(
                $"Invoice '{invoiceId}' was not found.",
                ex.StripeError.Code,
                ex.StripeError.Type,
                null,
                ex);
        }

        if (ex.StripeError?.Type == "card_error")
        {
            return new PaymentFailedException(
                ex.Message,
                ex.StripeError.DeclineCode ?? ex.StripeError.Code,
                ex.StripeError.Message,
                null,
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
