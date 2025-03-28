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
                id = course.id,
                nameCourse = course.nameCourse,
                description = course.description
            };
        }

        public static Course ToCourseFromCreateDto(this CreateCourseRequestDto courseDto)
        {
            return new Course
            {
                nameCourse = courseDto.nameCourse,
                description = courseDto.description
            };
        }  

        
    }
}