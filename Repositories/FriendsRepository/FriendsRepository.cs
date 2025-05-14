using BackEnd.Dto.Friends.ResponseDto;
using BackEnd.Enums;
using BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Repositories.FriendsRepository
{
    public class FriendsRepository : IFriendsRepository
    {
        private readonly IChatroomContext _context;

        public FriendsRepository(IChatroomContext context)
        {
            this._context = context;
        }

        public async Task<bool> AlreadyFriend(int requesterId, int responderId)
        {
            var result = await _context.Friends.FirstOrDefaultAsync(f =>
                (f.RequesterId == requesterId && f.ResponderId == responderId && f.Status == FriendStatus.Accepted) ||
                (f.RequesterId == responderId && f.ResponderId == requesterId && f.Status == FriendStatus.Accepted)
            );

            return result != null;
        }

        public async Task<bool> AddingFriend(int requesterId, int responderId)
        {
            var result = await _context.Friends.FirstOrDefaultAsync(f =>
                f.RequesterId == responderId &&
                f.ResponderId == requesterId &&
                f.Status == FriendStatus.Pending
            );

            if (result != null)
            {
                result.RequesterId = requesterId;
                result.ResponderId = responderId;

                _context.Friends.Update(result);
                await _context.SaveChangesAsync();
            }

            return result != null;
        }

        public async Task<bool> PendingFriend(int requesterId, int responderId)
        {
            var result = await _context.Friends.FirstOrDefaultAsync(f =>
                (f.RequesterId == requesterId && f.ResponderId == responderId && f.Status == FriendStatus.Pending)
            );

            return result != null;
        }

        public async Task<bool> AddFriend(int requesterId, int responderId)
        {
            var friend = new Friends
            {
                RequesterId = requesterId,
                ResponderId = responderId,
                Status = FriendStatus.Pending, // 0: pending, 1: accepted
                RequestedAt = DateTime.UtcNow
            };

            await _context.Friends.AddAsync(friend);
            var result = await _context.SaveChangesAsync();

            return result > 0;
        }

        public async Task<List<FriendsDto>> FriendsRequests(int userId)
        {
            var result = await _context.Users
                .Where(u => u.Id == userId)
                .SelectMany(u => u.ReceivedFriendRequests
                .Where(f => f.Status == 0)
                .Select(f => new FriendsDto
                {
                    Id = f.Id,
                    RequesterId = f.RequesterId,
                    RequesterDisplayName = f.Requester.DisplayName,
                    RequesterPicture = f.Requester.Picture
                }))
                .ToListAsync();

            return result;
        }

        public async Task<bool> AcceptFriend(int requesterId, int responderId)
        {
            //取得資料
            var friend = await _context.Friends.FirstOrDefaultAsync(f =>
                f.RequesterId == requesterId &&
                f.ResponderId == responderId &&
                f.Status == FriendStatus.Pending
            );
            if (friend != null)
            {
                friend.Status = FriendStatus.Accepted;
                _context.Friends.Update(friend);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> RejectFriend(int requesterId, int responderId)
        {
            //取得資料
            var friend = await _context.Friends.FirstOrDefaultAsync(f =>
                f.RequesterId == requesterId &&
                f.ResponderId == responderId &&
                f.Status == FriendStatus.Pending
            );
            if (friend != null)
            {
                _context.Friends.Remove(friend);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Dictionary<string, object>> GetFriendsAndGroupsById(int userId)
        {
            var response = new Dictionary<string, object>();
            Console.WriteLine(userId);
            var friendList = await _context.Friends
                .Where(f => (f.RequesterId == userId || f.ResponderId == userId) && f.Status == FriendStatus.Accepted)
                .Select(f => new FriendsDto
                {
                    Id = f.Id,
                    RequesterId = f.RequesterId == userId ? f.ResponderId: f.RequesterId,
                    RequesterDisplayName = f.RequesterId == userId ? f.Responder.DisplayName : f.Requester.DisplayName,
                    RequesterPicture = f.RequesterId == userId ? f.Responder.Picture : f.Requester.Picture,
                }).ToListAsync();



            response.Add("friendList", friendList);

            var groupList = await _context.ChatRoomMembers
                .Where(crm => crm.UserId == userId && crm.ChatRoom.IsPrivate == false)
                .Select(crm => new GroupDto
                {
                    ChatRoomId = crm.ChatRoom.Id,
                    Name = crm.ChatRoom.Name,
                    Picture = crm.ChatRoom.Picture
                }).ToListAsync();

            response.Add("groupList", groupList);

            return response;
        }

        public async Task<bool> DeleteFriend(int userId, int friendId)
        {
            var friend = await _context.Friends.FirstOrDefaultAsync(f =>
                ((f.RequesterId == userId && f.ResponderId == friendId) ||
                 (f.RequesterId == friendId && f.ResponderId == userId)) &&
                 f.Status == FriendStatus.Accepted
            );

            if (friend != null)
            {
                _context.Friends.Remove(friend);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }



    }
}
