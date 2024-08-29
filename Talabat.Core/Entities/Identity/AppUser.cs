using Microsoft.AspNetCore.Identity;

namespace Talabat.Core.Entities.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = null!;

        public Address? Address { get; set; } = null!;

        public List<RefreshToken>? RefreshTokens { get; set; }

      
    }
}
