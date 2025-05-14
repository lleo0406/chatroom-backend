using BackEnd.Dto.Users.ResponseDto;
using BackEnd.Models;

namespace BackEnd.Repositories.UserRepository
{
    public interface IUserRepository
    {
        Task<bool> GetUserByPassword(int id, string password);
        Task<User?> GetUserByDisplayId(string displayId);
        Task<User?> GetUserByEmail(string email);
        Task<UserDto> GetUserById(int id);
        Task<UserDto> GetUserByEmailAndPassword(string email, string password);
        Task<User> RegisterAccount(string email, string password, string displayName);
        Task<User> UpdateDisplayId(int id, string displayId);
        Task<User> UpdateName(int id, string displayName);
        Task<User> UpdatePassword(int id, string newPassword);
        Task<User> UpdateEmail(int id, string email);
        Task<User> UpdateAvatarPathAsync(int id, string avatarPath);
        Task<PasswordResetToken> PasswordResetToken(string token, int userId);
        Task<bool> VerifyForgotPasswordToken(string token);
        Task<int> ResetPassword(string token, string newPassword);
        Task<User> UpdateGoogleId(int id, string googleId);
        Task<User> RegisterGoogleAccount(string email, string displayName, string googleId);
        Task<bool> SetPassword(int id, string password);
    }
}
