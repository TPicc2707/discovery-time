namespace Activity.Infrastructure.Data.Extensions;

internal class InitialData
{
    public static IEnumerable<Theme> Themes =>
        new List<Theme>
        {
            Theme.Create(ThemeId.Of(new Guid("2f7fb473-031e-44f1-b97b-dbb48a5aa99f")), "Dummy Theme")
        };

    public static IEnumerable<Domain.Models.Activity> Activities
    {
        get
        {
            var activityDetails = ActivityDetails.Of("This is a dummy activity description.", "https://fake.url.com/test", DateTime.UtcNow);

            var activitiy = Domain.Models.Activity.Create(ActivityId.Of(new Guid("c94e680f-f87b-4b82-84e0-6f8c867523a5")), ThemeId.Of(new Guid("2f7fb473-031e-44f1-b97b-dbb48a5aa99f")), ActivityName.Of("Test Activity"), activityDetails);

            return new List<Domain.Models.Activity> { activitiy };
        }
    }
}
