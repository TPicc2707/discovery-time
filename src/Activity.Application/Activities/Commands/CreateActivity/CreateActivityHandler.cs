namespace Activity.Application.Activities.Commands.CreateActivity;

public class CreateActivityHandler(IUnitOfWork unitOfWork)
    : ICommandHandler<CreateActivityCommand, CreateActivityResult>
{
    public async Task<CreateActivityResult> Handle(CreateActivityCommand command, CancellationToken cancellationToken)
    {
        var activity = CreateNewActivity(command.Activity);

        var createdActivity = await unitOfWork.Theme.CreateThemeActivityAsync(activity);

        await unitOfWork.Complete(cancellationToken);

        return new CreateActivityResult(createdActivity.Id.Value);
    }

    private Domain.Models.Activity CreateNewActivity(ActivityDto activityDto)
    {
        var activityDetails = ActivityDetails.Of(activityDto.Details.Description, activityDto.Details.Url, activityDto.Details.Date);

        var newActivity = Domain.Models.Activity.Create(
            id: ActivityId.Of(Guid.NewGuid()),
            themeId: ThemeId.Of(activityDto.ThemeId),
            name: ActivityName.Of(activityDto.Name),
            details: activityDetails);

        return newActivity;
    }
}
