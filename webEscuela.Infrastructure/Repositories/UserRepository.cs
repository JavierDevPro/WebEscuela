using Microsoft.EntityFrameworkCore;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Interfaces;
using webEscuela.Infrastructure.Data;

namespace webEscuela.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllAsync() =>
            await _context.users.Include(u => u.Role).ToListAsync();

        public async Task<User?> GetByIdAsync(int id) =>
            await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User> CreateAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateAsync(int id, User user)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null) return null;

            existing.UserName = string.IsNullOrEmpty(user.UserName) ? existing.UserName : user.UserName;
            existing.Email = string.IsNullOrEmpty(user.Email) ? existing.Email : user.Email;
            existing.PasswordHash = string.IsNullOrEmpty(user.PasswordHash) ? existing.PasswordHash : user.PasswordHash;
            existing.RoleId = user.RoleId == 0 ? existing.RoleId : user.RoleId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}