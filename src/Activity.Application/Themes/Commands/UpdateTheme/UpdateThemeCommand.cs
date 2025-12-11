namespace Activity.Application.Themes.Commands.UpdateTheme;

public record UpdateThemeCommand(ThemeDto Theme)
    : ICommand<UpdateThemeResult>;

public record UpdateThemeResult(bool IsSuccess);

public class UpdateThemeCommandValidator : AbstractValidator<UpdateThemeCommand>
{
    public UpdateThemeCommandValidator()
    {
        RuleFor(x => x.Theme.Id).NotEmpty().WithMessage("Id is required.");
        RuleFor(x => x.Theme.Name).NotEmpty().WithMessage("Name is required.");
    }
}