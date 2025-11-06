using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webEscuela.Application.DTOs.Students;
using webEscuela.Application.Interfaces;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Repositories;

namespace webEscuela.Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;

        public StudentService(IStudentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllAsync()
        {
            var students = await _repository.GetAllAsync();
            return students.Select(s => new StudentResponseDto
            {
                Id = s.Id,
                FullName = $"{s.FirstName} {s.LastName}",
                Email = s.Email
            });
        }

        public async Task<StudentResponseDto?> GetByIdAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return null;

            return new StudentResponseDto
            {
                Id = student.Id,
                FullName = $"{student.FirstName} {student.LastName}",
                Email = student.Email
            };
        }

        public async Task<StudentResponseDto> CreateAsync(StudentCreateDto dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(student);
            await _repository.SaveChangesAsync();

            return new StudentResponseDto
            {
                Id = student.Id,
                FullName = $"{student.FirstName} {student.LastName}",
                Email = student.Email
            };
        }

        public async Task<bool> UpdateAsync(int id, StudentUpdateDto dto)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return false;

            student.FirstName = dto.FirstName;
            student.LastName = dto.LastName;
            student.Email = dto.Email;
            student.UpdatedAt = DateTime.UtcNow;

            _repository.UpdateAsync(student);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var student = await _repository.GetByIdAsync(id);
            if (student == null) return false;

            _repository.DeleteAsync(student);
            return await _repository.SaveChangesAsync();
        }
    }
}
