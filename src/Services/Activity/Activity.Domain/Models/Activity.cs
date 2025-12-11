namespace Activity.Domain.Models;

public class Activity : Entity<ActivityId>
{
    public ThemeId ThemeId { get; private set; } = default!;
    public ActivityName Name { get; private set; } = default!;
    public ActivityDetails Details { get; private set; } = default!;

    public static Activity Create(ActivityId id, ThemeId themeId, ActivityName name, ActivityDetails details)
    {
        var activity = new Activity
        {
            Id = id,
            ThemeId = themeId,
            Name = name,
            Details = details
        };

        return activity;
    }

    public void Update(ThemeId themeId, ActivityName name, ActivityDetails details)
    {
        ThemeId = themeId;
        Name = name;
        Details = details;
    }
}
