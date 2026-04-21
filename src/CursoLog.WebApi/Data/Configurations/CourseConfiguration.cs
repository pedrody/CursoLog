using CursoLog.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CursoLog.WebApi.Data.Configurations;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Description)
            .HasMaxLength(10000);
        
        builder.Property(c => c.Url)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.PriceRange)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.DurationRange)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.Language)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(200);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.Platform)
            .WithMany(p => p.Courses)
            .HasForeignKey(c => c.PlatformId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(c => c.Technologies)
            .WithMany(t => t.Courses)
            .UsingEntity(j => j.ToTable("CourseTechnologies"));
    }
}
