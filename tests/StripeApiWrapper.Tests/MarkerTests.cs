using FluentAssertions;
using Xunit;

namespace StripeApiWrapper.Tests;

public class MarkerTests
{
    [Fact]
    public void Version_ShouldReturnExpectedVersion()
    {
        // Act
        var version = StripeApiWrapperMarker.Version;

        // Assert
        version.Should().Be("1.0.0");
    }
}
