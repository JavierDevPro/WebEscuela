using Microsoft.EntityFrameworkCore;
using webEscuela.Domain.Entities;

namespace webEscuela.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Role>  Roles { get; set; }
    public DbSet<User>  Users { get; set; }
    public DbSet<Student>  Students { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Role>()
            .HasMany(f => f.users)
            .WithOne(p => p.Role)
            .HasForeignKey(p => p.RoleId);
        
        modelBuilder.Entity<Student>().HasKey(s => s.Id);
        
        modelBuilder.Entity<Role>().HasKey(r => r.Id);
        
        modelBuilder.Entity<User>().HasKey(u => u.Id);
    }
    
}