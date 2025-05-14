using Azure;
using BackEnd.Common;
using BackEnd.Dto.Users.ResponseDto;
using BackEnd.Helpers;
using BackEnd.Models;
using BackEnd.Repositories.UserRepository;
using BackEnd.Services.Auth;
using BackEnd.Services.CommonService;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Newtonsoft.Json;

namespace BackEnd.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;

        public UserService(IUserRepository userRepository, IJwtService jwtService, IEmailService emailService)
        {
            _userRepository = userRepository;
            this._jwtService = jwtService;
            this._emailService = emailService;
        }

        public async Task<Dictionary<string, object>> login(string email, string password)
        {
            var response = new Dictionary<string, object>();


            var user = await _userRepository.GetUserByEmailAndPassword(email, password);
            if (user != null)
            {
                var token = _jwtService.GenerateToken(
                    user.Id.ToString(),
                    user.DisplayName ?? "",
                    user.Email ?? ""
                );

                var result = new Dictionary<string, object>
                {
                    { "token", token },
                    { "userInfo", new
                        {
                            user.Id,
                            user.Email,
                            user.DisplayId,
                            user.DisplayName,
                            user.Picture,
                            user.CreatedAt
                        }
                    }
                };

                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
                response.Add(Constants.DATA, result);

            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }


            return response;
        }


        public async Task<UserInfoDto> GetUserAccount(int id)
        {
            var user = await _userRepository.GetUserById(id);
            var userInfo = new UserInfoDto
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                DisplayId = user.DisplayId,
                Email = user.Email,
                Picture = user.Picture,
                CreatedAt = user.CreatedAt
            };
            return userInfo;
        }


        public async Task<Dictionary<string, object>> RegisterAccount(string email, string password, string displayName)
        {
            var response = new Dictionary<string, object>();

            var existing = await CheckEmail(email);

            if (!existing)
            {
                await _userRepository.RegisterAccount(email, password, displayName);

                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.DATA_EXISTS);
                response.Add(Constants.MESSAGE, Constants.DATA_EXISTS_MESSAGE);
            }
            return response;

        }

        public async Task<Dictionary<string, object>> GetUserByDisplayId(string displayId)
        {
            var response = new Dictionary<string, object>();

            var user = await _userRepository.GetUserByDisplayId(displayId);

            if (user == null)
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);

            }
            else
            {
                response.Add(Constants.CODE, Constants.DATA_EXISTS);
                response.Add(Constants.MESSAGE, Constants.DATA_EXISTS_MESSAGE);
            }

            return response;

        }

        public async Task<Dictionary<string, object>> UpdateName(int id, string displayName)
        {
            var response = new Dictionary<string, object>();
            var user = await _userRepository.UpdateName(id, displayName);

            response.Add(Constants.CODE, Constants.SUCCESS);
            response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);

            return response;
        }

        public async Task<Dictionary<string, object>> UpdateDisplayId(int id, string displayId)
        {
            var response = new Dictionary<string, object>();
            var user = await _userRepository.UpdateDisplayId(id, displayId);

            response.Add(Constants.CODE, Constants.SUCCESS);
            response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);

            return response;

        }

        public async Task<bool> CheckEmail(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            return user != null;
        }

        public async Task<Dictionary<string, object>> UpdatePassword(int id, string password, string newPassword)
        {
            var response = new Dictionary<string, object>();
            var user = await _userRepository.GetUserById(id);

            var hashedPassword = PasswordHelper.HashPassword(password, user.Salt);
            Console.WriteLine("hashedPassword=> " + hashedPassword);
            Console.WriteLine("user.Password=> " + user.Password);
            if (user.Password != hashedPassword)
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
                return response;
            }
            var newHashedPassword = PasswordHelper.HashPassword(newPassword, user.Salt);

            if (user.Password == newHashedPassword)
            {
                response.Add(Constants.CODE, Constants.DATA_EXISTS);
                response.Add(Constants.MESSAGE, Constants.DATA_EXISTS_MESSAGE);
                return response;
            }

            var result = await _userRepository.UpdatePassword(id, newHashedPassword);
            if (result != null)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }

            return response;

        }

        public async Task<Dictionary<string, object>> UpdateEmail(int id, string email, string password)
        {
            var response = new Dictionary<string, object>();

            bool passwordCheck = await _userRepository.GetUserByPassword(id, password);
            if (!passwordCheck)
            {
                response.Add(Constants.CODE, Constants.PASSWORD_ERROR);
                response.Add(Constants.MESSAGE, Constants.PASSWORD_ERROR_MESSAGE);
                return response;
            }

            bool emailExist = await CheckEmail(email);
            if (emailExist)
            {
                response.Add(Constants.CODE, Constants.DATA_EXISTS);
                response.Add(Constants.MESSAGE, Constants.DATA_EXISTS_MESSAGE);
                return response;
            }

            var user = await _userRepository.UpdateEmail(id, email);
            if (user != null)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }

            return response;

        }

        public async Task<bool> UpdateAvatar(int id, string relativePath)
        {
            var result = await _userRepository.UpdateAvatarPathAsync(id, relativePath);
            return result != null;
        }

        public async Task<Dictionary<string, object>> ForgotPassword(string email)
        {
            var response = new Dictionary<string, object>();
            var user = await _userRepository.GetUserByEmail(email);

            if (user != null)
            {
                var resetToken = Guid.NewGuid().ToString();

                var passwordResetToken = await _userRepository.PasswordResetToken(resetToken, user.Id);

                var resetLink = $"http://127.0.0.1:5500/resetPassword.html?token={resetToken}";
                var emailSubject = "ChatRoom密碼重設連結";
                var emailBody = $"請點擊以下連結來重設您的密碼：<br/><a href=\"{resetLink}\">{resetLink}</a><br/><p>請在15分鐘內完成重設密碼。</p>";

                await _emailService.SendEmailAsync(email, emailSubject, emailBody);


                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);

                return response;
            }
            response.Add(Constants.CODE, Constants.NOT_FOUND);
            response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);

            return response;
        }

        public async Task<Dictionary<string, object>> VerifyForgotPasswordToken(string token)
        {
            var response = new Dictionary<string, object>();
            var result = await _userRepository.VerifyForgotPasswordToken(token);
            if (result)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }

            return response;
        }

        public async Task<Dictionary<string, object>> ResetPassword(string token, string newPassword)
        {
            var response = new Dictionary<string, object>();
            var result = await _userRepository.ResetPassword(token, newPassword);
            if (result == 200)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else if (result == 408)
            {
                response.Add(Constants.CODE, Constants.TIMEOUT);
                response.Add(Constants.MESSAGE, Constants.TIMEOUT_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.FAIL);
                response.Add(Constants.MESSAGE, Constants.FAIL_MESSAGE);
            }
            return response;

        }

        public async Task<Dictionary<string, object>> SetPassword(int id, string password)
        {
            var response = new Dictionary<string, object>();
            var result = await _userRepository.SetPassword(id, password);
            if (result)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.FAIL);
                response.Add(Constants.MESSAGE, Constants.FAIL_MESSAGE);
            }
            return response;

        }
    }
}
