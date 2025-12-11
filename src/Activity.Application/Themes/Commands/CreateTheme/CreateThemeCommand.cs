namespace Activity.Application.Themes.Commands.CreateTheme;

public record CreateThemeCommand(ThemeDto Theme)
    : ICommand<CreateThemeResult>;

public record CreateThemeResult(Guid Id);

public class CreateThemeCommandValidator : AbstractValidator<CreateThemeCommand>
{
    public CreateThemeCommandValidator()
    {
        RuleFor(x => x.Theme.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Theme.Name).NotEmpty().WithMessage("Name is required.");
    }
}
