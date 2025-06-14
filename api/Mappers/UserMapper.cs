using System;
using api.Dtos.User;
using api.Models;

namespace api.Mappers
{
    // Provides mapping methods between User models and DTOs
    public static class UserMappers
    {
        // Converts a User model to a User DTO
        public static UserDto ToDto(this User userModel)
        {
            return new UserDto
            {
                id = userModel.Id,
                userName = userModel.UserName!,
                role = userModel.Role,
                email = userModel.Email!,
                birthday = userModel.Birthday,
                registrationDate = userModel.RegistrationDate
            };
        }

        // Converts a CreateUserRequestDto to a User model
        // Sets registrationDate to current UTC time automatically
        public static User ToUserFromCreateDto(this CreateUserRequestDto userDto)
        {
            return new User
            {
                UserName = userDto.userName,
                Role = userDto.role,
                Email = userDto.email,
                Birthday = userDto.birthday,
                RegistrationDate = DateTime.Now
            };
        }
    }
}