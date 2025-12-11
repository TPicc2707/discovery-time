namespace Activity.Application.Themes.Commands.DeleteTheme;

public class DeleteThemeHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteThemeCommand, DeleteThemeResult>
{
    public async Task<DeleteThemeResult> Handle(DeleteThemeCommand command, CancellationToken cancellationToken)
    {
        var themeId = ThemeId.Of(command.Id);
        var theme = await unitOfWork.Theme.GetByIdAsync(themeId, cancellationToken);

        if (theme is null)
            throw new ThemeNotFoundException(command.Id);

        unitOfWork.Theme.Delete(theme);
        await unitOfWork.Complete(cancellationToken);

        return new DeleteThemeResult(true);
    }
}
