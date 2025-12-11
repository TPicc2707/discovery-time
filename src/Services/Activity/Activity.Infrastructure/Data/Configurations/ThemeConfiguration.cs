namespace Activity.Infrastructure.Data.Configurations;

public class ThemeConfiguration : IEntityTypeConfiguration<Theme>
{
    public void Configure(EntityTypeBuilder<Theme> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasConversion(
                themeId => themeId.Value,
                dbId => ThemeId.Of(dbId));

        builder.Property(t => t.Name).IsRequired();
    }
}
