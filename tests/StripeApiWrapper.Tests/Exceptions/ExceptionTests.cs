using FluentAssertions;
using StripeApiWrapper.Exceptions;
using Xunit;

namespace StripeApiWrapper.Tests.Exceptions;

public class StripeApiExceptionTests
{
    [Fact]
    public void Constructor_WithAllParameters_ShouldSetProperties()
    {
        // Arrange & Act
        var exception = new StripeApiException(
            "Test error",
            "invalid_request",
            "api_error",
            "req_123");

        // Assert
        exception.Message.Should().Be("Test error");
        exception.ErrorCode.Should().Be("invalid_request");
        exception.ErrorType.Should().Be("api_error");
        exception.RequestId.Should().Be("req_123");
    }

    [Theory]
    [InlineData("authentication_error", true)]
    [InlineData("api_error", true)]
    [InlineData("card_error", false)]
    [InlineData("invalid_request_error", false)]
    public void IsCritical_ShouldReturnExpectedValue(string errorType, bool expectedCritical)
    {
        // Arrange
        var exception = new StripeApiException("Test", null, errorType, null);

        // Assert
        exception.IsCritical.Should().Be(expectedCritical);
    }
}

public class PaymentFailedExceptionTests
{
    [Fact]
    public void Constructor_WithDeclineCode_ShouldSetProperties()
    {
        // Arrange & Act
        var exception = new PaymentFailedException(
            "Payment declined",
            "insufficient_funds",
            null,
            "pi_123");

        // Assert
        exception.DeclineCode.Should().Be("insufficient_funds");
        exception.PaymentIntentId.Should().Be("pi_123");
        exception.CanRetry.Should().BeTrue();
    }

    [Fact]
    public void UserMessage_WithInsufficientFunds_ShouldReturnFriendlyMessage()
    {
        // Arrange
        var exception = new PaymentFailedException(
            "Payment declined",
            "insufficient_funds",
            null,
            "pi_123");

        // Assert
        exception.UserMessage.Should().Contain("insufficient funds");
    }

    [Theory]
    [InlineData("insufficient_funds", true)]
    [InlineData("processing_error", true)]
    [InlineData("expired_card", false)]
    [InlineData("stolen_card", false)]
    [InlineData("lost_card", false)]
    public void CanRetry_ShouldReturnExpectedValue(string declineCode, bool expectedCanRetry)
    {
        // Arrange
        var exception = new PaymentFailedException("Test", declineCode, null, null);

        // Assert
        exception.CanRetry.Should().Be(expectedCanRetry);
    }

    [Fact]
    public void IsCritical_ShouldReturnFalse()
    {
        // Arrange
        var exception = new PaymentFailedException("Test", "card_declined", null, null);

        // Assert
        exception.IsCritical.Should().BeFalse();
    }
}

public class CustomerNotFoundExceptionTests
{
    [Fact]
    public void Constructor_WithCustomerId_ShouldSetMessage()
    {
        // Arrange & Act
        var exception = new CustomerNotFoundException("cus_123");

        // Assert
        exception.CustomerId.Should().Be("cus_123");
        exception.Message.Should().Contain("cus_123");
    }

    [Fact]
    public void RetryAfterSeconds_ShouldReturnNull()
    {
        // Arrange
        var exception = new CustomerNotFoundException("cus_123");

        // Assert
        exception.RetryAfterSeconds.Should().BeNull();
    }
}

public class SubscriptionExceptionTests
{
    [Fact]
    public void Constructor_WithReason_ShouldSetProperties()
    {
        // Arrange & Act
        var exception = new SubscriptionException(
            "Payment failed",
            "sub_123",
            SubscriptionErrorReason.PaymentFailed);

        // Assert
        exception.SubscriptionId.Should().Be("sub_123");
        exception.Reason.Should().Be(SubscriptionErrorReason.PaymentFailed);
    }

    [Fact]
    public void UserMessage_ShouldReturnFriendlyMessage()
    {
        // Arrange
        var exception = new SubscriptionException(
            "Error",
            "sub_123",
            SubscriptionErrorReason.CustomerNotFound);

        // Assert
        exception.UserMessage.Should().Contain("customer");
    }

    [Theory]
    [InlineData(SubscriptionErrorReason.InvalidConfiguration, true)]
    [InlineData(SubscriptionErrorReason.PaymentFailed, false)]
    [InlineData(SubscriptionErrorReason.CustomerNotFound, false)]
    public void IsCritical_ShouldReturnExpectedValue(SubscriptionErrorReason reason, bool expectedCritical)
    {
        // Arrange
        var exception = new SubscriptionException("Test", null, reason);

        // Assert
        exception.IsCritical.Should().Be(expectedCritical);
    }
}
