using BackEnd.Dto.Friends.ResponseDto;
using BackEnd.Models;

namespace BackEnd.Repositories.FriendsRepository
{
    public interface IFriendsRepository
    {
        Task<bool> AlreadyFriend(int requesterId, int responderId);
        Task<bool> AddFriend(int requesterId, int responderId);
        Task<bool> AddingFriend(int requesterId, int responderId);
        Task<bool> PendingFriend(int requesterId, int responderId);
        Task<List<FriendsDto>> FriendsRequests(int userId);
        Task<bool> AcceptFriend(int requesterId, int responderId);
        Task<bool> RejectFriend(int requesterId, int responderId);
        Task<Dictionary<string, object>> GetFriendsAndGroupsById(int userId);
        Task<bool> DeleteFriend(int userId, int frinedId);
    }
}
