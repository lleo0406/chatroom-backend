using BackEnd.Dto.Users.ResponseDto;
using BackEnd.Helpers;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.ComponentModel;

namespace BackEnd.Repositories.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IChatroomContext _context;

        public UserRepository(IChatroomContext context)
        {
            this._context = context;
        }

        public async Task<UserDto> GetUserById(int id)
        {
            var user = await _context.Users.FindAsync(id);
            UserDto userDto = new();
            if (user != null)
            {
                userDto.Id = user.Id;
                userDto.DisplayName = user.DisplayName;
                userDto.DisplayId = user.DisplayId;
                userDto.Email = user.Email;
                userDto.Picture = user.Picture;
                userDto.Password = user.Password;
                userDto.Salt = user.Salt;
            }
            return userDto;
        }

        public Task<User?> GetUserByDisplayId(string displayId)
        {
            return _context.Users.FirstOrDefaultAsync(u => u.DisplayId == displayId);
        }


        public async Task<User?> GetUserByEmail(string email)
        {

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<UserDto?> GetUserByEmailAndPassword(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return null;

            var hashedPassword = PasswordHelper.HashPassword(password, user.Salt);

            if (user.Password != hashedPassword)
                return null;

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayId = user.DisplayId,
                DisplayName = user.DisplayName,
                Picture = user.Picture,
                CreatedAt = user.CreatedAt
            };

            return userDto;
        }

        public async Task<User> RegisterAccount(string email, string password, string displayName)
        {
            var salt = PasswordHelper.GenerateSalt();

            var hashedPassword = PasswordHelper.HashPassword(password, salt);

            var user = new User
            {
                Email = email,
                Password = hashedPassword,
                Salt = salt,
                DisplayName = displayName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<User> RegisterGoogleAccount(string email, string displayName, string googleId)
        {
            var user = new User
            {
                Email = email,
                DisplayName = displayName,
                GoogleId = googleId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }


        public async Task<User> UpdateName(int id, string displayName)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.DisplayName = displayName;
                await _context.SaveChangesAsync();
            }
            return user;

        }

        public async Task<User> UpdateDisplayId(int id, string displayId)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.DisplayId = displayId;
                await _context.SaveChangesAsync();
            }
            return user;


        }

        public async Task<bool> GetUserByPassword(int id, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            var hashedPassword = PasswordHelper.HashPassword(password, user.Salt);

            var result = await _context.Users.FirstOrDefaultAsync(u => u.Password == hashedPassword);

            return result != null;
        }


        public async Task<User> UpdatePassword(int id, string newPassword)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Password = newPassword;
                await _context.SaveChangesAsync();

            }
            return user;
        }

        public async Task<User> UpdateGoogleId(int id, string googleId)
        {
            var user = await _context.Users.FindAsync(id);
            user.GoogleId = googleId;
            await _context.SaveChangesAsync();
            return user;

        }

        public async Task<User> UpdateEmail(int id, string email)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Email = email;
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<User> UpdateAvatarPathAsync(int id, string avatarPath)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.Picture = avatarPath;
                await _context.SaveChangesAsync();
            }
            return user;
        }

        public async Task<PasswordResetToken> PasswordResetToken(string resetToken, int userId)
        {
            var existingToken = await _context.PasswordResetToken
                .FirstOrDefaultAsync(t => t.UserId == userId && t.ExpirationTime > DateTime.UtcNow);

            if (existingToken != null)
            {
                _context.PasswordResetToken.Remove(existingToken);
                await _context.SaveChangesAsync();
            }

            var result = new PasswordResetToken
            {
                Token = resetToken,
                UserId = userId,
                ExpirationTime = DateTime.UtcNow.AddMinutes(15)
            };
            await _context.PasswordResetToken.AddAsync(result);
            await _context.SaveChangesAsync();

            return result;

        }

        public async Task<bool> VerifyForgotPasswordToken(string token)
        {
            var result = await _context.PasswordResetToken.FirstOrDefaultAsync(t => t.Token == token && t.ExpirationTime > DateTime.UtcNow);

            if (result == null)
            {
                return false;
            }

            return true;
        }

        public async Task<int> ResetPassword(string token, string newPassword)
        {
            var time = await _context.PasswordResetToken
                .FirstOrDefaultAsync(t => t.Token == token && t.ExpirationTime > DateTime.UtcNow);
            if (time == null)
            {
                return 408;
            }

            var user = await _context.PasswordResetToken
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == token && t.IsUsed == false);
            if (user == null)
            {
                return 400;
            }

            Console.WriteLine("Eail=> " + user.User.Email);
            Console.WriteLine("Salt=> " + user.User.Salt);
            Console.WriteLine("newPassword=> " + newPassword);

            var hashedPassword = PasswordHelper.HashPassword(newPassword, user.User.Salt);
            user.User.Password = hashedPassword;
            user.IsUsed = true;
            await _context.SaveChangesAsync();

            return 200;

        }

        public async Task<bool> SetPassword(int id, string password)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                var salt = PasswordHelper.GenerateSalt();
                var hashedPassword = PasswordHelper.HashPassword(password, salt);
                user.Password = hashedPassword;
                user.Salt = salt;
                await _context.SaveChangesAsync();

                return true;
            }

            return false;


        }

    }
}

