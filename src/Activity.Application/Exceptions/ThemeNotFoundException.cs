namespace Activity.Application.Exceptions;

public class ThemeNotFoundException : NotFoundException
{
    public ThemeNotFoundException(Guid Id) : base("Theme", Id)
    {

    }
}
