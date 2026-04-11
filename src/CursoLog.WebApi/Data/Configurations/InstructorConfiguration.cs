using CursoLog.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoLog.WebApi.Data.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(i => i.Bio)
            .HasMaxLength(500);

        builder.Property(i => i.ProfileUrl)
            .HasMaxLength(200);
    }
}
