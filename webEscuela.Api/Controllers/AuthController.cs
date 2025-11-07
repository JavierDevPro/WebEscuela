using Microsoft.AspNetCore.Mvc;
using webEscuela.Application.Dtos.AuthenticationDto;
using webEscuela.Application.Services;

namespace webEscuela.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        //  REGISTRO (cualquiera puede registrarse con el rol que elija)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);

            if (result == null)
            {
                return BadRequest(new { message = "Username o Email ya están en uso" });
            }

            return Ok(result);
        }

        //  LOGIN (autenticación)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized(new { message = "Usuario o contraseña incorrectos" });
            }

            return Ok(result);
        }
    }
}