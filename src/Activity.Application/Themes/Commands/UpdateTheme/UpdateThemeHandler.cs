namespace Activity.Application.Themes.Commands.UpdateTheme;

public class UpdateThemeHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateThemeCommand, UpdateThemeResult>
{
    public async Task<UpdateThemeResult> Handle(UpdateThemeCommand command, CancellationToken cancellationToken)
    {
        var themeId = ThemeId.Of(command.Theme.Id);

        var theme = await unitOfWork.Theme.GetByIdAsync(themeId, cancellationToken);

        if (theme is null)
            throw new ThemeNotFoundException(command.Theme.Id);

        UpdateThemeWithNewValues(theme, command.Theme);

        unitOfWork.Theme.Update(theme);

        await unitOfWork.Complete(cancellationToken);

        return new UpdateThemeResult(true);
    }

    private void UpdateThemeWithNewValues(Theme theme, ThemeDto themeDto)
    {
        theme.Update(themeDto.Name);
    }
}
