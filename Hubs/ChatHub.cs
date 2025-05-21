using BackEnd.Dto.Messages;
using BackEnd.Services.MessagesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BackEnd.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public IMessagesService _messagesService { get; }

        public ChatHub(IMessagesService messagesService)
        {
            _messagesService = messagesService;
        }

        private string GetUserId()
        {
            return Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        public async Task SendMessage(SendMessageDto message)
        {
            var senderId = int.Parse(GetUserId());

            var IsPrivateOrNotActivation = await _messagesService.IsPrivateOrNotActivation(message.ChatRoomId, senderId);
            var result = await _messagesService.SendMessage(message.ChatRoomId, senderId, message.Message);

            if (IsPrivateOrNotActivation)
            {
                var user = await _messagesService.ChangeActiveStatus(message.ChatRoomId, senderId);
                await Clients.Group(user.Id.ToString()).SendAsync("createChat", null);
            }

            await Clients.Group(message.ChatRoomId.ToString()).SendAsync("ReceiveMessage", new
            {
                chatRoomId = message.ChatRoomId,
                senderId,
                senderName = result.SenderName,
                senderPicture = result.SenderPicture,
                message = message.Message,
                fileUrl = result.FileUrl,
                messageType = result.MessageType,
                sendAt = result.SendAt,
            });

        }

        public async Task JoinRoom(int id)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, id.ToString());
        }

        public async Task LeaveRoom(int id)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, id.ToString());
        }

    }
}
