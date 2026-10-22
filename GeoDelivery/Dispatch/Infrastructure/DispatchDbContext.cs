using Microsoft.EntityFrameworkCore;
using Dispatch.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Dispatch.Infrastructure;

public class DispatchDbContext(DbContextOptions<DispatchDbContext> options) : DbContext(options)
{
    public DbSet<CourierAssignment> CourierAssignments => Set<CourierAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("dispatch");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DispatchDbContext).Assembly);
    }
}

public class CourierAssignmentConfiguration : IEntityTypeConfiguration<CourierAssignment>
{
    public void Configure(EntityTypeBuilder<CourierAssignment> builder)
    {
        builder.ToTable("CourierAssignments");

        builder.HasKey(ca => ca.Id);

        builder.Property(ca => ca.OrderId)
            .IsRequired();

        builder.Property(ca => ca.RestaurantId)
            .IsRequired();

        builder.Property(ca => ca.CourierId)
            .IsRequired();

        builder.Property(ca => ca.Status)
            .HasConversion<string>() 
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(ca => ca.AssignedAt)
            .IsRequired();

        builder.Property(ca => ca.CompletedAt);
    }
}