namespace BackEnd.Dto.Users.RequestDto
{
    public class ForgotPasswordToken
    {
        public string? Email { get; set; }
        public string? Token { get; set; }
        public string? newPassword { get; set; }
    }
}
