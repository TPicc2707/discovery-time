namespace Activity.Application.Themes.Commands.CreateTheme;

public class CreateThemeHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateThemeCommand, CreateThemeResult>
{
    public async Task<CreateThemeResult> Handle(CreateThemeCommand command, CancellationToken cancellationToken)
    {
        var theme = CreateNewTheme(command.Theme);

        var newTheme = await unitOfWork.Theme.AddAsync(theme);

        await unitOfWork.Complete(cancellationToken);

        return new CreateThemeResult(newTheme.Id.Value);
    }

    private Theme CreateNewTheme(ThemeDto themeDto)
    {
        var newTheme = Theme.Create(
            id: ThemeId.Of(themeDto.Id),
            name: themeDto.Name);

        return newTheme;
    }
}
