namespace BackEnd.Dto.Error.ResponsesDto
{
    public class ErrorResponse
    {
        public int Code { get; set; } = 500;
        public string Message { get; set; } = "伺服器錯誤，請稍後再試";
    }

}
