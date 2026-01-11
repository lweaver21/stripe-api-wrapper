using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using Stripe;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Models;
using StripeApiWrapper.Services;
using Xunit;

namespace StripeApiWrapper.Tests.Services;

public class PaymentServiceTests
{
    private readonly Mock<IStripeClient> _mockStripeClient;
    private readonly IOptions<StripeOptions> _options;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _mockStripeClient = new Mock<IStripeClient>();
        _options = Options.Create(new StripeOptions { SecretKey = "sk_test_123" });
        _service = new PaymentService(_mockStripeClient.Object, _options);
    }

    [Fact]
    public void Constructor_WithNullStripeClient_ShouldThrow()
    {
        // Act
        var act = () => new PaymentService(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("stripeClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrow()
    {
        // Act
        var act = () => new PaymentService(_mockStripeClient.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public async Task CreatePaymentAsync_WithNullRequest_ShouldThrow()
    {
        // Act
        var act = () => _service.CreatePaymentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WithNullPaymentIntentId_ShouldThrow()
    {
        // Act
        var act = () => _service.ConfirmPaymentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentIntentId");
    }

    [Fact]
    public async Task ConfirmPaymentAsync_WithEmptyPaymentIntentId_ShouldThrow()
    {
        // Act
        var act = () => _service.ConfirmPaymentAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentIntentId");
    }

    [Fact]
    public async Task CapturePaymentAsync_WithNullPaymentIntentId_ShouldThrow()
    {
        // Act
        var act = () => _service.CapturePaymentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentIntentId");
    }

    [Fact]
    public async Task RefundPaymentAsync_WithNullPaymentIntentId_ShouldThrow()
    {
        // Act
        var act = () => _service.RefundPaymentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentIntentId");
    }

    [Fact]
    public async Task CancelPaymentAsync_WithNullPaymentIntentId_ShouldThrow()
    {
        // Act
        var act = () => _service.CancelPaymentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentIntentId");
    }

    [Fact]
    public async Task GetPaymentAsync_WithNullPaymentIntentId_ShouldThrow()
    {
        // Act
        var act = () => _service.GetPaymentAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentIntentId");
    }
}

public class PaymentServiceRegistrationTests
{
    [Fact]
    public void AddStripeServices_ShouldRegisterPaymentService()
    {
        // Arrange
        var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();

        // Act
        services.AddStripeServices(options =>
        {
            options.SecretKey = "sk_test_123";
        });

        var provider = services.BuildServiceProvider();

        // Assert
        var paymentService = provider.GetService<IPaymentService>();
        paymentService.Should().NotBeNull();
        paymentService.Should().BeOfType<PaymentService>();
    }
}
