using ValidationPoc.Lib;
using Xunit;

namespace ValidationPoc.Tests;

public class GreeterTests
{
    [Fact]
    public void Greet_WithName_ReturnsPersonalizedGreeting()
    {
        var result = Greeter.Greet("Henry");
        Assert.Equal("Hello, Henry!", result);
    }

    [Fact]
    public void Greet_WithEmptyString_ReturnsDefaultGreeting()
    {
        var result = Greeter.Greet(string.Empty);
        Assert.Equal("Hello, world!", result);
    }

    [Fact]
    public void Greet_WithWhitespace_ReturnsDefaultGreeting()
    {
        var result = Greeter.Greet("   ");
        Assert.Equal("Hello, world!", result);
    }

    [Fact]
    public void Greet_WithSurroundingSpaces_TrimsName()
    {
        var result = Greeter.Greet("  Ada  ");
        Assert.Equal("Hello, Ada!", result);
    }
}
