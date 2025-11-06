using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using webEscuela.Application.DTOs.Students;
using webEscuela.Application.Interfaces;

namespace webEscuela.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        [HttpGet("api/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(StudentCreateDto dto)
        {
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, StudentUpdateDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            return ok ? NoContent() : NotFound();
        }

        [HttpDelete("Delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}