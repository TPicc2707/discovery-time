namespace Activity.Application.Dtos;

public record ActivityDto(
    Guid Id,
    Guid ThemeId,
    string Name,
    ActivityDetailsDto Details);