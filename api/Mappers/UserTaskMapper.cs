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
                idUser = task.idUser,
                idCourse = task.idCourse,
                description = task.description,
                dueDate = task.dueDate,
                status = task.status
            };
        }

        public static UserTask ToTaskFromCreateDto(this CreateUserTaskRequestDto taskDto)
        {
            return new UserTask
            {
                idUser = taskDto.idUser,
                idCourse = taskDto.idCourse,
                description = taskDto.description,
                dueDate = taskDto.dueDate,
                status = taskDto.status
            };
        }
    }
}
