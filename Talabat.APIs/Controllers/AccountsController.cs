using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.DTOs.Account;
using Talabat.APIs.Errors;
using Talabat.APIs.Extensions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.TokenService;
using Address = Talabat.Core.Entities.Identity.Address;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthenticationService _tokenService;
        private readonly IMapper _mapper;

        public AccountsController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 IAuthenticationService tokenService,
                                 IMapper mapper
                                 )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }



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

            var jwtSecurityToken = await _tokenService.CreateTokenAsync(user, _userManager);

            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var returnedUser = new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
            SetRefreshTokenInCookie(returnedUser.RefreshToken, returnedUser.RefreshTokenExpiration);


            return Ok(returnedUser);

        }
        #endregion


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

            var jwtSecurityToken = await _tokenService.CreateTokenAsync(user, _userManager);

            var returnedUser = new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),

            };

            //Refresh Token
            if (user.RefreshTokens.Any(A => A.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(A => A.IsActive);
                returnedUser.Token = activeToken.Token;
                returnedUser.RefreshTokenExpiration = activeToken.ExpiresOn;
            }
            else
            {
                var refreshToken = _tokenService.GenerateRefreshToken();
                returnedUser.RefreshToken = refreshToken.Token;
                returnedUser.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }
            if (!string.IsNullOrEmpty(returnedUser.RefreshToken))
            {
                SetRefreshTokenInCookie(returnedUser.RefreshToken, returnedUser.RefreshTokenExpiration);
            }
            return Ok(returnedUser);


        }
        #endregion


        #region GetCurrentUser

        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {

            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);
            var jwtSecurityToken = await _tokenService.CreateTokenAsync(user, _userManager);
            var refreshToken = _tokenService.GenerateRefreshToken();

            var returnedUser = new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn

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


        #region UpdateAddress
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
        #endregion


        #region RefreshToken
        [HttpGet("refreshToken")]
        public async Task<ActionResult<UserDto>> RefreshToken()
        {
            var token = Request.Cookies["refreshToken"];
            var user = await _userManager.Users.SingleOrDefaultAsync(U => U.RefreshTokens.Any(T => T.Token == token));
            if (user is null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Token"));
            }

            var refreshToken = user.RefreshTokens.Single(T => T.Token == token);

            if (!refreshToken.IsActive)
            {
                return BadRequest(new ApiResponse(400, "InActive Token"));
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await _tokenService.CreateTokenAsync(user, _userManager);

            var returnedUser = new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpiration = newRefreshToken.ExpiresOn,

            };
            SetRefreshTokenInCookie(returnedUser.RefreshToken, returnedUser.RefreshTokenExpiration);

            return Ok(returnedUser);
        }
        #endregion


        #region RevokeToken
        [HttpPost("revokeToken")]
        public async Task<ActionResult> RevokeToken([FromBody] RevokeTokenDto tokenDto)
        {
            var token = tokenDto.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new ApiResponse(400, "Token is required."));

            }

            var user = await _userManager.Users.SingleOrDefaultAsync(U => U.RefreshTokens.Any(T => T.Token == token));

            if (user is null)
            {
                return BadRequest(new ApiResponse(400, "Invalid Token"));
            }

            var refreshToken = user.RefreshTokens.Single(T => T.Token == token);

            if (!refreshToken.IsActive)
            {
                return BadRequest(new ApiResponse(400, "InActive Token"));
            }

            refreshToken.RevokedOn = DateTime.UtcNow;


            await _userManager.UpdateAsync(user);
            return Ok();
        }
        #endregion


        private void SetRefreshTokenInCookie(string refreshToken, DateTime expire)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = expire.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }
}
