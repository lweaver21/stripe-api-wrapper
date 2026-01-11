using Microsoft.AspNetCore.Mvc;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;
using StripeApiWrapper.Services;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomersController> _logger;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new customer.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerDto customer, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _customerService.CreateCustomerAsync(customer, cancellationToken);
            return CreatedAtAction(nameof(GetCustomer), new { customerId = result.Id }, result);
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to create customer");
            return StatusCode(502, new { error = "Customer service error" });
        }
    }

    /// <summary>
    /// Retrieves a customer by ID.
    /// </summary>
    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetCustomer(string customerId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _customerService.GetCustomerAsync(customerId, cancellationToken);
            return Ok(result);
        }
        catch (CustomerNotFoundException)
        {
            return NotFound(new { error = "Customer not found" });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to retrieve customer {CustomerId}", customerId);
            return StatusCode(502, new { error = "Customer service error" });
        }
    }

    /// <summary>
    /// Updates a customer.
    /// </summary>
    [HttpPut("{customerId}")]
    public async Task<IActionResult> UpdateCustomer(string customerId, [FromBody] CustomerDto customer, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _customerService.UpdateCustomerAsync(customerId, customer, cancellationToken);
            return Ok(result);
        }
        catch (CustomerNotFoundException)
        {
            return NotFound(new { error = "Customer not found" });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to update customer {CustomerId}", customerId);
            return StatusCode(502, new { error = "Customer service error" });
        }
    }

    /// <summary>
    /// Deletes a customer.
    /// </summary>
    [HttpDelete("{customerId}")]
    public async Task<IActionResult> DeleteCustomer(string customerId, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _customerService.DeleteCustomerAsync(customerId, cancellationToken);
            return deleted ? NoContent() : NotFound(new { error = "Customer not found" });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to delete customer {CustomerId}", customerId);
            return StatusCode(502, new { error = "Customer service error" });
        }
    }

    /// <summary>
    /// Attaches a payment method to a customer.
    /// </summary>
    [HttpPost("{customerId}/payment-methods")]
    public async Task<IActionResult> AttachPaymentMethod(
        string customerId,
        [FromBody] AttachPaymentMethodRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var paymentMethodId = await _customerService.AttachPaymentMethodAsync(
                customerId,
                request.PaymentMethodId,
                request.SetAsDefault,
                cancellationToken);

            return Ok(new { paymentMethodId });
        }
        catch (CustomerNotFoundException)
        {
            return NotFound(new { error = "Customer not found" });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to attach payment method to customer {CustomerId}", customerId);
            return StatusCode(502, new { error = "Payment method service error" });
        }
    }

    /// <summary>
    /// Lists payment methods for a customer.
    /// </summary>
    [HttpGet("{customerId}/payment-methods")]
    public async Task<IActionResult> ListPaymentMethods(
        string customerId,
        [FromQuery] string type = "card",
        CancellationToken cancellationToken = default)
    {
        try
        {
            var paymentMethods = await _customerService.ListPaymentMethodsAsync(customerId, type, cancellationToken);
            return Ok(new { paymentMethods });
        }
        catch (CustomerNotFoundException)
        {
            return NotFound(new { error = "Customer not found" });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to list payment methods for customer {CustomerId}", customerId);
            return StatusCode(502, new { error = "Payment method service error" });
        }
    }
}

public record AttachPaymentMethodRequest(string PaymentMethodId, bool SetAsDefault = false);
