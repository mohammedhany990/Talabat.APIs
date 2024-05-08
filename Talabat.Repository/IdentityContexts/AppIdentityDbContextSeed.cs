using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.IdentityContexts
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task UserSeedAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any() )
            {
                var User = new AppUser()
                {
                    DisplayName = "Mohammed Hany",
                    Email = "mohammedhanymaher990@gmail.com",
                    UserName = "mohammedhanymaher990",
                    PhoneNumber = "01099040294"
                };
                await userManager.CreateAsync(User, "P@ssw0rd");
            }
            
        }
    }
}
