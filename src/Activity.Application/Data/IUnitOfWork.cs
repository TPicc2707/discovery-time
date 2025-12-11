namespace Activity.Application.Data;

public interface IUnitOfWork
{
    IThemeRepository Theme { get; }

    Task<int> Complete(CancellationToken cancellationToken);
}
