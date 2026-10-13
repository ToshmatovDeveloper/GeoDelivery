using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Order.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Order.Domain.Entity;


public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }
    
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("order");
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.OwnsOne(o => o.DeliveryAddress, address =>
        {
            address.Property(a => a.AddressText)
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnName("DeliveryAddressText");

            address.Property(a => a.Latitude)
                .IsRequired()
                .HasColumnName("DeliveryLatitude");

            address.Property(a => a.Longitude)
                .IsRequired()
                .HasColumnName("DeliveryLongitude");
        });

        builder.Property(o => o.Status)
            .HasConversion<string>() 
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        var navigation = builder.Metadata.FindNavigation(nameof(Order.Items));
        navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(oi => oi.Id);
        
        builder.Property(oi => oi.Id)
            .ValueGeneratedNever();

        builder.Property(oi => oi.DishName)
            .IsRequired()
            .HasMaxLength(250);

        builder.Property(oi => oi.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(oi => oi.Quantity)
            .IsRequired();

        builder.Ignore(oi => oi.TotalPrice);
    }
}


