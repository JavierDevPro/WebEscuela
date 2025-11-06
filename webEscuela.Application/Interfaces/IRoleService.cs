
using webEscuela.Application.Dtos.RoleDtos;

namespace webEscuela.Application.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    }
}