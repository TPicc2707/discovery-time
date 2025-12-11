namespace Activity.Domain.ValueObjects;

public record ActivityName
{
    public string Value { get; }
    private ActivityName(string value) => Value = value;

    public static ActivityName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        //ArgumentOutOfRangeException.ThrowIfNotEqual(value.Length, DefaultLength);

        return new ActivityName(value);
    }
}
