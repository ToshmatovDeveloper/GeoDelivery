using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracking.Domain.Entities;

namespace Tracking.Infrastructure;

public class TrackingDbContext(DbContextOptions<TrackingDbContext> options) : DbContext(options)
{
    public DbSet<CourierLocationPoint> CourierLocationPoints => Set<CourierLocationPoint>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CourierLocationPointConfiguration());
    }
}

internal class CourierLocationPointConfiguration : IEntityTypeConfiguration<CourierLocationPoint>
{
    public void Configure(EntityTypeBuilder<CourierLocationPoint> builder)
    {
        builder.ToTable("CourierLocationPoints");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever();

        builder.Property(x => x.CourierId)
            .IsRequired();

        builder.Property(x => x.OrderId)
            .IsRequired(false);

        builder.Property(x => x.Latitude)
            .IsRequired();

        builder.Property(x => x.Longitude)
            .IsRequired();

        builder.Property(x => x.Timestamp)
            .IsRequired();

        builder.HasIndex(x => x.CourierId);
        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.Timestamp);
    }
}