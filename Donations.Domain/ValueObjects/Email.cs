namespace Proiect_POO.ValueObjects;

public sealed record Email
{
    public string Value { get; }

    public Email(string value)
    {
        if(string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        if (!value.Contains("@") || !value.Contains("."))
            throw new ArgumentException("Email invalid.");
        Value = value;
    }
    public override string ToString() => Value;
}