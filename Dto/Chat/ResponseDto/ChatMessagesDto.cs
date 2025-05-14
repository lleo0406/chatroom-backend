using BackEnd.Enums;

namespace BackEnd.Dto.Chat.ResponseDto
{
    public class ChatMessagesDto
    {
        public int SenderId { get; set; }
        public string SenderName { get; set; } = null!;
        public string SenderPicture { get; set; } = null!;
        public string? Message { get; set; }
        public string? FileUrl { get; set; }
        public MessageType MessageType { get; set; }
        public DateTime SendAt { get; set; }
        public int UserId { get; set; }
        public int? OtherUserId { get; set; }


        public override string ToString()
        {
            return $"SenderId: {SenderId}, SenderName: {SenderName}, SenderPicture: {SenderPicture}, Message: {Message}, FileUrl: {FileUrl}, MessageType: {MessageType}, SendAt: {SendAt}, UserId: {UserId}, OtherUserId: {OtherUserId}";
        }

    }
}
