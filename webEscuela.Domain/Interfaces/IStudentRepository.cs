using System.Collections.Generic;
using System.Threading.Tasks;
using webEscuela.Domain.Entities;

namespace webEscuela.Domain.Repositories
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task AddAsync(Student student);
        Task UpdateAsync(Student student);
        Task DeleteAsync(Student student);
        Task<bool> SaveChangesAsync();
    }
}