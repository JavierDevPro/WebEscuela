using webEscuela.Domain.Entities;

namespace webEscuela.Domain.Interfaces;

public interface IRoleRepository
{
    Task<IEnumerable<Role>> GetAllRoles();
    
}