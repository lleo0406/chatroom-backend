using BackEnd.Common;
using BackEnd.Hubs;
using BackEnd.Repositories.ChatRepository;
using BackEnd.Repositories.FriendsRepository;
using BackEnd.Repositories.UserRepository;
using Microsoft.AspNetCore.SignalR;
using System.Reflection.Metadata;

namespace BackEnd.Services.FrinedsService
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendsRepository _friendsRepository;
        private readonly IUserRepository _userRepository;
        private readonly IChatRepository _chatRepository;
        private readonly IHubContext<FriendNotificationHub> _hubContext;

        public FriendsService(IFriendsRepository friendsRepository, IUserRepository userRepository,IChatRepository chatRepository,IHubContext<FriendNotificationHub> hubContext)
        {
            this._friendsRepository = friendsRepository;
            this._userRepository = userRepository;
            this._chatRepository = chatRepository;
            this._hubContext = hubContext;
        }


        public async Task<Dictionary<string, object>> AddFriendById(int userId, string DisplayId)
        {
            var response = new Dictionary<string, object>();

            var friend = await _userRepository.GetUserByDisplayId(DisplayId);
            if (friend == null)
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);

                return response;
            }

            if (friend.Id == userId)
            {
                response.Add(Constants.CODE, Constants.CANNOT_ADD_SELF);
                response.Add(Constants.MESSAGE, Constants.CANNOT_ADD_SELF_MESSAGE);

                return response;
            }

            var already = await _friendsRepository.AlreadyFriend(userId, friend.Id);
            if (already)
            {
                response.Add(Constants.CODE, Constants.DATA_EXISTS);
                response.Add(Constants.MESSAGE, Constants.DATA_EXISTS_MESSAGE);

                return response;
            }

            var pending = await _friendsRepository.PendingFriend(userId, friend.Id);
            if (pending)
            {
                response.Add(Constants.CODE, Constants.PENDING);
                response.Add(Constants.MESSAGE, Constants.PENDING_MESSAGE);

                return response;
            }

            var adding = await _friendsRepository.AddingFriend(userId, friend.Id);
            if (adding)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);

                return response;
            }

            var addfriends = await _friendsRepository.AddFriend(userId, friend.Id);
            if (addfriends)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);

            }


            if (response["code"].ToString() == Constants.SUCCESS.ToString())
            {

                await _hubContext.Clients.Group($"user_{friend.Id}")
                    .SendAsync("ReceiveFriendRequest", new
                    {
                        FromUserId = userId
                    });
            }


            return response;
        }

        public async Task<Dictionary<string, object>> FriendsRequests(int userId)
        {
            var response = new Dictionary<string, object>();
            var friends = await _friendsRepository.FriendsRequests(userId);

            if (friends != null)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE); 
                response.Add(Constants.DATA, friends);
            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }

            return response;
        }

        public async Task<Dictionary<string, object>> AcceptFriend(int requesterId, int responderId)
        {
            var response = new Dictionary<string, object>();
            bool accept = await _friendsRepository.AcceptFriend(requesterId, responderId);
            if (accept)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }
            return response;
        }

        public async Task<Dictionary<string, object>> RejectFriend(int requesterId, int responderId)
        {
            var response = new Dictionary<string, object>();
            bool reject = await _friendsRepository.RejectFriend(requesterId, responderId);
            if (reject)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }
            return response;
        }

        public async Task<Dictionary<string, object>> GetFriendsAndGroupsById(int userId)
        {
            var response = new Dictionary<string, object>();

            var result = await _friendsRepository.GetFriendsAndGroupsById(userId);

            if(result != null)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
                response.Add(Constants.DATA, result);
            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }

            return response;
        }

        public async Task<Dictionary<string, object>> DeleteFriend(int userId, int friendId)
        {
            var response = new Dictionary<string, object>();

            var deleteFriend = await _friendsRepository.DeleteFriend(userId, friendId);

            if(deleteFriend)
            {
                var chatRoom = await _chatRepository.GetChatById(userId, friendId);

                var deleteChatRoom = await _chatRepository.DeleteChatRoom(chatRoom.ChatRoomId);

                if (deleteChatRoom)
                {
                    response.Add(Constants.CODE, Constants.SUCCESS);
                    response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
                }
            }
            else
            {
                response.Add(Constants.CODE, Constants.NOT_FOUND);
                response.Add(Constants.MESSAGE, Constants.NOT_FOUND_MESSAGE);
            }

            return response;



        }
    }
}
