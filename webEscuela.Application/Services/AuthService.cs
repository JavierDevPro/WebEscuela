using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using webEscuela.Application.Dtos.AuthenticationDto;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Interfaces;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace webEscuela.Application.Services;

public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    // REGISTRO DE USUARIO
    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        // Validar si el username ya existe
        if (await _userRepository.ExistsByUserNameAsync(dto.UserName))
        {
            return null; // Username ya existe
        }

        // Validar si el email ya existe
        if (await _userRepository.ExistsByEmailAsync(dto.Email))
        {
            return null; // Email ya existe
        }

        // Hashear contraseña con BCrypt
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

        // Crear usuario
        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = passwordHash,
            RoleId = dto.RoleId,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);
        
        // Obtener el usuario con el Role incluido
        var userWithRole = await _userRepository.GetByIdAsync(createdUser.Id);
        
        if (userWithRole == null) return null;

        // Generar token JWT
        var token = GenerateJwtToken(userWithRole);
        
        return new AuthResponseDto
        {
            Token = token,
            UserName = userWithRole.UserName,
            Role = userWithRole.Role.Name,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    //  LOGIN (Autenticación)
    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        // Buscar usuario por username
        var user = await _userRepository.GetByUserNameAsync(dto.UserName);
        
        if (user == null) return null;

        // Verificar contraseña con BCrypt
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        
        if (!isPasswordValid) return null;

        // Generar token JWT
        var token = GenerateJwtToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserName = user.UserName,
            Role = user.Role.Name,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }

    // GENERAR TOKEN JWT
    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.Name), // "Admin" o "User"
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}