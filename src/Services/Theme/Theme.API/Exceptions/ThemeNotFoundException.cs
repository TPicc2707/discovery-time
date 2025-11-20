namespace Theme.API.Exceptions;

public class ThemeNotFoundException : NotFoundException
{
    public ThemeNotFoundException(Guid Id) : base("Theme", Id)
    {
        
    }
}
