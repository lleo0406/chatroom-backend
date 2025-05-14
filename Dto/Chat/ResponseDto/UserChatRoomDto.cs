namespace BackEnd.Dto.Chat.ResponseDto
{
    public class UserChatRoomDto
    {
        public int ChatRoomId { get; set; }
        public string Name { get; set; } = null!;
        public string? Picture { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }

}
