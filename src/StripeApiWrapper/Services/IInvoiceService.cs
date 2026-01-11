using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Service interface for invoice operations.
/// </summary>
public interface IInvoiceService
{
    /// <summary>
    /// Creates a new invoice.
    /// </summary>
    /// <param name="request">The invoice creation request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created invoice.</returns>
    Task<InvoiceDto> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finalizes a draft invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The finalized invoice.</returns>
    Task<InvoiceDto> FinalizeInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Pays an invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <param name="paymentMethodId">Optional payment method ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paid invoice.</returns>
    Task<InvoiceDto> PayInvoiceAsync(string invoiceId, string? paymentMethodId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Voids an invoice.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The voided invoice.</returns>
    Task<InvoiceDto> VoidInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an invoice by ID.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The invoice.</returns>
    Task<InvoiceDto> GetInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists invoices for a customer.
    /// </summary>
    /// <param name="customerId">The customer ID.</param>
    /// <param name="status">Filter by status (optional).</param>
    /// <param name="limit">Maximum number of invoices to return.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of invoices.</returns>
    Task<IReadOnlyList<InvoiceDto>> ListInvoicesAsync(string customerId, string? status = null, int limit = 10, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an invoice to the customer.
    /// </summary>
    /// <param name="invoiceId">The invoice ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sent invoice.</returns>
    Task<InvoiceDto> SendInvoiceAsync(string invoiceId, CancellationToken cancellationToken = default);
}
