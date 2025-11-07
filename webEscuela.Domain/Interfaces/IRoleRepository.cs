using webEscuela.Domain.Entities;

namespace webEscuela.Domain.Interfaces;

public interface IRoleRepository<T>
{
    Task<IEnumerable<Role>> GetAllRoles();
    
}