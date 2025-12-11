namespace Activity.Application.Activities.Commands.UpdateActivity;

public class UpdateActivityHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<UpdateActivityCommand, UpdateActivityResult>
{
    public async Task<UpdateActivityResult> Handle(UpdateActivityCommand command, CancellationToken cancellationToken)
    {
        var activityId = ActivityId.Of(command.Activity.Id);
        var activity = await unitOfWork.Theme.GetThemeActivityByIdAsync(activityId, cancellationToken);

        if(activity is null)
            throw new ActivityNotFoundException(command.Activity.Id);

        UpdateActivityWithNewValues(activity, command.Activity);

        unitOfWork.Theme.UpdateThemeActivity(activity);
        await unitOfWork.Complete(cancellationToken);

        return new UpdateActivityResult(true);
    }

    private void UpdateActivityWithNewValues(Domain.Models.Activity activity, ActivityDto activityDto)
    {
        var updatedActivityDetails = ActivityDetails.Of(activityDto.Details.Description, activityDto.Details.Url, activityDto.Details.Date);

        activity.Update(
            themeId: ThemeId.Of(activityDto.ThemeId),
            name: ActivityName.Of(activityDto.Name),
            details: updatedActivityDetails);
    }
}
