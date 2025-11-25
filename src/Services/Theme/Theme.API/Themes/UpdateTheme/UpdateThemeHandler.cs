namespace Theme.API.Themes.UpdateTheme;

public record UpdateThemeCommand(Guid Id, string Name, int Number, string Letter, DateTime StartDate, DateTime EndDate, string ModifiedBy)
    : ICommand<UpdateThemeResult>;

public record UpdateThemeResult(bool IsSuccess);

public class UpdateThemeCommandValidator : AbstractValidator<UpdateThemeCommand>
{
    public UpdateThemeCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Theme Id is required");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
        .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");
        RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required")
            .ExclusiveBetween(0, 100).WithMessage("Number must be between 0 and 100");
        RuleFor(x => x.Letter).NotEmpty().WithMessage("Letter is required")
            .Length(2).WithMessage("Letter must be 2 characters");
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start Date is required.");
        RuleFor(x => x.EndDate).NotEmpty().WithMessage("End Date is required.");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("Modified By is required.");
    }
}

public class UpdateThemeHandler
    (IDocumentSession documentSession)
    : ICommandHandler<UpdateThemeCommand, UpdateThemeResult>
{
    public async Task<UpdateThemeResult> Handle(UpdateThemeCommand command, CancellationToken cancellationToken)
    {
        var theme = await documentSession.LoadAsync<Models.Theme>(command.Id, cancellationToken);

        if (theme is null)
            throw new ThemeNotFoundException(command.Id);

        theme.Name = command.Name;
        theme.Number = command.Number;
        theme.Letter = command.Letter;
        theme.StartDate = command.StartDate;
        theme.EndDate = command.EndDate;
        theme.ModifiedDate = DateTime.Now;
        theme.ModifiedBy = command.ModifiedBy;

        documentSession.Update(theme);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Uncomment When Messaging service is created
        //var eventMessage = league.Adapt<LeagueUpdatedEvent>();

        //await publishEndpoint.Publish(eventMessage, cancellationToken);

        return new UpdateThemeResult(true);

    }
}
