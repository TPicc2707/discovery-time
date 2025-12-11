namespace Activity.Infrastructure.Data.Configurations;

public class ActivityConfiguration : IEntityTypeConfiguration<Domain.Models.Activity>
{
    public void Configure(EntityTypeBuilder<Domain.Models.Activity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id).HasConversion(
                activityId => activityId.Value,
                dbId => ActivityId.Of(dbId));

        builder.HasOne<Theme>()
            .WithMany()
            .HasForeignKey(a => a.ThemeId)
            .IsRequired();

        builder.ComplexProperty(
            a => a.Name, namebuilder =>
            {
                namebuilder.Property(n => n.Value)
                .HasColumnName(nameof(Domain.Models.Activity.Name))
                .HasMaxLength(50)
                .IsRequired();
            });

        builder.ComplexProperty(
            a => a.Details, detailsBuilder =>
            {
                detailsBuilder.Property(d => d.Description)
                     .HasMaxLength(500);

                detailsBuilder.Property(d => d.Url)
                     .HasMaxLength(500)
                     .IsRequired();

                detailsBuilder.Property(d => d.Date)
                        .IsRequired();
            });
    }
}
