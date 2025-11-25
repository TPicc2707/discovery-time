namespace Theme.API.Themes.CreateTheme;

public record CreateThemeCommand(string Name, int Number, string Letter, DateTime StartDate, DateTime EndDate, string CreatedBy, string ModifiedBy)
    : ICommand<CreateThemeResult>;

public record CreateThemeResult(Guid Id);

public class CreateThemeCommandValidator : AbstractValidator<CreateThemeCommand>
{
    public CreateThemeCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required")
            .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");
        RuleFor(x => x.Number).NotEmpty().WithMessage("Number is required")
            .ExclusiveBetween(0, 100).WithMessage("Number must be between 0 and 100");
        RuleFor(x => x.Letter).NotEmpty().WithMessage("Letter is required")
            .Length(2).WithMessage("Letter must be 2 characters");
        RuleFor(x => x.StartDate).NotEmpty().WithMessage("Start Date is required.");
        RuleFor(x => x.EndDate).NotEmpty().WithMessage("End Date is required.");
        RuleFor(x => x.CreatedBy).NotEmpty().WithMessage("Created By is required.");
        RuleFor(x => x.ModifiedBy).NotEmpty().WithMessage("Modified By is required.");
    }
}

public class CreateThemeHandler
    (IDocumentSession documentSession)
    : ICommandHandler<CreateThemeCommand, CreateThemeResult>
{
    public async Task<CreateThemeResult> Handle(CreateThemeCommand command, CancellationToken cancellationToken)
    {
        var theme = new Models.Theme()
        {
            Name = command.Name,
            Number = command.Number,
            Letter = command.Letter,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            CreatedDate = DateTime.Now,
            CreatedBy = command.CreatedBy,
            ModifiedDate = DateTime.Now,
            ModifiedBy = command.ModifiedBy
        };

        documentSession.Store(theme);
        await documentSession.SaveChangesAsync(cancellationToken);

        // Uncomment When Messaging service is created
        //var eventMessage = league.Adapt<ThemeCreationEvent>();

        //await publishEndpoint.Publish(eventMessage, cancellationToken);

        return new CreateThemeResult(theme.Id);
    }
}
