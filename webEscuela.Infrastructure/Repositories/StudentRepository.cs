using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Repositories;
using webEscuela.Infrastructure.Data;

namespace webEscuela.Infrastructure.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllAsync()
            => await _context.Students.ToListAsync();

        public async Task<Student?> GetByIdAsync(int id)
            => await _context.Students.FindAsync(id);

        public async Task AddAsync(Student student)
            => await _context.Students.AddAsync(student);

        public async Task UpdateAsync(Student student)
            => _context.Students.Update(student);

        public async Task DeleteAsync(Student student)
            => _context.Students.Remove(student);

        public async Task<bool> SaveChangesAsync()
            => await _context.SaveChangesAsync() > 0;
    }
}