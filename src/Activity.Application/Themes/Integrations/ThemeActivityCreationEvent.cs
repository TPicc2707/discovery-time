namespace Activity.Application.Themes.Integrations;

public class ThemeCreationEventHandler
    (ISender sender, ILogger<ThemeCreationEventHandler> logger)
    : IConsumer<ThemeCreationEvent>
{
    public async Task Consume(ConsumeContext<ThemeCreationEvent> context)
    {
        logger.LogInformation("Integration Event handled: {IntegrationEvent}", context.Message.GetType().Name);

        var command = MapToCreateThemeCommand(context.Message);

        await sender.Send(command);
    }

    private CreateThemeCommand MapToCreateThemeCommand(ThemeCreationEvent message)
    {
        var themeDto = new ThemeDto(
            Id: message.Id,
            Name: message.Name);

        return new CreateThemeCommand(themeDto);
    }
}
