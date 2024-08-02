using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;

namespace Talabat.Core.TokenService
{
    public interface IAuthenticationService
    {
        Task<JwtSecurityToken> CreateTokenAsync(AppUser user, UserManager<AppUser> userManager);
        public RefreshToken GenerateRefreshToken();

    }
}
