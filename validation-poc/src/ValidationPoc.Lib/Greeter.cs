namespace ValidationPoc.Lib;

/// <summary>
/// A tiny helper that demonstrates the kind of code an agent might generate
/// against this proof-of-concept repository.
/// </summary>
public static class Greeter
{
    public static string Greet(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return "Hello, world!";
        }

        return $"Hello, {name.Trim()}!";
    }
}
