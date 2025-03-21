using api.Dtos.UserTask;
using api.Models;

namespace api.Mappers
{
    public static class TaskMapper
    {
        public static UserTaskDto ToDto(this UserTask task)
        {
            return new UserTaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted,
                DueDate = task.DueDate
            };
        }

        public static UserTask ToTaskFromCreateDto(this CreateUserTaskRequestDto taskDto)
        {
            return new UserTask
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                IsCompleted = false
            };
        }
    }
}
