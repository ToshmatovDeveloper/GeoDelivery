using Geo.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geo.Infrastructure;

using Microsoft.EntityFrameworkCore;

public class GeoDbContext : DbContext
{
    public DbSet<DeliveryZone> DeliveryZones => Set<DeliveryZone>();

    public GeoDbContext(DbContextOptions<GeoDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("geo");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoDbContext).Assembly);
    }
}

public class DeliveryZoneConfiguration : IEntityTypeConfiguration<DeliveryZone>
{
    public void Configure(EntityTypeBuilder<DeliveryZone> builder)
    {
        builder.ToTable("delivery_zones");

        builder.HasKey(z => z.Id);

        builder.Property(z => z.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(z => z.Boundary)
            .HasColumnType("geometry(Polygon, 4326)")
            .IsRequired();

        builder.Property(z => z.DeliveryCost)
            .HasPrecision(18, 2);

        builder.HasIndex(z => z.Boundary)
            .HasMethod("gist");
    }
}