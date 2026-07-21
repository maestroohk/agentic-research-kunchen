using System.Linq;

namespace ValidationPoc.Lib;

/// <summary>
/// A deliberately minimal word counter used to verify that
/// the test suite and the formatting gate both exercise the library.
/// </summary>
public static class WordCounter
{
    public static int CountWords(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return 0;
        }

        // Split on any whitespace, collapse empty tokens, and ignore
        // any leading/trailing whitespace.
        return input
            .Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Count(w => !string.IsNullOrWhiteSpace(w));
    }
}
