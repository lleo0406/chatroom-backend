using BackEnd.Common;
using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Models;
using BackEnd.Repositories.MessagesRepository;

namespace BackEnd.Services.MessagesService
{
    public class MessagesService(IMessagesRepository _messagesRepository) : IMessagesService
    {
        private readonly IMessagesRepository _messagesRepository = _messagesRepository;

        public async Task<Dictionary<string, object>> GetChatMessages(int chatRoomId, int userId)
        {
            var response = new Dictionary<string, object>();

            var chatMessages = await _messagesRepository.GetChatMessagesById(chatRoomId, userId);

            response.Add(Constants.CODE, Constants.SUCCESS);
            response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            response.Add(Constants.DATA, chatMessages);
            return response;
        }

        public async Task<ChatMessagesDto> SendMessage(int chatRoomId, int userId, string message)
        {
            
            var messages = await _messagesRepository.SendMessage(chatRoomId, userId, message);
            return messages;
        }

        public async Task<bool> IsPrivateOrNotActivation(int chatRoomId, int userId)
        {
            var IsPrivateOrNotActivation = await _messagesRepository.IsPrivateOrNotActivation(chatRoomId, userId);
            return IsPrivateOrNotActivation;
        }

        public async Task<User> ChangeActiveStatus(int chatRoomId, int userId)
        {
            var changeActiveStatus = await _messagesRepository.ChangeActiveStatus(chatRoomId, userId);
            return changeActiveStatus;
        }




    }
}
