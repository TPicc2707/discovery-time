namespace Activity.Domain.Models;

public class Theme : Aggregate<ThemeId>
{
    public string Name { get; private set; } = default!;

    public static Theme Create(ThemeId id, string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        var theme = new Theme
        {
            Id = id,
            Name = name,
        };

        theme.AddDomainEvent(new ThemeCreateEvent(theme));

        return theme;
    }

    public void Update(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        Name = name;

        AddDomainEvent(new ThemeUpdateEvent(this));
    }
}
