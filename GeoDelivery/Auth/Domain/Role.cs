using Microsoft.AspNetCore.Identity;

namespace Auth.Domain;

public sealed class Role : IdentityRole<Guid>
{
    public Role() { }
    
    public Role(string name) : base(name)
    {
    }
}