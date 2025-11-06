
using webEscuela.Application.Dtos.RoleDtos;
using webEscuela.Application.Interfaces;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Interfaces;

namespace webEscuela.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository<Role> _repository;

        public RoleService(IRoleRepository<Role> repository)
        {
            _repository = repository;
        }

        private RoleDto MapDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };
        }

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await _repository.GetAllRoles();
            return roles.Select(MapDto);
        }
    }
}