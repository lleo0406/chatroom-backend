using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Enums;
using BackEnd.Exceptions;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.ChatRepository
{
    public class ChatRepository(IChatroomContext context) : IChatRepository
    {
        private readonly IChatroomContext _context = context;

        public async Task<ChatDto> GetChatById(int userId1, int userId2)
        {
            var chatRoomId = await _context.ChatRoomMembers
                .Where(m => m.ChatRoom.Type == ChatRoomType.Private && (m.UserId == userId1 || m.UserId == userId2))
                .GroupBy(m => m.ChatRoomId)
                .Where(g => g.Select(x => x.UserId).Distinct().Count() == 2)
                .Select(g => g.Key)
                .FirstOrDefaultAsync();

            var result = new ChatDto
            {
                ChatRoomId = chatRoomId
            };

            return result;
        }

        public async Task<ChatDto> CreateChatRoom(int userId1, int userId2)
        {
            var newChatRoom = new ChatRoom
            {
                Type = ChatRoomType.Private,
                CreatedAt = DateTime.UtcNow,
                IsPrivate = true,
                Members = new List<ChatRoomMembers>
                {
                    new ChatRoomMembers { UserId = userId1 ,Activation = ActivationStatus.Activated},
                    new ChatRoomMembers { UserId = userId2 }
                }
            };

            _context.ChatRoom.Add(newChatRoom);
            await _context.SaveChangesAsync();

            var result = new ChatDto
            {
                ChatRoomId = newChatRoom.Id 
            };

            return result;
        }

        public async Task<int> CreateGroup(int userId, string name, string picture, List<int> members)
        {

            var newChatRoom = new ChatRoom
            {
                Type = ChatRoomType.Group,
                CreatedAt = DateTime.UtcNow,
                IsPrivate = false,
                Name = name,
                Picture = picture,
                Members = members
                 .Append(userId) 
                 .Distinct()     
                 .Select(id => new ChatRoomMembers { UserId = id ,Activation = ActivationStatus.Activated })
                 .ToList()
                    };

            _context.ChatRoom.Add(newChatRoom);
            await _context.SaveChangesAsync();

            return newChatRoom.Id;

        }

        public async Task<List<UserChatRoomDto>> GetChatListById(int userId)
        {

            var query = await _context.ChatRoomMembers
                .Where(m => m.UserId == userId && m.Activation == ActivationStatus.Activated)
                .Select(m => new
                {
                    ChatRoomId = m.ChatRoomId,
                    IsPrivate = m.ChatRoom.IsPrivate,
                    ChatRoomName = m.ChatRoom.Name,
                    ChatRoomPicture = m.ChatRoom.Picture,
                    CreatedAt = m.ChatRoom.CreatedAt,

                    LastMessageContent = m.ChatRoom.Messages
                        .OrderByDescending(msg => msg.SendAt)
                        .Select(msg => msg.Content)
                        .FirstOrDefault(),

                    LastMessageSendAt = m.ChatRoom.Messages
                        .OrderByDescending(msg => msg.SendAt)
                        .Select(msg => (DateTime?)msg.SendAt)
                        .FirstOrDefault(),

                    OtherUser = m.ChatRoom.Members
                        .Where(cm => cm.UserId != userId)
                        .Select(cm => new
                        {
                            cm.User.DisplayName,
                            cm.User.Picture
                        })
                        .FirstOrDefault()
                })
                .OrderByDescending(x => x.LastMessageSendAt ?? x.CreatedAt)
                .ToListAsync();

            var result = query.Select(x => new UserChatRoomDto
            {
                ChatRoomId = x.ChatRoomId,
                Name = x.IsPrivate ? (x.OtherUser?.DisplayName ?? "未知用戶") : (x.ChatRoomName ?? "未命名群組"),
                Picture = x.IsPrivate ? (x.OtherUser?.Picture) : x.ChatRoomPicture,
                LastMessage = x.LastMessageContent,
                LastMessageTime = x.LastMessageSendAt ?? x.CreatedAt
            }).ToList();

            return result;
        }

        public async Task<bool> ExitGroup(int chatRoomId, int userId)
        {
            var chatRoom = await _context.ChatRoom
                .Include(c => c.Members)
                .FirstOrDefaultAsync(c => c.Id == chatRoomId);


            if (chatRoom != null)
            {
                var memberToRemove = chatRoom.Members.FirstOrDefault(m => m.UserId == userId);


                if (memberToRemove != null)
                {
                    _context.ChatRoomMembers.Remove(memberToRemove);
                    await _context.SaveChangesAsync();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> DeleteChatRoom(int chatRoomId)
        {
            var chatRoom = await _context.ChatRoom
                .FirstOrDefaultAsync(m => m.Id == chatRoomId);

           if(chatRoom != null)
            {
                _context.ChatRoom.Remove(chatRoom);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }


    }
}
