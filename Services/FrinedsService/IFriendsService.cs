namespace BackEnd.Services.FrinedsService
{
    public interface IFriendsService
    {
        Task<Dictionary<string, object>> AcceptFriend(int requesterId, int responderId);
        Task<Dictionary<string, object>> AddFriendById(int userId, string DisplayId);
        Task<Dictionary<string, object>> DeleteFriend(int userId, int friendId);
        Task<Dictionary<string, object>> FriendsRequests(int userId);
        Task<Dictionary<string, object>> GetFriendsAndGroupsById(int userId);
        Task<Dictionary<string, object>> RejectFriend(int requesterId, int responderId);
    }
}
