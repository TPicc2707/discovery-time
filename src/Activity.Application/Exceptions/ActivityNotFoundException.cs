namespace Activity.Application.Exceptions;

public class ActivityNotFoundException : NotFoundException
{
    public ActivityNotFoundException(Guid Id) : base("Activity", Id)
    {
        
    }
}
