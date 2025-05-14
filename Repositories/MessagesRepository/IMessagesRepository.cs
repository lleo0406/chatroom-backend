using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Models;

namespace BackEnd.Repositories.MessagesRepository
{
    public interface IMessagesRepository
    {
        Task<User> ChangeActiveStatus(int chatRoomId, int userId);
        Task<Dictionary<string, object>> GetChatMessagesById(int chatRoomId, int userId);
        Task<bool> IsPrivateOrNotActivation(int chatRoomId, int userId);
        Task<ChatMessagesDto> SendMessage(int chatRoomId, int userId, string message);
    }
}
