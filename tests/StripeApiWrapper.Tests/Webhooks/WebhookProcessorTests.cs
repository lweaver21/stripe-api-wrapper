using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StripeApiWrapper.Configuration;
using StripeApiWrapper.Exceptions;
using StripeApiWrapper.Webhooks;
using Xunit;

namespace StripeApiWrapper.Tests.Webhooks;

public class WebhookProcessorTests
{
    private readonly ServiceCollection _services;
    private readonly StripeOptions _options;

    public WebhookProcessorTests()
    {
        _services = new ServiceCollection();
        _options = new StripeOptions
        {
            SecretKey = "sk_test_123",
            WebhookSecret = "whsec_test_123"
        };
    }

    [Fact]
    public void Constructor_WithNullServiceProvider_ShouldThrow()
    {
        // Arrange
        var options = Options.Create(_options);

        // Act
        var act = () => new WebhookProcessor(null!, options);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("serviceProvider");
    }

    [Fact]
    public void Constructor_WithNullOptions_ShouldThrow()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();

        // Act
        var act = () => new WebhookProcessor(provider, null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("options");
    }

    [Fact]
    public void ValidateAndParseWebhook_WithNullPayload_ShouldThrow()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var options = Options.Create(_options);
        var processor = new WebhookProcessor(provider, options);

        // Act
        var act = () => processor.ValidateAndParseWebhook(null!, "sig_123");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("payload");
    }

    [Fact]
    public void ValidateAndParseWebhook_WithEmptyPayload_ShouldThrow()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var options = Options.Create(_options);
        var processor = new WebhookProcessor(provider, options);

        // Act
        var act = () => processor.ValidateAndParseWebhook("", "sig_123");

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("payload");
    }

    [Fact]
    public void ValidateAndParseWebhook_WithNullSignature_ShouldThrow()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var options = Options.Create(_options);
        var processor = new WebhookProcessor(provider, options);

        // Act
        var act = () => processor.ValidateAndParseWebhook("{}", null!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("signature");
    }

    [Fact]
    public void ValidateAndParseWebhook_WithMissingWebhookSecret_ShouldThrow()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var optionsWithoutSecret = Options.Create(new StripeOptions { SecretKey = "sk_test_123" });
        var processor = new WebhookProcessor(provider, optionsWithoutSecret);

        // Act
        var act = () => processor.ValidateAndParseWebhook("{}", "sig_123");

        // Assert
        act.Should().Throw<StripeApiException>()
            .Where(e => e.ErrorCode == "webhook_secret_missing");
    }

    [Fact]
    public async Task ProcessEventAsync_WithNullEvent_ShouldThrow()
    {
        // Arrange
        var provider = _services.BuildServiceProvider();
        var options = Options.Create(_options);
        var processor = new WebhookProcessor(provider, options);

        // Act
        var act = () => processor.ProcessEventAsync(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("stripeEvent");
    }
}

public class WebhookEndpointExtensionsTests
{
    [Fact]
    public void AddStripeWebhooks_ShouldRegisterWebhookProcessor()
    {
        // Arrange
        var services = new ServiceCollection();
        services.Configure<StripeOptions>(o => o.SecretKey = "sk_test_123");

        // Act
        services.AddStripeWebhooks();
        var provider = services.BuildServiceProvider();

        // Assert
        var processor = provider.GetService<WebhookProcessor>();
        processor.Should().NotBeNull();
    }
}
