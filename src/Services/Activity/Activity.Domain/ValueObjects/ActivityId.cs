namespace Activity.Domain.ValueObjects;

public class ActivityId
{
    public Guid Value { get; }
    private ActivityId(Guid value) => Value = value;
    public static ActivityId Of(Guid value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value == Guid.Empty)
            throw new DomainException("ActivityId cannot be empty.");

        return new ActivityId(value);
    }
}
