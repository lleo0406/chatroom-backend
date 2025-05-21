using BackEnd.Common;
using BackEnd.Dto.Chat.RequestDto;
using BackEnd.Models;
using BackEnd.Repositories.ChatRepository;
using BackEnd.Services.ImageService;
using SixLabors.ImageSharp.Formats.Webp;
using System.Reflection.Metadata;

namespace BackEnd.Services.ChatService
{
    public class ChatService(IChatRepository chatRepository, IImageService imageService) : IChatService
    {
        private readonly IChatRepository _chatRepository = chatRepository;
        private readonly IImageService _imageService = imageService;

        public async Task<Dictionary<string, object>> CreateChatRoom(int userId1, int userId2)
        {
            var response = new Dictionary<string, object>();

            var chatDto = await _chatRepository.GetChatById(userId1, userId2);

            if (chatDto.ChatRoomId == 0)
            {
                chatDto = await _chatRepository.CreateChatRoom(userId1, userId2);
            }

            response[Constants.CODE] = Constants.SUCCESS;
            response[Constants.MESSAGE] = Constants.SUCCESS_MESSAGE;
            response[Constants.DATA] = chatDto;

            return response;
        }

        public async Task<Dictionary<string, object>> CreateGroup(int userId, string name, IFormFile photo, List<int> members)
        {
            var response = new Dictionary<string, object>();
            var photoPath = string.Empty;

            if (photo != null)
            {
                photoPath = await _imageService.UploadImageAsync(photo);
            }

            await _chatRepository.CreateGroup(userId, name, photoPath, members);

            response[Constants.CODE] = Constants.SUCCESS;
            response[Constants.MESSAGE] = Constants.SUCCESS_MESSAGE;

            return response;
        }


        public async Task<Dictionary<string, object>> GetChatList(int userId)
        {
            var response = new Dictionary<string, object>();

            var chatRoom = await _chatRepository.GetChatListById(userId);

            response.Add(Constants.CODE, Constants.SUCCESS);
            response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            response.Add(Constants.DATA, chatRoom);

            return response;
        }

        public async Task<Dictionary<string, object>> ExitGroup(int chatRoomId, int userId)
        {
            var response = new Dictionary<string, object>();
            var result = await _chatRepository.ExitGroup(chatRoomId, userId);
            if (result)
            {
                response.Add(Constants.CODE, Constants.SUCCESS);
                response.Add(Constants.MESSAGE, Constants.SUCCESS_MESSAGE);
            }
            else
            {
                response.Add(Constants.CODE, Constants.FAIL);
                response.Add(Constants.MESSAGE, Constants.FAIL_MESSAGE);
            }
            return response;
        }




    }
}
