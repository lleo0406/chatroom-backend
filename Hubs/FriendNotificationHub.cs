using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace BackEnd.Hubs
{
    [Authorize]
    public class FriendNotificationHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            return base.OnConnectedAsync();
        }
    }
}
