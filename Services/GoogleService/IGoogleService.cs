
namespace BackEnd.Services.GoogleService
{
    public interface IGoogleService
    {
        Task<Dictionary<string, object>> GoogleLogin(string googleId, string email, string name);
    }
}
