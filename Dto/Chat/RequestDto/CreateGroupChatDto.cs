namespace BackEnd.Dto.Chat.RequestDto
{
    public class CreateGroupChatDto
    {
        public string GroupName { get; set; } = null!;
        public IFormFile? Photo { get; set; }
        public string? SelectedFriends { get; set; }

    }
}
