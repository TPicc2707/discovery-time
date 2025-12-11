namespace Activity.Application.Themes.Integrations;

public class ThemeActivityUpdatedEventHandler
    (ISender sender, ILogger<ThemeActivityUpdatedEventHandler> logger)
    : IConsumer<ThemeUpdatedEvent>
{
    public async Task Consume(ConsumeContext<ThemeUpdatedEvent> context)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

        var command = MapToUpdateThemeCommand(context.Message);

        await sender.Send(command);
    }

    private UpdateThemeCommand MapToUpdateThemeCommand(ThemeUpdatedEvent message)
    {
        var themeDto = new ThemeDto(
            Id: message.Id,
            Name: message.Name);

        return new UpdateThemeCommand(themeDto);
    }
}
