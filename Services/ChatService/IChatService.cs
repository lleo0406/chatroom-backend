
namespace BackEnd.Services.ChatService
{
    public interface IChatService
    {
        Task<Dictionary<string, object>> GetChatList(int userId);
        Task<Dictionary<string, object>> CreateChatRoom(int userId1, int userId2);
        Task<Dictionary<string, object>> CreateGroup(int userId, string name, IFormFile picture, List<int> members);
        Task<Dictionary<string, object>> ExitGroup(int chatRoomId, int v);
    }
}
