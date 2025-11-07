using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
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
//  CONFIGURACIÓN GENERAL
// ======================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "WebEscuela API", 
        Version = "v1",
        Description = "API para gestión escolar con autenticación JWT"
    });

    //  Configuración de autenticación Bearer (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer.\n\n" +
                      "Ejemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            new string[] {}
        }
    });
});

// ======================
//  CONFIGURACIÓN DE CORS
// ======================
var corsPolicyName = "AllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());// si el front envía cookies o auth headers
        });
});

// ======================
//  BASE DE DATOS
// ======================
var connection = builder.Configuration.GetConnectionString("ConnectionDefault");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));

// ======================
//  CONFIGURACIÓN DE JWT
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
//  INYECCIÓN DE DEPENDENCIAS
// ======================

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
//  Inyección de dependencias
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();


// Repositorios
builder.Services.AddScoped<IRoleRepository<Role>, RoleRepository>();

// Servicios de aplicación
builder.Services.AddScoped<IRoleService, RoleService>();

// Servicio Autenticacion
builder.Services.AddScoped<AuthService>();

// ======================
//  CONSTRUCCIÓN DE LA APLICACIÓN
// ======================
var app = builder.Build();

// ======================
//  MIDDLEWARES
// ======================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebEscuela API v1");
    c.RoutePrefix = "api-docs"; // Se accederá en /api-docs
});

// HTTPS y controladores
app.UseHttpsRedirection();

app.UseCors("AllowAll");

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

