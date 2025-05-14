using BackEnd.Dto.Chat.RequestDto;
using BackEnd.Dto.Chat.ResponseDto;
using BackEnd.Services.ChatService;
using BackEnd.Services.MessagesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text.Json;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("chatroom/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly IMessagesService _messagesService;

        public ChatController(IChatService chatService, IMessagesService messages)
        {
            this._chatService = chatService;
            this._messagesService = messages;
        }

        [Authorize]
        [HttpPost("startChat")]
        public async Task<IActionResult> StartChat(CreatePrivateChatDto request)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result =await _chatService.CreateChatRoom(int.Parse(id), request.Id);

            return Ok(result);
        }

        //建立群組
        [Authorize]
        [HttpPost("createGroup")]
        public async Task<IActionResult> CreateGroup([FromForm] CreateGroupChatDto request)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var selectedFriendIds = JsonSerializer.Deserialize<List<int>>(request.SelectedFriends);
            var result = await _chatService.CreateGroup(int.Parse(id), request.GroupName, request.Photo, selectedFriendIds);

            return Ok(result);
        }


        //查詢全部聊天室
        [Authorize]
        [HttpGet("getChatList")]
        public async Task<IActionResult> GetChatList()
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _chatService.GetChatList(int.Parse(id));

            return Ok(result);
        }

        //查詢單一聊天室
        [Authorize]
        [HttpPost("getChatMessages")]
        public async Task<IActionResult> GetChatMessages(ChatDto request)
        {
            var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            var result = await _messagesService.GetChatMessages(request.ChatRoomId, int.Parse(id));

            return Ok(result);
        }

        [Authorize]
        [HttpPost("exitGroup")]
        public async Task<IActionResult> ExitGroup(ChatDto request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Console.WriteLine(userId);
            Console.WriteLine(request.ChatRoomId);

            var result = await _chatService.ExitGroup(request.ChatRoomId, int.Parse(userId));

            return Ok(result);
        }


        


    }
}
