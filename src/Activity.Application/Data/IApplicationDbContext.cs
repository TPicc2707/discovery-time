namespace Activity.Application.Data;

public interface IApplicationDbContext
{
    DbSet<Theme> Themes { get; }
    DbSet<Domain.Models.Activity> Activities { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
