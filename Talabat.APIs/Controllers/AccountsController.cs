using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.DTOs.Account;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.TokenService;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 ITokenService tokenService,
                                 IMapper mapper
                                 )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }


        #region Login

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return NotFound(new ApiResponse(400, "You are unauthorized."));
            }

            var flag = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!flag.Succeeded)
            {
                return NotFound(new ApiResponse(400, "You are unauthorized."));
            }

            return Ok(new UserDto()
            {
                Email = dto.Email,
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }
        #endregion


        #region Register

        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
        {
            if (CheckEmailExisting(dto.Email).Result.Value)
            {
                return BadRequest(new ApiResponse(400, "This Account Already Exists"));
            }

            var user = new AppUser()
            {
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                PhoneNumber = dto.Phone,
                UserName = dto.Email.Split("@")[0]
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new ValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });
            }

            return Ok(new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });

        }
        #endregion

        #region GetCurrentUser

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            var returnedUser = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };
            return Ok(returnedUser);
        }
        #endregion


        #region EmailExists
        [HttpGet("emailExists")]
        public async Task<ActionResult<bool>> CheckEmailExisting([Required] string email)
        {
            return await _userManager.FindByEmailAsync(email) is not null;
        }
        #endregion


        #region GetUserAddress
        [HttpGet("GetUserAddress")]
        public async Task<ActionResult<Address>> GetUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);

            var ReturnesAddress = _mapper.Map<Address, AddressDto>(user.Address);

            return Ok(ReturnesAddress);
        }
        #endregion


        [Authorize]
        [HttpPut("UpdateAddress")]
        public async Task<ActionResult<Address>> UpdateAddress(AddressDto dto)
        {
            var address = _mapper.Map<AddressDto, Address>(dto);

            var user = await _userManager.FindUserWithAddressAsync(User);

            address.Id = user.Address.Id;
            user.Address = address;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest();
            }

            return Ok(dto);
        }

    }
}
