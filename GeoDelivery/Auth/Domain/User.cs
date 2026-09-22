using Microsoft.AspNetCore.Identity;

namespace Auth.Domain;

public sealed class User : IdentityUser<Guid>
{
    public User(string name, string email)
    {
        Id = Guid.CreateVersion7();
        UserName = name;
        Email = email;
        CreatedAt = DateTime.UtcNow;
    }
    
    public DateTime CreatedAt { get; set; }
}