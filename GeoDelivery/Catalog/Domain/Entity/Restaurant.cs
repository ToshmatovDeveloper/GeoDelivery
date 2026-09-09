namespace Catalog.Domain.Entity;

public class Restaurant(string name, string description)  
{  
    private Restaurant() : this(string.Empty, string.Empty) { }  
      
    public Guid Id { get; init; } = Guid.CreateVersion7();  
      
    public string Name { get; init; } = name;  
  
    public string Description { get; init; } = description;  
  
    public bool IsActive { get; init; } = true;  
  
    private readonly List<Category> _categories = new();  
}