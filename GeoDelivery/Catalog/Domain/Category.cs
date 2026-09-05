namespace Catalog.Domain;

public class Category(Guid restaurantId, string name)  
{  
    private Category() : this(Guid.Empty, string.Empty) { }  
      
    public Guid Id { get; init; } = Guid.CreateVersion7();  
      
    public Guid RestaurantId { get; init; } = restaurantId;  
  
    public string Name { get; init; } = name;  
}