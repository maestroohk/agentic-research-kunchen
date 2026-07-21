namespace ValidationPoc.Lib;

/// <summary>
/// A trivial addition that exercises the no-mistakes gate on the
/// green branch.
/// </summary>
public static class Farewell
{
    public static string Goodbye(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Goodbye, world!";
        }

        return $"Goodbye, {name.Trim()}!";
    }
}
