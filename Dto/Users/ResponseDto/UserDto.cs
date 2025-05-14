using Humanizer;

namespace BackEnd.Dto.Users.ResponseDto
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? Email { get; set; }

        public string? Password { get; set; }
        public string? DisplayId { get; set; }
        public string? DisplayName { get; set; }
        public string? Picture { get; set; }

        public string Salt { get; set; }
        public DateTime CreatedAt { get; set; }


        public UserDto()
        {
        }
        public UserDto(int id, string? email, string? displayId, string? displayName, string? picture, DateTime createdAt)
        {
            Id = id;
            Email = email;
            DisplayId = displayId;
            DisplayName = displayName;
            Picture = picture;
            CreatedAt = createdAt;
        }


    }
}
