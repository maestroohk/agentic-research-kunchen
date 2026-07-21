using ValidationPoc.Lib;
using Xunit;

namespace ValidationPoc.Tests;

public class WordCounterTests
{
    [Fact]
    public void CountWords_WithEmptyString_ReturnsZero()
    {
        Assert.Equal(0, WordCounter.CountWords(string.Empty));
    }

    [Fact]
    public void CountWords_WithNull_ReturnsZero()
    {
        Assert.Equal(0, WordCounter.CountWords(null));
    }

    [Fact]
    public void CountWords_WithSingleWord_ReturnsOne()
    {
        Assert.Equal(1, WordCounter.CountWords("hello"));
    }

    [Fact]
    public void CountWords_WithMultipleWordsSeparatedBySpaces_ReturnsWordCount()
    {
        Assert.Equal(3, WordCounter.CountWords("one two three"));
    }

    [Fact]
    public void CountWords_WithNewlinesAndTabs_ReturnsWordCount()
    {
        Assert.Equal(4, WordCounter.CountWords("alpha\nbeta\tgamma\ndelta"));
    }

    [Fact]
    public void CountWords_WithExcessWhitespace_CollapsesEmptyTokens()
    {
        Assert.Equal(2, WordCounter.CountWords("  spaced   out  "));
    }
}
