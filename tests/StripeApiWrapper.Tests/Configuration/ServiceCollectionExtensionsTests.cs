using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StripeApiWrapper.Configuration;
using Stripe;
using Xunit;

namespace StripeApiWrapper.Tests.Configuration;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddStripeServices_WithAction_ShouldConfigureOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        const string expectedSecretKey = "sk_test_1234567890";

        // Act
        services.AddStripeServices(options =>
        {
            options.SecretKey = expectedSecretKey;
            options.EnableTestMode = true;
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<StripeOptions>>();

        // Assert
        options.Value.SecretKey.Should().Be(expectedSecretKey);
        options.Value.EnableTestMode.Should().BeTrue();
    }

    [Fact]
    public void AddStripeServices_WithAction_ShouldRegisterStripeClient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddStripeServices(options =>
        {
            options.SecretKey = "sk_test_1234567890";
        });

        var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IStripeClient>();

        // Assert
        client.Should().NotBeNull();
        client.Should().BeOfType<StripeClient>();
    }

    [Fact]
    public void AddStripeServices_WithNullServices_ShouldThrow()
    {
        // Arrange
        IServiceCollection services = null!;

        // Act
        var act = () => services.AddStripeServices(_ => { });

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("services");
    }

    [Fact]
    public void AddStripeServices_WithNullAction_ShouldThrow()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var act = () => services.AddStripeServices((Action<StripeOptions>)null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("configureOptions");
    }

    [Fact]
    public void AddStripeServices_ShouldReturnSameServiceCollection()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var result = services.AddStripeServices(options =>
        {
            options.SecretKey = "sk_test_1234567890";
        });

        // Assert
        result.Should().BeSameAs(services);
    }
}
