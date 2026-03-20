using Electrical_Equipment_Rental.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Electrical_Equipment_Rental.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductImage> ProductImages { get; set; }
    public DbSet<ProductUnit> ProductUnits { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<DeviceLocation> DeviceLocations { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Repair> Repairs { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ProductUnit>()
            .HasIndex(u => u.SerialNumber).IsUnique();

        builder.Entity<ProductUnit>()
            .HasOne(u => u.CurrentLocation)
            .WithMany(l => l.Units)
            .HasForeignKey(u => u.CurrentLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Rental>()
            .HasOne(r => r.StartLocation)
            .WithMany()
            .HasForeignKey(r => r.StartLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rental>()
            .HasOne(r => r.EndLocation)
            .WithMany()
            .HasForeignKey(r => r.EndLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Rental>()
            .HasOne(r => r.User)
            .WithMany(u => u.Rentals)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Repair>()
            .HasOne(r => r.Technician)
            .WithMany()
            .HasForeignKey(r => r.TechnicianId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Entity<Product>()
            .Property(p => p.PricePerHour).HasColumnType("decimal(10,2)");
        builder.Entity<Product>()
            .Property(p => p.DepositAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Rental>()
            .Property(r => r.RatePerHour).HasColumnType("decimal(10,2)");
        builder.Entity<Rental>()
            .Property(r => r.DepositAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Rental>()
            .Property(r => r.TotalAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Rental>()
            .Property(r => r.FineAmount).HasColumnType("decimal(10,2)");
        builder.Entity<Payment>()
            .Property(p => p.Amount).HasColumnType("decimal(10,2)");
        builder.Entity<Repair>()
            .Property(r => r.Cost).HasColumnType("decimal(10,2)");
    }
}
