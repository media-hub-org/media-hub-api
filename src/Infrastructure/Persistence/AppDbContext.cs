using MediaHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediaHub.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Device> Devices => Set<Device>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Device>(entity =>
        {
            entity.ToTable("devices");
            entity.HasKey(d => d.Id);
            entity.Property(d => d.Name).IsRequired().HasMaxLength(100);
            entity.Property(d => d.TokenHash).IsRequired();
            entity.HasIndex(d => d.TokenHash).IsUnique();
        });
    }
}