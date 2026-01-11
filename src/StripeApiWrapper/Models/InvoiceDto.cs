using System.ComponentModel.DataAnnotations;

namespace StripeApiWrapper.Models;

/// <summary>
/// Data transfer object for invoice operations.
/// </summary>
public class InvoiceDto
{
    /// <summary>
    /// Gets or sets the Stripe invoice ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the invoice number.
    /// </summary>
    public string? Number { get; set; }

    /// <summary>
    /// Gets or sets the Stripe customer ID.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Gets or sets the Stripe subscription ID.
    /// </summary>
    public string? SubscriptionId { get; set; }

    /// <summary>
    /// Gets or sets the invoice status.
    /// </summary>
    public InvoiceStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the total amount in smallest currency unit.
    /// </summary>
    public long AmountDue { get; set; }

    /// <summary>
    /// Gets or sets the amount paid.
    /// </summary>
    public long AmountPaid { get; set; }

    /// <summary>
    /// Gets or sets the amount remaining.
    /// </summary>
    public long AmountRemaining { get; set; }

    /// <summary>
    /// Gets or sets the subtotal before tax.
    /// </summary>
    public long Subtotal { get; set; }

    /// <summary>
    /// Gets or sets the total tax amount.
    /// </summary>
    public long Tax { get; set; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    public long Total { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    public string Currency { get; set; } = "usd";

    /// <summary>
    /// Gets or sets the collection method.
    /// </summary>
    public string? CollectionMethod { get; set; }

    /// <summary>
    /// Gets or sets the due date.
    /// </summary>
    public DateTime? DueDate { get; set; }

    /// <summary>
    /// Gets or sets the invoice line items.
    /// </summary>
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();

    /// <summary>
    /// Gets or sets the hosted invoice URL.
    /// </summary>
    public string? HostedInvoiceUrl { get; set; }

    /// <summary>
    /// Gets or sets the invoice PDF URL.
    /// </summary>
    public string? InvoicePdfUrl { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the invoice was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the invoice was paid.
    /// </summary>
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// Represents a line item on an invoice.
/// </summary>
public class InvoiceLineItemDto
{
    /// <summary>
    /// Gets or sets the line item ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit amount.
    /// </summary>
    public long UnitAmount { get; set; }

    /// <summary>
    /// Gets or sets the total amount.
    /// </summary>
    public long Amount { get; set; }

    /// <summary>
    /// Gets or sets the currency code.
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the price ID.
    /// </summary>
    public string? PriceId { get; set; }
}

/// <summary>
/// Request model for creating an invoice.
/// </summary>
public class CreateInvoiceRequest
{
    /// <summary>
    /// Gets or sets the Stripe customer ID.
    /// </summary>
    [Required]
    public string CustomerId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the collection method.
    /// </summary>
    public string CollectionMethod { get; set; } = "charge_automatically";

    /// <summary>
    /// Gets or sets the number of days until the invoice is due.
    /// </summary>
    [Range(1, 365)]
    public int? DaysUntilDue { get; set; }

    /// <summary>
    /// Gets or sets additional metadata.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to auto-advance the invoice.
    /// </summary>
    public bool AutoAdvance { get; set; } = true;
}

/// <summary>
/// Represents the status of an invoice.
/// </summary>
public enum InvoiceStatus
{
    /// <summary>Invoice is a draft.</summary>
    Draft,

    /// <summary>Invoice is open and awaiting payment.</summary>
    Open,

    /// <summary>Invoice has been paid.</summary>
    Paid,

    /// <summary>Invoice is uncollectible.</summary>
    Uncollectible,

    /// <summary>Invoice was voided.</summary>
    Void
}
