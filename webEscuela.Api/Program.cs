using Microsoft.EntityFrameworkCore;
using webEscuela.Application.Interfaces;
using webEscuela.Application.Services;
using webEscuela.Domain.Interfaces;
using webEscuela.Infrastructure.Data;
using webEscuela.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("ConnectionDefault");
builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();     


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();