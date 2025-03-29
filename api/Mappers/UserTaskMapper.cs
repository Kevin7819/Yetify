using api.Dtos.UserTask;
using api.Models;

namespace api.Mappers
{
    // Mapper for converting between UserTask models and DTOs
    public static class TaskMapper
    {
        // Converts UserTask model to DTO (for API responses)
        public static UserTaskDto ToDto(this UserTask task)
        {
            return new UserTaskDto
            {
                id = task.id,
                idUser = task.idUser,        // User ID
                idCourse = task.idCourse,   // Course ID
                description = task.description,  // Task description
                dueDate = task.dueDate,     // Deadline date
                status = task.status         // Current status
            };
        }

        // Converts Create DTO to UserTask model (for database operations)
        public static UserTask ToTaskFromCreateDto(this CreateUserTaskRequestDto taskDto)
        {
            return new UserTask
            {
                idUser = taskDto.idUser,     // Required user ID
                idCourse = taskDto.idCourse, // Required course ID
                description = taskDto.description, // Task details
                dueDate = taskDto.dueDate,   // Due date
                status = taskDto.status      // Initial status
            };
        }
    }
}