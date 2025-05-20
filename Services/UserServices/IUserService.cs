using BackEnd.Common;
using BackEnd.Dto.Users.ResponseDto;
using BackEnd.Models;

namespace BackEnd.Services.UserServices
{
    public interface IUserService
    {

        Task<Dictionary<string, object>> GetUserByDisplayId(string displayId);
        Task<bool> CheckEmail(string email);
        Task<Dictionary<string, object>> login(string email, string password);
        Task<UserInfoDto> GetUserAccount(int id);
        Task<Dictionary<string,object>> RegisterAccount(string email, string password, string displayName);

        Task<Dictionary<string, object>> UpdateName(int id,string displayName);
        Task<Dictionary<string, object>> UpdateDisplayId(int id,string displayId);
        Task<Dictionary<string, object>> UpdateEmail(int id, string email, string password);
        Task<bool> UpdateAvatar(int id, string relativePath);
        Task<Dictionary<string, object>> UpdatePassword(int id,string password, string newPassword);
        Task<Dictionary<string, object>> ForgotPassword(string email);
        Task<Dictionary<string, object>> VerifyForgotPasswordToken(string token);
        Task<Dictionary<string, object>> ResetPassword(string token, string newPassword);
        Task<Dictionary<string, object>> SetPassword(int id, string password);
        Task<Dictionary<string, object>> GetPasswordById(int id);
    }
}
