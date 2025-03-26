using System.Reflection.Metadata.Ecma335;
using api.Dtos.Course;
using api.Models;

namespace api.Mappers
{

    public static class CourseMapper
    {
        public static CourseDto ToDto (this Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                NameCourse = course.NameCourse,
                Description = course.Description
            };
        }

        public static Course ToCourseFromCreateDto(this CreateCourseRequestDto courseDto)
        {
            return new Course
            {
                NameCourse = courseDto.NameCourse,
                Description = courseDto.Description
            };
        }  

         public static void UpdateFromDto(this Course course, UpdateCourseRequestDto courseDto)
        {
            if (!string.IsNullOrWhiteSpace(courseDto.NameCourse))
                course.NameCourse = courseDto.NameCourse;

            if (!string.IsNullOrWhiteSpace(courseDto.Description))
                course.Description = courseDto.Description;
        } 
    }
}