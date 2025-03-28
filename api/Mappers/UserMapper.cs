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
                id = userModel.id,
                userName = userModel.userName,
                role = userModel.role,
                email = userModel.email,
                birthday = userModel.birthday,
                registrationDate = userModel.registrationDate
            };
        }

        // Converts a CreateUserRequestDto to a User model
        // Sets registrationDate to current UTC time automatically
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