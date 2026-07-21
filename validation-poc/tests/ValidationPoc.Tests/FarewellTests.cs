using ValidationPoc.Lib;
using Xunit;

namespace ValidationPoc.Tests;

public class FarewellTests
{
    [Fact]
    public void Goodbye_WithName_ReturnsPersonalizedFarewell()
    {
        Assert.Equal("Goodbye, Henry!", Farewell.Goodbye("Henry"));
    }

    [Fact]
    public void Goodbye_WithEmpty_ReturnsDefault()
    {
        Assert.Equal("Goodbye, world!", Farewell.Goodbye(string.Empty));
    }
}
