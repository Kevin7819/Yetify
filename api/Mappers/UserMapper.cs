using System.Reflection.Metadata.Ecma335;
using api.Dtos.User;
using api.Models;
namespace api.Mappers

{
    public static class UserMappers
    {
        public static UserDto ToDto(this User userModel)
        {
            return new UserDto
            {
                id = userModel.id,
                userName = userModel.userName,
                role = userModel.role,
                email = userModel.email,
                birthday = userModel.birthday,
                registrationDate = userModel.registrationDate
            };
        }

        public static User ToUserFromCreateDto(this CreateUserRequestDto userDto)
        {
            return new User
            {
                userName = userDto.userName,
                password = userDto.password,
                role = userDto.role,
                email = userDto.email,
                birthday = userDto.birthday,
                registrationDate = DateTime.Now
            };
        }
    }
}