using BackEnd.Common;
using BackEnd.Models;
using BackEnd.Repositories.UserRepository;
using BackEnd.Services.Auth;

namespace BackEnd.Services.GoogleService
{
    public class GoogleService : IGoogleService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public GoogleService(IUserRepository userRepository, IJwtService jwtService) {
            this._userRepository = userRepository;
            this._jwtService = jwtService;
        }

        public async Task<Dictionary<string, object>> GoogleLogin(string googleId, string email, string name)
        {
            var response = new Dictionary<string, object>();

            var user = await _userRepository.GetUserByEmail(email);
            string? token;

            if (user == null)
            {
                var registerUser = await _userRepository.RegisterGoogleAccount(email, name, googleId);

                token = _jwtService.GenerateToken(
                        registerUser.Id.ToString(),
                        registerUser.DisplayName ?? "",
                        registerUser.Email ?? ""
                    );

                var result = new Dictionary<string, object>
                {
                    { "token", token },
                    { "userInfo", registerUser }
                };

                response.Add(Constants.CODE, Constants.GOOGLE_REGISTER_SUCCESS);
                response.Add(Constants.MESSAGE, Constants.GOOGLE_REGISTER_SUCCESS_MESSAGE);
                response.Add(Constants.DATA, result);
                return response;
            }

            if(user.GoogleId == null)
            {
                await _userRepository.UpdateGoogleId(user.Id, googleId);

                token = _jwtService.GenerateToken(
                        user.Id.ToString(),
                        user.DisplayName ?? "",
                        user.Email ?? ""
                    );

                var result = new Dictionary<string, object>
                {
                    { "token", token },
                    { "userInfo", user }

                };

                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
                response.Add(Constants.DATA, result);
                return response;

            }
            
            if(user.GoogleId != null)
            {
                if(user.GoogleId != googleId)
                {
                    response.Add(Constants.CODE, Constants.FAIL);
                    response.Add(Constants.MESSAGE, "登入失敗");
                }
                else
                {
                    token = _jwtService.GenerateToken(
                        user.Id.ToString(),
                        user.DisplayName ?? "",
                        user.Email ?? ""
                    );

                    var result = new Dictionary<string, object>
                    {
                        { "token", token },
                        { "userInfo", user }
                    };
                    response.Add(Constants.CODE, Constants.SUCCESS);
                    response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
                    response.Add(Constants.DATA, result);
                }
            }
            return response;

        }
    }
}
