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

public class CustomerServiceTests
{
    private readonly Mock<IStripeClient> _mockStripeClient;
    private readonly IOptions<StripeOptions> _options;
    private readonly StripeApiWrapper.Services.CustomerService _service;

    public CustomerServiceTests()
    {
        _mockStripeClient = new Mock<IStripeClient>();
        _options = Options.Create(new StripeOptions { SecretKey = "sk_test_123" });
        _service = new StripeApiWrapper.Services.CustomerService(_mockStripeClient.Object, _options);
    }

    [Fact]
    public void Constructor_WithNullStripeClient_ShouldThrow()
    {
        // Act
        var act = () => new StripeApiWrapper.Services.CustomerService(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("stripeClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrow()
    {
        // Act
        var act = () => new StripeApiWrapper.Services.CustomerService(_mockStripeClient.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public async Task CreateCustomerAsync_WithNullCustomer_ShouldThrow()
    {
        // Act
        var act = () => _service.CreateCustomerAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("customer");
    }

    [Fact]
    public async Task UpdateCustomerAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.UpdateCustomerAsync(null!, new CustomerDto());

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task UpdateCustomerAsync_WithNullCustomer_ShouldThrow()
    {
        // Act
        var act = () => _service.UpdateCustomerAsync("cus_123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("customer");
    }

    [Fact]
    public async Task GetCustomerAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.GetCustomerAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task GetCustomerAsync_WithEmptyCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.GetCustomerAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task DeleteCustomerAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.DeleteCustomerAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task AttachPaymentMethodAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.AttachPaymentMethodAsync(null!, "pm_123");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task AttachPaymentMethodAsync_WithNullPaymentMethodId_ShouldThrow()
    {
        // Act
        var act = () => _service.AttachPaymentMethodAsync("cus_123", null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentMethodId");
    }

    [Fact]
    public async Task DetachPaymentMethodAsync_WithNullPaymentMethodId_ShouldThrow()
    {
        // Act
        var act = () => _service.DetachPaymentMethodAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("paymentMethodId");
    }

    [Fact]
    public async Task ListPaymentMethodsAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.ListPaymentMethodsAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }
}

public class CustomerServiceRegistrationTests
{
    [Fact]
    public void AddStripeServices_ShouldRegisterCustomerService()
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
        var customerService = provider.GetService<ICustomerService>();
        customerService.Should().NotBeNull();
        customerService.Should().BeOfType<StripeApiWrapper.Services.CustomerService>();
    }
}
