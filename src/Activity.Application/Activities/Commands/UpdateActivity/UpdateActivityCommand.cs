namespace Activity.Application.Activities.Commands.UpdateActivity;

public record UpdateActivityCommand(ActivityDto Activity) : ICommand<UpdateActivityResult>;

public record UpdateActivityResult(bool IsSuccess);

public class UpdateActivityCommandValidator : AbstractValidator<UpdateActivityCommand>
{
    public UpdateActivityCommandValidator()
    {
        RuleFor(x => x.Activity.Id).NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Activity.Name).NotEmpty().WithMessage("Name is required").MaximumLength(50).WithMessage("Name can no more than 50 characters");
        RuleFor(x => x.Activity.ThemeId).NotEmpty().WithMessage("Theme Id is required");
        RuleFor(x => x.Activity.Details.Url).NotEmpty().WithMessage("Url is required").MaximumLength(500).WithMessage("Name can no more than 500 characters");
        RuleFor(x => x.Activity.Details.Date).NotEmpty().WithMessage("Date is required");

    }
}
