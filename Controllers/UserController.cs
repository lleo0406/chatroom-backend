using BackEnd.Common;
using BackEnd.Dto.Users.RequestDto;
using BackEnd.Exceptions;
using BackEnd.Helpers;
using BackEnd.Services.Auth;
using BackEnd.Services.ImageService;
using BackEnd.Services.UserServices;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using NuGet.Common;
using System.Net.Mail;
using System.Reflection.Metadata;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("chatroom/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IDistributedCache _cacheService;
        private readonly IImageService _imageService;

        public UserController(IUserService userService, IDistributedCache cacheService,IImageService imageService)
        {
            this._userService = userService;
            _cacheService = cacheService;
            this._imageService = imageService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var result = await _userService.login(loginRequest.Email, loginRequest.Password);

            if (result.TryGetValue("code", out var codeObj) && Convert.ToInt32(codeObj) == 200)
            {
                if (result["data"] is Dictionary<string, object> data &&
                    data.TryGetValue("token", out var tokenObj))
                {
                    string token = tokenObj?.ToString();

                    Response.Cookies.Append("access_token", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });
                }

                return Ok(result);
            }

            return Ok(result); 
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Append("access_token", "", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                Path = "/"
            });

            Dictionary<string, object> response = new Dictionary<string, object>
            {
                { Constants.CODE, Constants.SUCCESS },
                { Constants.MESSAGE, Constants.SUCCESS_MESSAGE }
            };

            return Ok(response);
        }



        [HttpPost("registerAccount")]
        public async Task<IActionResult> RegisterAccount(RegisterRequest registerRequest)
        {
            var result = await _userService.RegisterAccount(registerRequest.Email, registerRequest.Password, registerRequest.DisplayName);
            return Ok(result);

        }

        [Authorize]
        [HttpGet("getUserInfo")]
        public async Task<IActionResult> getUserInfo()
        {
            var response = new Dictionary<string, object>();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }
            var result = await _userService.GetUserAccount(int.Parse(userId));

            response.Add(Constants.CODE, Constants.SUCCESS);
            response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            response.Add(Constants.DATA, result);

            return Ok(response);
        }

        [Authorize]
        [HttpPost("updateName")]
        public async Task<IActionResult> UpdateName(UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            var result = await _userService.UpdateName(int.Parse(userId), request.DisplayName);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("updateDisplayId")]
        public async Task<IActionResult> UpdateDisplayId(UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");

            }

            var displayIdResult = await _userService.GetUserByDisplayId(request.DisplayId);

            if (displayIdResult.TryGetValue(Constants.CODE, out var codeValue))
            {
                int code = Convert.ToInt32(codeValue);
                if (code == Constants.DATA_EXISTS)
                {
                    return Ok(displayIdResult);
                }

            }

            var result = await _userService.UpdateDisplayId(int.Parse(userId), request.DisplayId);

            return Ok(result);
        }

        [HttpPost("updatePassword")]
        public async Task<IActionResult> UpdatePassword(UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }
            var result = await _userService.UpdatePassword(int.Parse(userId), request.Password, request.NewPassword);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("setPassword")]
        public async Task<IActionResult> SetPassword(UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _userService.SetPassword(int.Parse(userId), request.Password);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("updateEmail")]
        public async Task<IActionResult> UpdateEmail(UpdateUserRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            var result = await _userService.UpdateEmail(int.Parse(userId), request.Email, request.Password);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("uploadAvatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile avatar)
        {
            var response = new Dictionary<string, object>();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            if (avatar == null || avatar.Length == 0)
            {
                return BadRequest("未上傳檔案");
            }

            var photoPath  = await _imageService.SaveWebpAsync(avatar, "uploads/avatars", "/uploads/avatars/f2dda304-3531-4cb1-9883-2b2d97af41c1.webp");

            var result = await _userService.UpdateAvatar(int.Parse(userId), photoPath );

            response.Add(Constants.CODE, Constants.SUCCESS);
            response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            response.Add(Constants.DATA,  photoPath );

            return Ok(response);
        }


        [Authorize]
        [HttpPost("getProfile")]
        public IActionResult GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(userId);
            if(userId != null)
            {
                return Ok(new { isValid = true });

            }
            return Unauthorized();
        }


        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordToken request)
        {
            var result = await _userService.ForgotPassword(request.Email);
            return Ok(result);
        }


        [HttpPost("verifyForgotPasswordToken")]
        public async Task<IActionResult> VerifyForgotPasswordToken(ForgotPasswordToken request)
        {
            var result = await _userService.VerifyForgotPasswordToken(request.Token);
            return Ok(result);
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword(ForgotPasswordToken request)
        {
            var result = await _userService.ResetPassword(request.Token, request.newPassword);
            return Ok(result);
        }

    }
}
