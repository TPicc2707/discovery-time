namespace BuildingBlocks.Messaging.Events;

public record ThemeUpdatedEvent : IntegrationEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
