using Microsoft.EntityFrameworkCore;
using webEscuela.Application.Interfaces;
using webEscuela.Application.Services;
using webEscuela.Domain.Repositories;         
using webEscuela.Infrastructure.Data;
using webEscuela.Infrastructure.Repositories; 

var builder = WebApplication.CreateBuilder(args);

// ✅ Conexión a base de datos
var connection = builder.Configuration.GetConnectionString("ConnectionDefault");
builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));

// ✅ Inyección de dependencias
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// ✅ Swagger / OpenAPI
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// ✅ Swagger solo en desarrollo
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers(); // ✅ Necesario para que los endpoints funcionen

app.Run();