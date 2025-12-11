
using Activity.Application.Themes.Commands.DeleteTheme;

namespace Activity.Application.Themes.Integrations;

public class ThemeActivityDeletionEventHandler
    (ISender sender, ILogger<ThemeActivityDeletionEventHandler> logger)
    : IConsumer<ThemeDeletionEvent>
{
    public async Task Consume(ConsumeContext<ThemeDeletionEvent> context)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

        var command = MapToDeleteThemeCommand(context.Message);

        await sender.Send(command);
    }

    private DeleteThemeCommand MapToDeleteThemeCommand(ThemeDeletionEvent message)
    {
        return new DeleteThemeCommand(message.Id);
    }
}
