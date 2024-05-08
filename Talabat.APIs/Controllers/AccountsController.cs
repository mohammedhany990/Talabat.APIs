using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.Identity;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : APIBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public AccountsController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO model)
        {
            var User = new AppUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber,
            };
            var Result = await _userManager.CreateAsync(User, model.Password);
            if (!Result.Succeeded)
            {
                return BadRequest(new ApiResponse(400));
            }

            var ReturnedUser = new UserDTO()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                Token = "TokenAfterAwhile",
            };
            return Ok(ReturnedUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is not null)
            {
                var Result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (Result.Succeeded)
                {
                    return Ok(new UserDTO()
                    {
                        DisplayName = user.DisplayName,
                        Email = user.Email,
                        Token = "TokenAfterAwhile",
                    });
                }
                else
                {
                    return Unauthorized(new ApiResponse(401));
                }

            }
            else
            {
                return Unauthorized(new ApiResponse(401));
            }
        }
    }
}
