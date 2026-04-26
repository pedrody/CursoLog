using CursoLog.WebApi.DTOs.Course;
using CursoLog.WebApi.Models;

namespace CursoLog.WebApi.Extensions.Mappings;

public static class CourseMappingExtensions
{
    public static CourseDetailResponse ToCourseDetailResponse(this Course course)
    {
        return new CourseDetailResponse(
            course.Id,
            course.Name,
            course.Description,
            course.Url,
            course.PriceRange,
            course.DurationRange,
            course.Language,
            course.ThumbnailUrl,
            course.InstructorId,
            course.Instructor.Name,
            course.PlatformId,
            course.Platform.Name,
            course.CategoryId,
            course.Category.Name,
            course.Reviews.Count,
            CalculateAverageRating(course.Reviews),
            course.Technologies.Select(t => t.ToTechnologyResponse())
        );
    }

    public static CourseListResponse ToCourseListResponse(this Course course)
    {
        return new CourseListResponse(
            course.Id, 
            course.Name, 
            course.ThumbnailUrl, 
            course.Reviews.Count,
            CalculateAverageRating(course.Reviews));
    }

    // TODO: search and implement how to calculate average rating
    private static decimal CalculateAverageRating(ICollection<Review> reviews)
    {
        return reviews.Count != 0
                ? reviews.Average(r => r.Rating)
                : 0;
    }
}
