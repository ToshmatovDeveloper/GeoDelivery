namespace Catalog.Domain;

public class Dish(Guid restaurantId, Guid categoryId, string name, decimal price)  
{  
    private Dish() : this(Guid.Empty, Guid.Empty, string.Empty, 0m) { }  
      
    public Guid Id { get; set; } = Guid.CreateVersion7();  
      
    public Guid RestaurantId { get; set; } = restaurantId;  
  
    public Guid CategoryId { get; set; } = categoryId;  
  
    public string Name { get; set; } = name;  
  
    public decimal Price { get; set; } = price;  
  
    public bool IsAvailable { get; set; } = true;  
}