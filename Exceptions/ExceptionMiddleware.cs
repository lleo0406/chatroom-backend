using BackEnd.Dto.Error.ResponsesDto;
using Newtonsoft.Json;
using System.Net.Mail;

namespace BackEnd.Exceptions
{
    /// <summary>
    /// 全域例外處理 Middleware，攔截所有未處理例外，並回傳統一錯誤格式。
    /// </summary>
    public class ExceptionMiddleware
    {
        // 儲存下一個 Middleware 的委派（即呼叫鏈的下一站）
        private readonly RequestDelegate _next;

        // 用來記錄錯誤訊息到 Console 或 Log 系統
        private readonly ILogger<ExceptionMiddleware> _logger;

        // 用來判斷是否為開發環境（用來顯示堆疊資訊）
        private readonly IHostEnvironment _env;

        /// <summary>
        /// 建構式，注入所需的服務。
        /// </summary>
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        /// <summary>
        /// Middleware 執行邏輯：捕捉所有未處理例外，統一格式回傳。
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 呼叫下一個 Middleware，讓請求繼續往下傳遞
                await _next(context);
            }
            catch (Exception ex)
            {
                // 將例外錯誤寫入 Log，包含錯誤訊息與堆疊資訊
                _logger.LogError(ex,"[Unhandled Exception] {Message}", ex.Message);

                // 設定回應格式與狀態碼
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = ex switch
                {
                    AppException => StatusCodes.Status400BadRequest,          // 自訂錯誤回傳 400
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized, // 未授權錯誤 401
                    KeyNotFoundException => StatusCodes.Status404NotFound,    // 找不到資源 404
                    _ => StatusCodes.Status500InternalServerError             // 其他錯誤回傳 500
                };

                // 建立錯誤回應物件
                var response = new ErrorResponse();

                // 根據例外類型填入相對應的錯誤資訊
                switch (ex)
                {
                    case AppException appEx:
                        response.Code = appEx.ErrorCode;   // 自訂錯誤代碼
                        response.Message = appEx.Message;  // 錯誤訊息
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

                // 將錯誤物件序列化為 JSON 並寫入回應
                var json = JsonConvert.SerializeObject(response);
                await context.Response.WriteAsync(json);
            }
        }

    }
}



