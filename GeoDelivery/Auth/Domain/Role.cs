using Microsoft.AspNetCore.Identity;

namespace Auth.Domain;

public class Role(string name) : IdentityRole<Guid>(name);