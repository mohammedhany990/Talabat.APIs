using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Extensions
{
    public static class UserManagerExtensionMethod
    {
        public static async Task<AppUser?> FindUserWithAddressAsync(this UserManager<AppUser> manager,
                                                                    ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            var user = await manager.Users
                .Include(A => A.Address)
                .FirstOrDefaultAsync(U => U.NormalizedEmail == email.ToUpper());

            return user;
        }
    }
}
