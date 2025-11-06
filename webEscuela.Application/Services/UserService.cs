using webEscuela.Application.DTOs;
using webEscuela.Application.Interfaces;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace webEscuela.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;

        public UserService(IUserRepository repo)
        {
            _repo = repo;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        public async Task<IEnumerable<UserDto>> GetAllAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? "",
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserDto?> GetByIdAsync(int id)
        {
            var u = await _repo.GetByIdAsync(id);
            if (u == null) return null;

            return new UserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                RoleId = u.RoleId,
                RoleName = u.Role?.Name ?? "",
                CreatedAt = u.CreatedAt
            };
        }

        public async Task<UserDto> CreateAsync(CreateUserDto dto)
        {
            var entity = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password),
                RoleId = dto.RoleId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repo.CreateAsync(entity);

            return new UserDto
            {
                Id = created.Id,
                UserName = created.UserName,
                Email = created.Email,
                RoleId = created.RoleId,
                RoleName = created.Role?.Name ?? "",
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto)
        {
            var userToUpdate = new User
            {
                UserName = dto.UserName ?? "",
                Email = dto.Email ?? "",
                PasswordHash = dto.Password != null ? HashPassword(dto.Password) : "",
                RoleId = dto.RoleId ?? 0
            };

            var updated = await _repo.UpdateAsync(id, userToUpdate);
            if (updated == null) return null;

            return new UserDto
            {
                Id = updated.Id,
                UserName = updated.UserName,
                Email = updated.Email,
                RoleId = updated.RoleId,
                RoleName = updated.Role?.Name ?? "",
                CreatedAt = updated.CreatedAt
            };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
