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

public class InvoiceServiceTests
{
    private readonly Mock<IStripeClient> _mockStripeClient;
    private readonly IOptions<StripeOptions> _options;
    private readonly StripeApiWrapper.Services.InvoiceService _service;

    public InvoiceServiceTests()
    {
        _mockStripeClient = new Mock<IStripeClient>();
        _options = Options.Create(new StripeOptions { SecretKey = "sk_test_123" });
        _service = new StripeApiWrapper.Services.InvoiceService(_mockStripeClient.Object, _options);
    }

    [Fact]
    public void Constructor_WithNullStripeClient_ShouldThrow()
    {
        // Act
        var act = () => new StripeApiWrapper.Services.InvoiceService(null!, _options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("stripeClient");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrow()
    {
        // Act
        var act = () => new StripeApiWrapper.Services.InvoiceService(_mockStripeClient.Object, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public async Task CreateInvoiceAsync_WithNullRequest_ShouldThrow()
    {
        // Act
        var act = () => _service.CreateInvoiceAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("request");
    }

    [Fact]
    public async Task FinalizeInvoiceAsync_WithNullInvoiceId_ShouldThrow()
    {
        // Act
        var act = () => _service.FinalizeInvoiceAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("invoiceId");
    }

    [Fact]
    public async Task FinalizeInvoiceAsync_WithEmptyInvoiceId_ShouldThrow()
    {
        // Act
        var act = () => _service.FinalizeInvoiceAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("invoiceId");
    }

    [Fact]
    public async Task PayInvoiceAsync_WithNullInvoiceId_ShouldThrow()
    {
        // Act
        var act = () => _service.PayInvoiceAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("invoiceId");
    }

    [Fact]
    public async Task VoidInvoiceAsync_WithNullInvoiceId_ShouldThrow()
    {
        // Act
        var act = () => _service.VoidInvoiceAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("invoiceId");
    }

    [Fact]
    public async Task GetInvoiceAsync_WithNullInvoiceId_ShouldThrow()
    {
        // Act
        var act = () => _service.GetInvoiceAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("invoiceId");
    }

    [Fact]
    public async Task ListInvoicesAsync_WithNullCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.ListInvoicesAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task ListInvoicesAsync_WithEmptyCustomerId_ShouldThrow()
    {
        // Act
        var act = () => _service.ListInvoicesAsync("");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("customerId");
    }

    [Fact]
    public async Task SendInvoiceAsync_WithNullInvoiceId_ShouldThrow()
    {
        // Act
        var act = () => _service.SendInvoiceAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("invoiceId");
    }
}

public class InvoiceServiceRegistrationTests
{
    [Fact]
    public void AddStripeServices_ShouldRegisterInvoiceService()
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
        var invoiceService = provider.GetService<IInvoiceService>();
        invoiceService.Should().NotBeNull();
        invoiceService.Should().BeOfType<StripeApiWrapper.Services.InvoiceService>();
    }
}
