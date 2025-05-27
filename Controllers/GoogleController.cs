using BackEnd.Models;
using BackEnd.Services.Auth;
using BackEnd.Services.GoogleService;
using BackEnd.Services.UserServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Common;
using System.Security.Claims;
using System.Text.Json;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("chatroom/auth")]
    public class GoogleController :ControllerBase
    {
        private readonly IGoogleService _googleService;

        public GoogleController(IGoogleService googleService)
        {
            this._googleService = googleService;
        }

        [HttpGet("google-login-url")]
        public IActionResult GetGoogleLoginUrl()
        {
            var redirectUrl = Url.Action("GoogleResponse", "Google", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleResponse() 
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!result.Succeeded)
                return Unauthorized();

            var claims = result.Principal.Identities
                .FirstOrDefault()?.Claims.ToDictionary(c => c.Type, c => c.Value);
            var id = claims[ClaimTypes.NameIdentifier];
            var email = claims[ClaimTypes.Email];
            var name = claims[ClaimTypes.Name];

            var user = await _googleService.GoogleLogin(id, email, name);

            var redirectUrl = string.Empty;

            if (user.TryGetValue("data", out var dataObj) && dataObj is Dictionary<string, object> data)
            {
                if (data.TryGetValue("token", out var tokenObj) && tokenObj is string token)
                {
                    Response.Cookies.Append("access_token", token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(1)
                    });
                }

                var baseUrl = "https://chatroom-frontend-uc36.onrender.com";
                //var baseUrl = "http://127.0.0.1:5500";

                if (data.TryGetValue("userInfo", out var userInfoObj) && userInfoObj is User userInfo)
                {
                    if (string.IsNullOrEmpty(userInfo.Password))
                    {
                        redirectUrl = $"{baseUrl}/setPassword.html";
                    }
                    else
                    {
                        redirectUrl = $"{baseUrl}/profile.html";
                    }
                }


            }
            return Redirect(redirectUrl);
        }

    }
}
