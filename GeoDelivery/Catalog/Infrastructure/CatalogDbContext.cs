using Catalog.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure;

public class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options) : DbContext(options)  
{  
    public DbSet<Restaurant> Restaurants { get; set; }  
    public DbSet<Category> Categories { get; set; }  
    public DbSet<Dish> Dishes { get; set; }  
      
    protected override void OnModelCreating(ModelBuilder modelBuilder)  
    {  
        modelBuilder.ApplyConfiguration(new RestaurantConfiguration());  
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());  
        modelBuilder.ApplyConfiguration(new DishConfiguration());  
    }  
}  

public class RestaurantConfiguration : IEntityTypeConfiguration<Restaurant>  
{  
    public void Configure(EntityTypeBuilder<Restaurant> builder)  
    {  
        builder.ToTable("restaurants");  
          
        builder.HasKey(x => x.Id);  
          
        builder.Property(x => x.Name)  
            .HasMaxLength(100)  
            .IsRequired();  
          
        builder.Property(x => x.Description)  
            .HasMaxLength(500)  
            .IsRequired();  
          
        builder.HasMany(typeof(Category), "_categories")  
            .WithOne()  
            .HasForeignKey("RestaurantId")  
            .IsRequired();  
  
        builder.Navigation("_categories")  
            .UsePropertyAccessMode(PropertyAccessMode.Field);  
    }  
}  
  
public class CategoryConfiguration : IEntityTypeConfiguration<Category>  
{  
    public void Configure(EntityTypeBuilder<Category> builder)  
    {  
        builder.ToTable("categories");  
          
        builder.HasKey(x => x.Id);  
  
        builder.Property(x => x.RestaurantId)  
            .IsRequired();  
          
        builder.Property(x => x.Name)  
            .HasMaxLength(100)  
            .IsRequired();  
          
         
    }  
}  
  
public class DishConfiguration : IEntityTypeConfiguration<Dish>  
{  
    public void Configure(EntityTypeBuilder<Dish> builder)  
    {  
        builder.ToTable("dishes");  
          
        builder.HasKey(x => x.Id);  
          
        builder.Property(x => x.RestaurantId)  
            .IsRequired();  
              
        builder.Property(x => x.CategoryId)  
            .IsRequired();  
  
        builder.Property(x => x.Name)  
            .HasMaxLength(150)  
            .IsRequired();  
  
        builder.Property(x => x.Price)  
            .HasPrecision(18, 2)  
            .IsRequired();  
  
        builder.Property(x => x.IsAvailable)  
            .IsRequired();  
    }  
}