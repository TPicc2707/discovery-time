namespace Activity.Application.Extensions;

public static class ActivityExtensions
{
    public static IEnumerable<ActivityDto> ToActivityDtoList(this IEnumerable<Domain.Models.Activity> activities)
    {
        return activities.Select(activity => new ActivityDto(
            Id: activity.Id.Value,
            ThemeId: activity.ThemeId.Value,
            Name: activity.Name.Value,
            Details: new ActivityDetailsDto(
                activity.Details.Description,
                activity.Details.Url,
                activity.Details.Date)));
    }

    public static ActivityDto ToSingleActivityDto(this Domain.Models.Activity activity)
    {
        return new ActivityDto(
            Id: activity.Id.Value,
            ThemeId: activity.ThemeId.Value,
            Name: activity.Name.Value,
            Details: new ActivityDetailsDto(
                activity.Details.Description,
                activity.Details.Url,
                activity.Details.Date));
    }
}
