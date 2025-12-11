namespace Activity.Domain.Events;

public record ThemeCreateEvent(Theme theme) : IDomainEvent;

