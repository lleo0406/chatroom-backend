using BackEnd.Dto.Messages;
using BackEnd.Exceptions;
using BackEnd.Services.MessagesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace BackEnd.Controllers
{
    [ApiController]
    [Route("chatroom/message")]
    public class MessagesController : ControllerBase
    {
        private readonly IMessagesService _messagesService;

        public MessagesController(IMessagesService messagesService)
        {
            this._messagesService = messagesService;
        }

    }
}
