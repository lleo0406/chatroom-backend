using BackEnd.Models;

namespace BackEnd.Dto.Friends.ResponseDto
{
    public class FriendsDto
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public string RequesterDisplayName { get; set; } = string.Empty;
        public string RequesterPicture { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"Id: {Id}, RequesterId: {RequesterId}, RequesterDisplayName: {RequesterDisplayName}";
        }
    }


}
