using Microsoft.Extensions.Options;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;

namespace StripeApiWrapper.Services;

/// <summary>
/// Implementation of the customer service.
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly Stripe.CustomerService _customerService;
    private readonly PaymentMethodService _paymentMethodService;
    private readonly StripeOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="CustomerService"/> class.
    /// </summary>
    /// <param name="stripeClient">The Stripe client.</param>
    /// <param name="options">The Stripe options.</param>
    public CustomerService(IStripeClient stripeClient, IOptions<StripeOptions> options)
    {
        ArgumentNullException.ThrowIfNull(stripeClient);
        ArgumentNullException.ThrowIfNull(options);

        _customerService = new Stripe.CustomerService(stripeClient);
        _paymentMethodService = new PaymentMethodService(stripeClient);
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(customer);

        try
        {
            var options = new CustomerCreateOptions
            {
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.Phone,
                Description = customer.Description,
                Metadata = customer.Metadata,
                Address = MapAddress(customer.Address),
                Shipping = MapShipping(customer.Shipping)
            };

            var stripeCustomer = await _customerService.CreateAsync(options, cancellationToken: cancellationToken);

            return MapToCustomerDto(stripeCustomer);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<CustomerDto> UpdateCustomerAsync(string customerId, CustomerDto customer, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentNullException.ThrowIfNull(customer);

        try
        {
            var options = new CustomerUpdateOptions
            {
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.Phone,
                Description = customer.Description,
                Metadata = customer.Metadata,
                Address = MapAddress(customer.Address),
                Shipping = MapShipping(customer.Shipping)
            };

            var stripeCustomer = await _customerService.UpdateAsync(customerId, options, cancellationToken: cancellationToken);

            return MapToCustomerDto(stripeCustomer);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, customerId);
        }
    }

    /// <inheritdoc/>
    public async Task<CustomerDto> GetCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        try
        {
            var stripeCustomer = await _customerService.GetAsync(customerId, cancellationToken: cancellationToken);

            if (stripeCustomer.Deleted == true)
            {
                throw new CustomerNotFoundException(customerId);
            }

            return MapToCustomerDto(stripeCustomer);
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, customerId);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCustomerAsync(string customerId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        try
        {
            var result = await _customerService.DeleteAsync(customerId, cancellationToken: cancellationToken);
            return result.Deleted ?? false;
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, customerId);
        }
    }

    /// <inheritdoc/>
    public async Task<string> AttachPaymentMethodAsync(string customerId, string paymentMethodId, bool setAsDefault = false, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentMethodId);

        try
        {
            var attachOptions = new PaymentMethodAttachOptions
            {
                Customer = customerId
            };

            var paymentMethod = await _paymentMethodService.AttachAsync(paymentMethodId, attachOptions, cancellationToken: cancellationToken);

            if (setAsDefault)
            {
                var updateOptions = new CustomerUpdateOptions
                {
                    InvoiceSettings = new CustomerInvoiceSettingsOptions
                    {
                        DefaultPaymentMethod = paymentMethodId
                    }
                };

                await _customerService.UpdateAsync(customerId, updateOptions, cancellationToken: cancellationToken);
            }

            return paymentMethod.Id;
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, customerId);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DetachPaymentMethodAsync(string paymentMethodId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentMethodId);

        try
        {
            var result = await _paymentMethodService.DetachAsync(paymentMethodId, cancellationToken: cancellationToken);
            return result.Customer == null;
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex);
        }
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<string>> ListPaymentMethodsAsync(string customerId, string type = "card", CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);

        try
        {
            var options = new PaymentMethodListOptions
            {
                Customer = customerId,
                Type = type
            };

            var paymentMethods = await _paymentMethodService.ListAsync(options, cancellationToken: cancellationToken);

            return paymentMethods.Data.Select(pm => pm.Id).ToList();
        }
        catch (StripeException ex)
        {
            throw MapStripeException(ex, customerId);
        }
    }

    private static AddressOptions? MapAddress(AddressDto? address)
    {
        if (address == null)
        {
            return null;
        }

        return new AddressOptions
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static ShippingOptions? MapShipping(ShippingDto? shipping)
    {
        if (shipping == null)
        {
            return null;
        }

        return new ShippingOptions
        {
            Name = shipping.Name,
            Phone = shipping.Phone,
            Address = MapAddress(shipping.Address)
        };
    }

    private static CustomerDto MapToCustomerDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Email = customer.Email,
            Name = customer.Name,
            Phone = customer.Phone,
            Description = customer.Description,
            Metadata = customer.Metadata?.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
            Address = MapAddressDto(customer.Address),
            Shipping = MapShippingDto(customer.Shipping),
            DefaultPaymentMethodId = customer.InvoiceSettings?.DefaultPaymentMethodId,
            Currency = customer.Currency,
            CreatedAt = customer.Created
        };
    }

    private static AddressDto? MapAddressDto(Address? address)
    {
        if (address == null)
        {
            return null;
        }

        return new AddressDto
        {
            Line1 = address.Line1,
            Line2 = address.Line2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country
        };
    }

    private static ShippingDto? MapShippingDto(Shipping? shipping)
    {
        if (shipping == null)
        {
            return null;
        }

        return new ShippingDto
        {
            Name = shipping.Name,
            Phone = shipping.Phone,
            Address = MapAddressDto(shipping.Address)
        };
    }

    private static Exception MapStripeException(StripeException ex, string? customerId = null)
    {
        if (ex.StripeError?.Code == "resource_missing" && customerId != null)
        {
            return new CustomerNotFoundException(customerId, ex);
        }

        return new StripeApiException(
            ex.Message,
            ex.StripeError?.Code,
            ex.StripeError?.Type,
            null,
            ex);
    }
}
