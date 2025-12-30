namespace Activity.Application.Activities.Commands.DeleteActivity;

public record DeleteActivityCommand(Guid Id) : ICommand<DeleteActivityResult>;

public record DeleteActivityResult(bool IsSuccess);

public class DeleteActivityCommandValidator : AbstractValidator<DeleteActivityCommand>
{
    public DeleteActivityCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
    }
}
