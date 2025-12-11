namespace Activity.Application.Themes.Commands.DeleteTheme;

public record DeleteThemeCommand(Guid Id)
    : ICommand<DeleteThemeResult>;

public record DeleteThemeResult(bool IsSuccess);

public class DeleteThemeCommandValidator : AbstractValidator<DeleteThemeCommand>
{
    public DeleteThemeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required.");
    }
}
