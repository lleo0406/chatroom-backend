using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BackEnd.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(string userId, string userName, string email)
        {
            // 讀取 appsettings.json 中的 Jwt 設定區段
            var jwtSettings = _configuration.GetSection("Jwt");

            // 建立 JWT 內部的使用者聲明 (Claims)，包含使用者識別碼與角色
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // 使用者 ID（識別用）
                new Claim(ClaimTypes.Name, userName),                     // 使用者角色（授權用）
                new Claim(ClaimTypes.Email, email),                     // 使用者電子郵件（授權用）
                new Claim(ClaimTypes.Role, "User")                       // 使用者角色（授權用）
            };

            // 將設定中的金鑰轉為對稱加密金鑰物件（SymmetricSecurityKey）
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            // 使用 HmacSha256 演算法建立簽章憑證
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // 建立 JWT Token 物件，設定發行者、受眾、有效期限與簽章等資訊
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],                         // Token 發行者
                audience: jwtSettings["Audience"],                     // Token 受眾
                claims: claims,                                        // 內含的使用者資料
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpireMinutes"])),  // 設定過期時間
                signingCredentials: creds                              // 簽章憑證
            );

            // 將 Token 轉為字串並回傳（會回傳給前端）
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
