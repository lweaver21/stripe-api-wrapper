using System.ComponentModel.DataAnnotations;

namespace StripeApiWrapper.Models;

/// <summary>
/// Data transfer object for customer operations.
/// </summary>
public class CustomerDto
{
    /// <summary>
    /// Gets or sets the Stripe customer ID.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets the customer's email address.
    /// </summary>
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the customer's full name.
    /// </summary>
    [StringLength(256)]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the customer's phone number.
    /// </summary>
    [Phone]
    public string? Phone { get; set; }

    /// <summary>
    /// Gets or sets the customer's description.
    /// </summary>
    [StringLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the customer's billing address.
    /// </summary>
    public AddressDto? Address { get; set; }

    /// <summary>
    /// Gets or sets the customer's shipping address.
    /// </summary>
    public ShippingDto? Shipping { get; set; }

    /// <summary>
    /// Gets or sets additional metadata for the customer.
    /// </summary>
    public Dictionary<string, string>? Metadata { get; set; }

    /// <summary>
    /// Gets or sets the default payment method ID.
    /// </summary>
    public string? DefaultPaymentMethodId { get; set; }

    /// <summary>
    /// Gets or sets the customer's preferred currency.
    /// </summary>
    [StringLength(3, MinimumLength = 3)]
    public string? Currency { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the customer was created.
    /// </summary>
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// Address information.
/// </summary>
public class AddressDto
{
    /// <summary>
    /// Gets or sets the first line of the address.
    /// </summary>
    public string? Line1 { get; set; }

    /// <summary>
    /// Gets or sets the second line of the address.
    /// </summary>
    public string? Line2 { get; set; }

    /// <summary>
    /// Gets or sets the city.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the state or province.
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Gets or sets the postal code.
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// Gets or sets the two-letter country code.
    /// </summary>
    [StringLength(2, MinimumLength = 2)]
    public string? Country { get; set; }
}

/// <summary>
/// Shipping information.
/// </summary>
public class ShippingDto
{
    /// <summary>
    /// Gets or sets the recipient name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the shipping address.
    /// </summary>
    public AddressDto? Address { get; set; }

    /// <summary>
    /// Gets or sets the phone number.
    /// </summary>
    public string? Phone { get; set; }
}
