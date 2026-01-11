using Microsoft.AspNetCore.Mvc;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;
using StripeApiWrapper.Services;

namespace SampleApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly ILogger<SubscriptionsController> _logger;

    public SubscriptionsController(ISubscriptionService subscriptionService, ILogger<SubscriptionsController> logger)
    {
        _subscriptionService = subscriptionService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new subscription.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateSubscription([FromBody] CreateSubscriptionRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _subscriptionService.CreateSubscriptionAsync(request, cancellationToken);
            return CreatedAtAction(nameof(GetSubscription), new { subscriptionId = result.Id }, result);
        }
        catch (SubscriptionException ex)
        {
            _logger.LogWarning(ex, "Subscription creation failed: {Reason}", ex.Reason);
            return BadRequest(new { error = ex.UserMessage, reason = ex.Reason.ToString() });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to create subscription");
            return StatusCode(502, new { error = "Subscription service error" });
        }
    }

    /// <summary>
    /// Retrieves a subscription by ID.
    /// </summary>
    [HttpGet("{subscriptionId}")]
    public async Task<IActionResult> GetSubscription(string subscriptionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _subscriptionService.GetSubscriptionAsync(subscriptionId, cancellationToken);
            return Ok(result);
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to retrieve subscription {SubscriptionId}", subscriptionId);
            return NotFound(new { error = "Subscription not found" });
        }
    }

    /// <summary>
    /// Updates a subscription.
    /// </summary>
    [HttpPut("{subscriptionId}")]
    public async Task<IActionResult> UpdateSubscription(
        string subscriptionId,
        [FromBody] UpdateSubscriptionRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _subscriptionService.UpdateSubscriptionAsync(
                subscriptionId,
                request.PriceId,
                request.Quantity,
                cancellationToken);
            return Ok(result);
        }
        catch (SubscriptionException ex)
        {
            return BadRequest(new { error = ex.UserMessage, reason = ex.Reason.ToString() });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to update subscription {SubscriptionId}", subscriptionId);
            return StatusCode(502, new { error = "Subscription service error" });
        }
    }

    /// <summary>
    /// Cancels a subscription.
    /// </summary>
    [HttpPost("{subscriptionId}/cancel")]
    public async Task<IActionResult> CancelSubscription(
        string subscriptionId,
        [FromBody] CancelSubscriptionRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _subscriptionService.CancelSubscriptionAsync(
                subscriptionId,
                request?.CancelAtPeriodEnd ?? true,
                cancellationToken);
            return Ok(result);
        }
        catch (SubscriptionException ex)
        {
            return BadRequest(new { error = ex.UserMessage, reason = ex.Reason.ToString() });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to cancel subscription {SubscriptionId}", subscriptionId);
            return StatusCode(502, new { error = "Subscription service error" });
        }
    }

    /// <summary>
    /// Resumes a canceled subscription.
    /// </summary>
    [HttpPost("{subscriptionId}/resume")]
    public async Task<IActionResult> ResumeSubscription(string subscriptionId, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _subscriptionService.ResumeSubscriptionAsync(subscriptionId, cancellationToken);
            return Ok(result);
        }
        catch (SubscriptionException ex)
        {
            return BadRequest(new { error = ex.UserMessage, reason = ex.Reason.ToString() });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to resume subscription {SubscriptionId}", subscriptionId);
            return StatusCode(502, new { error = "Subscription service error" });
        }
    }

    /// <summary>
    /// Lists subscriptions for a customer.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListSubscriptions(
        [FromQuery] string customerId,
        [FromQuery] string? status = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _subscriptionService.ListSubscriptionsAsync(customerId, status, cancellationToken);
            return Ok(new { subscriptions = result });
        }
        catch (StripeApiException ex)
        {
            _logger.LogError(ex, "Failed to list subscriptions for customer {CustomerId}", customerId);
            return StatusCode(502, new { error = "Subscription service error" });
        }
    }
}

public record UpdateSubscriptionRequest(string? PriceId, int? Quantity);
public record CancelSubscriptionRequest(bool CancelAtPeriodEnd = true);
