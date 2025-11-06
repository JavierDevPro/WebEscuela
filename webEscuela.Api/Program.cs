using Microsoft.EntityFrameworkCore;
using webEscuela.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

var connection = builder.Configuration.GetConnectionString("ConnectionDefault");
builder.Services.AddDbContext<AppDbContext>(
    options =>
        options.UseMySql(connection, MySqlServerVersion.AutoDetect(connection)));


// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();