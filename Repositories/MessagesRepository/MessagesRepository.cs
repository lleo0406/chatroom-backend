using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Enums;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.MessagesRepository
{
    public class MessagesRepository(IChatroomContext context) : IMessagesRepository
    {
        private readonly IChatroomContext _context = context;

        public async Task<Dictionary<string, object>> GetChatMessagesById(int chatRoomId, int userId)
        {
            var response = new Dictionary<string, object>();

            var chatRoomData = await _context.ChatRoom
               .Where(c => c.Id == chatRoomId)
               .Select(c => new
               {
                   IsPrivate = c.IsPrivate,
                   Name = c.Name,
                   Picture = c.Picture,
                   Members = c.Members.Select(m => new
                   {
                       m.UserId,
                       m.User.DisplayName,
                       m.User.Picture
                   }).ToList()
               })
               .FirstOrDefaultAsync();

            if (chatRoomData.IsPrivate)
            {
                var otherUser = chatRoomData.Members.FirstOrDefault(m => m.UserId != userId);

                if (otherUser != null)
                {
                    response.Add("chatInfo", new
                    {
                        Id = chatRoomId,
                        Name = otherUser.DisplayName,
                        Picture = otherUser.Picture,
                        Private = chatRoomData.IsPrivate
                    });
                }
            }
            else
            {
                response.Add("chatInfo", new
                {
                    Id = chatRoomId,
                    Name = chatRoomData.Name,
                    Picture = chatRoomData.Picture,
                    Private = chatRoomData.IsPrivate
                });
            }

            var messages = await _context.Messages
                 .Where(m => m.ChatRoomId == chatRoomId)
                 .OrderBy(m => m.SendAt)
                 .Select(m => new ChatMessagesDto
                 {
                     SenderId = m.SenderId,
                     SenderName = m.Sender.DisplayName,
                     SenderPicture = m.Sender.Picture,
                     Message = m.Content,
                     MessageType = m.MessageType,
                     FileUrl = m.FileUrl,
                     SendAt = m.SendAt,
                     UserId = userId,
                 })
                 .ToListAsync();

            response.Add("messages", messages);
            return response;
        }

        public async Task<ChatMessagesDto> SendMessage(int chatRoomId, int userId, string message)
        {
            var chatMessage = new Messages
            {
                Content = message,
                MessageType = MessageType.Text,
                ChatRoomId = chatRoomId,
                SenderId = userId,
                SendAt = DateTime.UtcNow
            };

            _context.Messages.Add(chatMessage);
            await _context.SaveChangesAsync();

            var otherUser = await _context.ChatRoom
                .Include(c => c.Members)
                .Where(c => c.IsPrivate == true && c.Id == chatRoomId)
                .Select(c => c.Members.FirstOrDefault(m => m.UserId != userId))
                .FirstOrDefaultAsync();

            var messages = await _context.Messages
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.SendAt)
                .Select(m => new ChatMessagesDto
                {
                    SenderId = m.SenderId,
                    SenderName = m.Sender.DisplayName,
                    SenderPicture = m.Sender.Picture,
                    Message = m.Content,
                    MessageType = m.MessageType,
                    FileUrl = m.FileUrl,
                    SendAt = m.SendAt,
                    UserId = userId,
                    OtherUserId = otherUser != null ? otherUser.UserId : (int?)null
                })
                .FirstOrDefaultAsync();



            return messages;
        }

        public async Task<bool> IsPrivateOrNotActivation(int chatRoomId, int userId)
        {
            var chatRoom = await _context.ChatRoom
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == chatRoomId && c.IsPrivate);

            var otherMember = chatRoom?.Members.FirstOrDefault(m => m.UserId != userId);

            return otherMember?.Activation == ActivationStatus.NotActivated;
        }


        public async Task<User> ChangeActiveStatus(int chatRoomId, int userId)
        {
            var member = await _context.ChatRoom
                .Where(c => c.Id == chatRoomId)
                .SelectMany(c => c.Members)
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId != userId);

            Console.WriteLine("member=> " + member.User);
            if (member == null) return null;

            member.Activation = ActivationStatus.Activated;
            await _context.SaveChangesAsync();
            return member.User;
        }

    }
}
