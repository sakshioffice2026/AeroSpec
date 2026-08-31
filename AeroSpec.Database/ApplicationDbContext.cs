
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace AeroSpec.Database;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<FanSize> FanSizes => Set<FanSize>();
    public DbSet<FanType> FanTypes => Set<FanType>();
    public DbSet<PerformanceData> PerformanceData => Set<PerformanceData>();
    public DbSet<FanSelection> FanSelections => Set<FanSelection>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<FanSize>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SizeId).IsUnique();
            entity.Property(e => e.SizeId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DiameterIn).HasPrecision(10, 2);
            entity.Property(e => e.CfmScale).HasPrecision(10, 4);
            entity.Property(e => e.SpScale).HasPrecision(10, 4);
            entity.Property(e => e.OutletArea).HasPrecision(10, 2);
        });

        builder.Entity<FanType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TypeId).IsUnique();
            entity.Property(e => e.TypeId).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Label).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Prefix).HasMaxLength(10).IsRequired();
            entity.Property(e => e.SpMod).HasPrecision(10, 4);
            entity.Property(e => e.EffMod).HasPrecision(10, 4);
        });

        builder.Entity<PerformanceData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.FanSizeId, e.Rpm });
            entity.Property(e => e.Volume).HasPrecision(12, 4);
            entity.Property(e => e.StaticPressure).HasPrecision(12, 4);

            entity.HasOne(e => e.FanSize)
                .WithMany(f => f.PerformanceDataSet)
                .HasForeignKey(e => e.FanSizeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<FanSelection>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProjectName);
            entity.HasIndex(e => e.CreatedDate);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.ProjectName).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Tag).HasMaxLength(100);
            entity.Property(e => e.FanType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Arrangement).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SelectedFanId).HasMaxLength(50).IsRequired();

            entity.Property(e => e.RequiredCfm).HasPrecision(12, 2);
            entity.Property(e => e.RequiredSp).HasPrecision(12, 2);
            entity.Property(e => e.SelectedBhp).HasPrecision(12, 2);
            entity.Property(e => e.SelectedEfficiency).HasPrecision(5, 2);
            entity.Property(e => e.DensityRatio).HasPrecision(10, 4);
        });
    }
}