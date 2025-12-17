using Activity.Domain.ValueObjects;
using Bogus;

namespace Discovery.Time.Tests.Data.MockData;

public class ThemeFakeData
{
    public static Faker<Theme.API.Models.Theme> GenerateTheme()
    {
        return new Faker<Theme.API.Models.Theme>()
            .RuleFor(t => t.Id, f => f.Random.Guid())
            .RuleFor(t => t.Name, f => f.Name.JobType())
            .RuleFor(t => t.Number, f => f.Random.Number(1))
            .RuleFor(t => t.Letter, f => f.Random.String(2))
            .RuleFor(t => t.StartDate, f => f.Date.Future())
            .RuleFor(t => t.EndDate, f => f.Date.Future())
            .RuleFor(t => t.CreatedDate, f => f.Date.Recent())
            .RuleFor(t => t.CreatedBy, f => f.Name.FullName())
            .RuleFor(t => t.ModifiedDate, f => f.Date.Recent())
            .RuleFor(t => t.ModifiedBy, f => f.Name.FullName());
    }

    public static List<Theme.API.Models.Theme> GenerateThemes(int count)
    {
        return GenerateTheme().Generate(count);
    }

    public static Faker<Activity.Domain.Models.Theme> GenerateActivityTheme()
    {
        return new Faker<Activity.Domain.Models.Theme>()
            .RuleFor(t => t.Id, f => ThemeId.Of(Guid.NewGuid()))
            .RuleFor(t => t.Name, f => f.Name.JobType())
            .RuleFor(t => t.CreatedBy, f => f.Name.FullName())
            .RuleFor(t => t.CreatedAt, f => f.Date.Future())
            .RuleFor(t => t.LastModifiedBy, f => f.Name.FullName())
            .RuleFor(t => t.LastModified, f => f.Date.Future());
    }

    public static Faker<Activity.Domain.Models.Activity> GenerateActivity(List<Guid> themeIds, Faker faker)
    {
        var themeSelect = faker.Random.Int(1, themeIds.Count == 1 ? themeIds.Count : themeIds.Count - 1);
        var themeId = themeIds.Skip(themeSelect == 1 ? 0 : themeSelect).FirstOrDefault();

        return new Faker<Activity.Domain.Models.Activity>()
            .RuleFor(t => t.Id, f => ActivityId.Of(Guid.NewGuid()))
            .RuleFor(t => t.ThemeId, f => ThemeId.Of(themeId))
            .RuleFor(t => t.Name, f => ActivityName.Of(f.Name.JobType()))
            .RuleFor(t => t.Details, f => ActivityDetails.Of(f.Lorem.Sentence(), f.Internet.Url(), f.Date.Future()))
            .RuleFor(t => t.CreatedBy, f => f.Name.FullName())
            .RuleFor(t => t.CreatedAt, f => f.Date.Future())
            .RuleFor(t => t.LastModifiedBy, f => f.Name.FullName())
            .RuleFor(t => t.LastModified, f => f.Date.Future());
    }

    public static List<Activity.Domain.Models.Theme> GenerateActivityThemes(int count)
    {
        return GenerateActivityTheme().Generate(count);
    }

    public static List<Activity.Domain.Models.Activity> GenerateActivities(List<Guid> themeIds, Faker faker)
    {
        var count = faker.Random.Int(1, 20);

        return GenerateActivity(themeIds, faker).Generate(count);
    }
}
