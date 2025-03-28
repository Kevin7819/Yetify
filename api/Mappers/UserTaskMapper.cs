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
                id = task.id,
                title = task.title,
                description = task.description,
                isCompleted = task.isCompleted,
                dueDate = task.dueDate
            };
        }

        public static UserTask ToTaskFromCreateDto(this CreateUserTaskRequestDto taskDto)
        {
            return new UserTask
            {
                title = taskDto.title,
                description = taskDto.description,
                dueDate = taskDto.dueDate,
                isCompleted = false
            };
        }
    }
}
