using CursoLog.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoLog.WebApi.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        // TODO: Add unique index on (CourseId, UserId) after configuring ASP.NET Identity
        // This will ensure that a user can only leave one review per course

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Rating)
            .IsRequired()
            .HasColumnType("decimal(2,1)");

        builder.Property(r => r.Comment)
            .HasMaxLength(1000);

        builder.HasOne(r => r.Course)
            .WithMany(c => c.Reviews)
            .HasForeignKey(r => r.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
