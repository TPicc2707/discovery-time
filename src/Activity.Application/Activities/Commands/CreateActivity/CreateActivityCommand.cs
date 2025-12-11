namespace Activity.Application.Activities.Commands.CreateActivity;

public record CreateActivityCommand(ActivityDto Activity)
    : ICommand<CreateActivityResult>;

public record CreateActivityResult(Guid Id);

public class CreateActivityCommandValidator : AbstractValidator<CreateActivityCommand>
{
    public CreateActivityCommandValidator()
    {
        RuleFor(x => x.Activity.Name).NotEmpty().WithMessage("Name is required").MaximumLength(50).WithMessage("Name can no more than 50 characters");
        RuleFor(x => x.Activity.ThemeId).NotEmpty().WithMessage("Theme Id is required");
        RuleFor(x => x.Activity.Details.Url).NotEmpty().WithMessage("Url is required").MaximumLength(500).WithMessage("Name can no more than 500 characters");
        RuleFor(x => x.Activity.Details.Date).NotEmpty().WithMessage("Date is required");
    }
}
