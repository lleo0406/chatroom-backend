using BackEnd.Dto.Error.ResponsesDto;
using Newtonsoft.Json;
using System.Net.Mail;

namespace BackEnd.Exceptions
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ILogger<ExceptionMiddleware> _logger;

        private readonly IHostEnvironment _env;


        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }


        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"[Unhandled Exception] {Message}", ex.Message);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    AppException => StatusCodes.Status400BadRequest,          
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized, 
                    KeyNotFoundException => StatusCodes.Status404NotFound,   
                    _ => StatusCodes.Status500InternalServerError             
                };

                var response = new ErrorResponse();

                switch (ex)
                {
                    case AppException appEx:
                        response.Code = appEx.ErrorCode;   
                        response.Message = appEx.Message;  
                        break;

                    case UnauthorizedAccessException:
                        response.Code = 401;
                        response.Message = "未授權的存取";
                        break;

                    case KeyNotFoundException:
                        response.Code = 404;
                        response.Message = "找不到資源";
                        break;

                    default:
                        response.Code = 500;
                        response.Message = "伺服器發生錯誤，請稍後再試";
                        break;
                }

                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);
            }
        }

    }
}



