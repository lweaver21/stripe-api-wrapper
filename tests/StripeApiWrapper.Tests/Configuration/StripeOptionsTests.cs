using FluentAssertions;
using StripeApiWrapper.Configuration;
using Xunit;

namespace StripeApiWrapper.Tests.Configuration;

public class StripeOptionsTests
{
    [Fact]
    public void SectionName_ShouldBeStripe()
    {
        // Assert
        StripeOptions.SectionName.Should().Be("Stripe");
    }

    [Fact]
    public void DefaultValues_ShouldBeSet()
    {
        // Arrange & Act
        var options = new StripeOptions();

        // Assert
        options.SecretKey.Should().BeEmpty();
        options.WebhookSecret.Should().BeEmpty();
        options.PublishableKey.Should().BeNull();
        options.EnableTestMode.Should().BeFalse();
        options.MaxRetries.Should().Be(2);
    }

    [Fact]
    public void Validate_WithValidSecretKey_ShouldNotThrow()
    {
        // Arrange
        var options = new StripeOptions
        {
            SecretKey = "sk_test_1234567890"
        };

        // Act
        var act = () => options.Validate();

        // Assert
        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithMissingSecretKey_ShouldThrow(string? secretKey)
    {
        // Arrange
        var options = new StripeOptions
        {
            SecretKey = secretKey!
        };

        // Act
        var act = () => options.Validate();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*SecretKey*required*");
    }
}
