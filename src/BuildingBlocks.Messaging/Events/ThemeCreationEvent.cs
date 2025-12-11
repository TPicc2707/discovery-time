namespace BuildingBlocks.Messaging.Events;

public record class ThemeCreationEvent : IntegrationEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}
