

using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Models;

namespace BackEnd.Services.MessagesService
{
    public interface IMessagesService
    {
        Task<User> ChangeActiveStatus(int chatRoomId, int userId);
        Task<Dictionary<string, object>> GetChatMessages(int chatRoomId, int id);
        Task<bool> IsPrivateOrNotActivation(int chatRoomId, int userId);
        Task<ChatMessagesDto> SendMessage(int chatRoomId, int userId, string message);
    }
}
