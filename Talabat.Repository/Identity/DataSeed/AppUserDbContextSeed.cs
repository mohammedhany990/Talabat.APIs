using Microsoft.AspNetCore.Identity;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity.DataSeed
{
    public static class AppUserDbContextSeed
    {
        public static async Task AddAppUserDbContextSeed(UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var User = new AppUser()
                {
                    DisplayName = "Mohammed Hany",
                    Email = "mohammedhanymaher990@gmail.com",
                    UserName = "mohammedhanymaher990",
                    PhoneNumber = "01099040294"
                };
                userManager.CreateAsync(User, "Abcd@1234");
            }

        }
    }
}
