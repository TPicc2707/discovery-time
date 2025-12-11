using MassTransit;

namespace Theme.API.Themes.DeleteTheme;

public record DeleteThemeCommand(Guid Id) : ICommand<DeleteThemeResult>;

public record DeleteThemeResult(bool IsSuccess);

public class DeleteThemeCommandValidator : AbstractValidator<DeleteThemeCommand>
{
    public DeleteThemeCommandValidator()
    {
        RuleFor(x =>  x.Id).NotEmpty().WithMessage("Theme Id is required");
    }
}

public class DeleteThemeHandler
    (IDocumentSession documentSession, IPublishEndpoint publishEndpoint)
    : ICommandHandler<DeleteThemeCommand, DeleteThemeResult>
{
    public async Task<DeleteThemeResult> Handle(DeleteThemeCommand command, CancellationToken cancellationToken)
    {
        documentSession.Delete<Models.Theme>(command.Id);
        await documentSession.SaveChangesAsync(cancellationToken);

        var eventMessage = new ThemeDeletionEvent()
        {
            Id = command.Id
        };

        await publishEndpoint.Publish(eventMessage, cancellationToken);

        return new DeleteThemeResult(true);
    }
}
