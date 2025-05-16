using Microsoft.EntityFrameworkCore;
using QMS_Data_Updater.Domain.Entities;

namespace QMS_Data_Updater.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<UserCertificate> UserCertificates { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserCertificate>(entity =>
        {
            entity.HasKey(e => e.UserCertificateGUID);
            entity.Property(e => e.LicenseNumber).HasMaxLength(100);
            entity.Property(e => e.IssuingCountry).HasMaxLength(100);
            entity.Property(e => e.Grade).HasMaxLength(50);
            entity.Property(e => e.Trainer).HasMaxLength(100);
            entity.Property(e => e.Hours).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.TimeZone).HasMaxLength(100);
            entity.Property(e => e.SuspensionType).HasMaxLength(100);
        });
    }
}