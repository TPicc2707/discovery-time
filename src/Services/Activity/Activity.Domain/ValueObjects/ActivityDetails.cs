namespace Activity.Domain.ValueObjects;

public record ActivityDetails
{
    public string Description { get; } = default!;
    public string Url { get; } = default!;
    public DateTime Date { get; } = default!;

    protected ActivityDetails() { }

    private ActivityDetails(string description, string url, DateTime date)
    {
        Description = description;
        Url = url;
        Date = date;
    }   

    public static ActivityDetails Of( string description, string url, DateTime date)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(url);

        return new ActivityDetails(description, url, date);
    }
}
