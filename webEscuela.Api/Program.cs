using Microsoft.EntityFrameworkCore;
using webEscuela.Application.Interfaces;
using webEscuela.Application.Services;
using webEscuela.Domain.Entities;
using webEscuela.Domain.Interfaces;
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
// 🔹 INYECCIÓN DE DEPENDENCIAS
// ======================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
// Repositorios
builder.Services.AddScoped<IRoleRepository<Role>, RoleRepository>();

// Servicios de aplicación
builder.Services.AddScoped<IRoleService, RoleService>();

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
app.UseAuthorization();

app.MapControllers();

app.Run();