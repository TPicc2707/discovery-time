namespace Activity.Application.Activities.Commands.DeleteActivity;

public class DeleteActivityHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<DeleteActivityCommand, DeleteActivityResult>
{
    public async Task<DeleteActivityResult> Handle(DeleteActivityCommand command, CancellationToken cancellationToken)
    {
        var activityId = ActivityId.Of(command.Id);

        var activity = await unitOfWork.Theme.GetThemeActivityByIdAsync(activityId, cancellationToken);

        if (activity is null)
            throw new ActivityNotFoundException(command.Id);

        unitOfWork.Theme.RemoveThemeActivity(activity);

        await unitOfWork.Complete(cancellationToken);

        return new DeleteActivityResult(true);
    }
}
