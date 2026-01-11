using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Models;
using StripeApiWrapper.Services;
using Xunit;

namespace StripeApiWrapper.Tests.Services;

public class SubscriptionServiceTests
{
    private readonly Mock<IStripeClient> _mockStripeClient;
    private readonly IOptions<StripeOptions> _options;
    private readonly StripeApiWrapper.Services.SubscriptionService _service;

    public SubscriptionServiceTests()
    {
        _mockStripeClient = new Mock<IStripeClient>();
        _options = Options.Create(new StripeOptions { SecretKey = "sk_test_123" });
        _service = new StripeApiWrapper.Services.SubscriptionService(_mockStripeClient.Object, _options);
    }

    [Fact]
    public void Constructor_WithNullStripeClient_ShouldThrow()
    {
        // Act
        var act = () => new StripeApiWrapper.Services.SubscriptionService(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("stripeClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrow()
    {
        // Act
        var act = () => new StripeApiWrapper.Services.SubscriptionService(_mockStripeClient.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public async Task CreateSubscriptionAsync_WithNullRequest_ShouldThrow()
    {
        // Act
        var act = () => _service.CreateSubscriptionAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task UpdateSubscriptionAsync_WithNullSubscriptionId_ShouldThrow()
    {
        // Act
        var act = () => _service.UpdateSubscriptionAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("subscriptionId");
    }

    [Fact]
    public async Task UpdateSubscriptionAsync_WithEmptySubscriptionId_ShouldThrow()
    {
        // Act
        var act = () => _service.UpdateSubscriptionAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("subscriptionId");
    }

    [Fact]
    public async Task CancelSubscriptionAsync_WithNullSubscriptionId_ShouldThrow()
    {
        // Act
        var act = () => _service.CancelSubscriptionAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("subscriptionId");
    }

    [Fact]
    public async Task ResumeSubscriptionAsync_WithNullSubscriptionId_ShouldThrow()
    {
        // Act
        var act = () => _service.ResumeSubscriptionAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("subscriptionId");
    }

    [Fact]
    public async Task GetSubscriptionAsync_WithNullSubscriptionId_ShouldThrow()
    {
        // Act
        var act = () => _service.GetSubscriptionAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("subscriptionId");
    }

    [Fact]
    public async Task ListSubscriptionsAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.ListSubscriptionsAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task ListSubscriptionsAsync_WithEmptyCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.ListSubscriptionsAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }
}

public class SubscriptionServiceRegistrationTests
{
    [Fact]
    public void AddStripeServices_ShouldRegisterSubscriptionService()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStripeServices(options =>
        {
            options.SecretKey = "sk_test_123";
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var subscriptionService = provider.GetService<ISubscriptionService>();
        subscriptionService.Should().NotBeNull();
        subscriptionService.Should().BeOfType<StripeApiWrapper.Services.SubscriptionService>();
    }
}
