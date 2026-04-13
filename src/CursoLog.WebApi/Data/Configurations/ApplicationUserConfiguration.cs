using CursoLog.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoLog.WebApi.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ProfilePictureUrl)
            .HasMaxLength(200);

        builder.Property(x => x.Bio)
            .HasMaxLength(200);

        builder.HasIndex(x => x.RefreshToken).IsUnique();

        builder.Property(x => x.RefreshToken).HasMaxLength(100);
    }
}
