namespace webEscuela.Application.Dtos.AuthenticationDto;

public class RegisterDto
{
    public string Token { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public string Password { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
}