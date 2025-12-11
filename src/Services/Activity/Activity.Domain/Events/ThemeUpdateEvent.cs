namespace Activity.Domain.Events;

public record ThemeUpdateEvent(Theme theme) : IDomainEvent;

