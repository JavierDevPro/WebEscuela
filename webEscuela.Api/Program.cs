using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using webEscuela.Application.Interfaces;
using webEscuela.Application.Services;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Interfaces;
using webEscuela.Domain.Repositories;
using webEscuela.Infrastructure.Data;
using webEscuela.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ======================
// 🔹 CONFIGURACIÓN GENERAL
// ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi(); // solo una vez

// ======================
// 🔹 BASE DE DATOS
// ======================
var connection = builder.Configuration.GetConnectionString("ConnectionDefault");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));

// ======================
// 🔹 CONFIGURACIÓN DE JWT
// ======================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero, // Sin margen de tiempo extra
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            )
        };
    });

builder.Services.AddAuthorization();

// ======================
// 🔹 INYECCIÓN DE DEPENDENCIAS
// ======================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
// ✅ Inyección de dependencias
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();


// Repositorios
builder.Services.AddScoped<IRoleRepository<Role>, RoleRepository>();

// Servicios de aplicación
builder.Services.AddScoped<IRoleService, RoleService>();

// Servicio Autenticacion
builder.Services.AddScoped<AuthService>();

// ======================
// 🔹 CONSTRUCCIÓN DE LA APLICACIÓN
// ======================
var app = builder.Build();

// ======================
// 🔹 MIDDLEWARES
// ======================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// HTTPS y controladores
app.UseHttpsRedirection();

// Si luego agregas autenticación/autorización, colócalo aquí:
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new 
    { 
        status = "healthy", 
        timestamp = DateTime.UtcNow,
        version = "1.0.0"
    }))
    .AllowAnonymous();

app.Run();