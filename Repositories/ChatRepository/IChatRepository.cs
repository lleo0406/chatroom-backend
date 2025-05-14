
using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Models;

namespace BackEnd.Repositories.ChatRepository
{
    public interface IChatRepository
    {
        Task<ChatDto> CreateChatRoom(int userId1, int userId2);
        Task<List<UserChatRoomDto>> GetChatListById(int id);
        Task<ChatDto> GetChatById(int userId1, int userId2);
        Task<int> CreateGroup(int userId, string name, string picture, List<int> members);
        Task<bool> ExitGroup(int chatRoomId, int userId);
        Task<bool> DeleteChatRoom(int chatRoomId);
    }
}
