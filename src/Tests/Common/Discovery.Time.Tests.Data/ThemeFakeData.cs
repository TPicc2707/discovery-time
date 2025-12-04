using Bogus;

namespace Discovery.Time.Tests.Data;

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
}
