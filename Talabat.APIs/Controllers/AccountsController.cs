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
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services;
using Talabat.Core.TokenService;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using Address = Talabat.Core.Entities.Identity.Address;

namespace Talabat.APIs.Controllers
{

    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IAuthenticationService _tokenService;
        private readonly IMapper _mapper;
        private readonly IEmailSettings _emailSettings;


        public AccountsController(UserManager<AppUser> userManager,
                                 SignInManager<AppUser> signInManager,
                                 IAuthenticationService tokenService,
                                 IMapper mapper,
                                 IEmailSettings emailSettings
                                 )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _emailSettings = emailSettings;
            _emailSettings = emailSettings;
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

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);

            if (!result.Succeeded)
            {
                return NotFound(new ApiResponse(400, "You are unauthorized."));
            }

            var jwtSecurityToken = await _tokenService.CreateTokenAsync(user, _userManager);
            
            var returnedUser = new UserDto()
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                ExpiresOn = jwtSecurityToken.ValidTo,
            };

            //Refresh Token
            if (user.RefreshTokens.Any(A => A.IsActive))
            {
                var activeToken = user.RefreshTokens.FirstOrDefault(A => A.IsActive);

                returnedUser.RefreshToken = activeToken.Token;

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


        #region SignOut
        [HttpGet("sign-out")]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
        #endregion

        #region DeleteAccount

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteAccount()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                var res = await _userManager.DeleteAsync(user);
                if (res.Succeeded)
                {
                    return Ok(new ApiResponse(200, "Deleted Successfully!") );
                }
                else
                {
                    return BadRequest(new ApiResponse(400, "Error deleting data."));
                }
            }
            else
            {
                return BadRequest(new ApiResponse(400, "User not found"));
            }
        }
        #endregion


        #region Reset Password
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is not null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

               var resetPasswordLink = Url.Action(nameof(ResetPassword),
                    "Accounts", new { Token = token, email = user.Email },
                    Request.Scheme);

                var emailToSend = new Email()
                {
                    To = user.Email,
                    Subject = "Reset Password",
                    Body = $"Please reset your password by clicking here: <a href=\"{resetPasswordLink}\">link</a>",
                };
                try
                {
                    _emailSettings.SendEmail(emailToSend);
                    return Ok(new ApiResponse(200, "Password reset link sent."));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiResponse(500, $"Failed to send email: {ex.Message}"));
                }
            }
            else
            {
                return BadRequest(new ApiResponse(404, "User not found"));
            }
        }

        [HttpGet("ResetPassword")]
        public async Task<ActionResult> ResetPassword(string email, string token)
        {
            var model = new ResetPasswordDto()
            {
                Email = email,
                Token = token,
            };
            return Ok(model);
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user is not null)
            {
                var resetPassword = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
                
                if (!resetPassword.Succeeded)
                {
                    foreach (var error in resetPassword.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return Ok(ModelState);
                }
                else
                {
                    return Ok(new ApiResponse(200, "Password has been changed."));
                }
            }
            return BadRequest(new ApiResponse(400, "couldn't reset password."));
        }

        #endregion


        #region GetCurrentUser

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public async Task<ActionResult<Address>> GetUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);

            var ReturnesAddress = _mapper.Map<Address, AddressDto>(user.Address);
            if (ReturnesAddress is null)
            {
                return NotFound(new ApiResponse(404, "No Address Found"));
            }
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
