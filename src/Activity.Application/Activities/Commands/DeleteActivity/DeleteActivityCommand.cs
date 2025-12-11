namespace Activity.Application.Activities.Commands.DeleteActivity;

public record DeleteActivityCommand(Guid Id) : ICommand<DeleteActivityResult>;

public record DeleteActivityResult(bool IsSuccess);

public class DeleteActivityValidator : AbstractValidator<DeleteActivityCommand>
{
    public DeleteActivityValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
    }
}
