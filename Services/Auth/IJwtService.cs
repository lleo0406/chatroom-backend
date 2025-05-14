namespace BackEnd.Services.Auth
{
    public interface IJwtService
    {
        string GenerateToken(string userId, string userRole, string email);
    }
}
