using BackEnd.Dto.Friends.RequestDto;
using BackEnd.Services.FrinedsService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("chatroom/[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendsService _friendsService;

        public FriendsController(IFriendsService friendsService)
        {
            this._friendsService = friendsService;
        }

        [HttpPost]
        [Authorize]
        [Route("addFriend")]
        public async Task<IActionResult> AddFreind(AddFriend request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            var result = await _friendsService.AddFriendById(int.Parse(userId), request.DisplayId);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("getFriendsList")]
        public async Task<IActionResult> GetFriendsList()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            var result = await _friendsService.GetFriendsAndGroupsById(int.Parse(userId));


            return Ok(result);
        }

        [Authorize]
        [HttpGet("getFriendsRequests")]
        public async Task<IActionResult> GetFriendsRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }
            var result = await _friendsService.FriendsRequests(int.Parse(userId));
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [Route("acceptFriend")]
        public async Task<IActionResult> AcceptFriend(AcceptRejectFriend request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            var result = await _friendsService.AcceptFriend(request.Id, int.Parse(userId));

            return Ok(result);

        }

        [Authorize]
        [HttpPost("rejectFriend")]
        public async Task<IActionResult> RejectFriend(AcceptRejectFriend request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }


            var result = await _friendsService.RejectFriend(request.Id, int.Parse(userId));

            return Ok(result);
        }

        [HttpPost]
        [Route("deleteFriend")]
        public async Task<IActionResult> DeleteFriend(DeleteFreindRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized("系統異常，請稍後再試");
            }

            var result = await _friendsService.DeleteFriend(int.Parse(userId),request.FriendId);

            return Ok(result);
        }

    }
}
