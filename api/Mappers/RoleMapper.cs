using System.Reflection.Metadata.Ecma335;
using api.Dtos.Role;
using api.Models;


namespace api.Mappers
{
    public static class RoleMapper
    {
        public static RoleDto ToDto(this Role role)
        {
            return new RoleDto
            {
                idRole = role.idRole,
                name = role.name
            };
        }

        public static Role ToCourseFromCreateDto(this CreateRoleRequestDto roleDto)
        {
            return new Role
            {
                name = roleDto.name
            };
        }
    }
}

