namespace BackEnd.Dto.Users.RequestDto
{
    public class UpdateUserRequest
    {
        public string? DisplayId { get; set; }
        public string? DisplayName { get; set; }
        public string? Picture { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? NewPassword { get; set; }
    }
    
}
