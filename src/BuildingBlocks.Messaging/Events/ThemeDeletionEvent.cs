namespace BuildingBlocks.Messaging.Events;

public record ThemeDeletionEvent : IntegrationEvent
{
    public Guid Id { get; set; }

}
