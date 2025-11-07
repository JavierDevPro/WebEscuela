using webEscuela.Domain.Entities;

namespace webEscuela.Domain.Interfaces;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task<User> CreateAsync(User user);
    Task<User?> UpdateAsync(int id, User user);
    Task<bool> DeleteAsync(int id);
    Task<User?> GetByUserNameAsync(string userName);
    Task<bool> ExistsByUserNameAsync(string userName);
    Task<bool> ExistsByEmailAsync(string email);
}