#if ENABLE_FAILING_TESTS
using ValidationPoc.Lib;
using Xunit;

namespace ValidationPoc.Tests;

// These tests are intentionally failing to exercise the red branch of the
// quality gate. The file is compiled out by default so the test suite stays
// green. To enable, pass `/p:DefineConstants=ENABLE_FAILING_TESTS` to
// `dotnet build` (see README.md for the exact command).
public class KnownFailingTests
{
    [Fact]
    public void Greeter_ShouldProduceExpectedGreeting_ForKnownName()
    {
        var result = Greeter.Greet("Henry");
        Assert.Equal("Good morning, Henry", result);
    }

    [Fact]
    public void WordCounter_ShouldCountWordsCorrectly_ForKnownInput()
    {
        Assert.Equal(99, WordCounter.CountWords("one two three"));
    }
}
#endif
