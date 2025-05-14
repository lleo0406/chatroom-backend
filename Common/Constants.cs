namespace BackEnd.Common
{
    public static class Constants
    {
        public const int SUCCESS = 200;
        public const int GOOGLE_REGISTER_SUCCESS = 201;
        public const int FAIL = 400;
        public const int TIMEOUT = 408;
        public const int PENDING = 202;
        public const int SERVER_ERROR = 500;
        public const int NOT_FOUND = 404;
        public const int PASSWORD_ERROR = 403;
        public const int DATA_EXISTS = 409;
        public const int UNAUTHORIZED = 401; 
        public const int CANNOT_ADD_SELF = 460;


        public const string SUCCESS_MESSAGE = "操作成功";
        public const string GOOGLE_REGISTER_SUCCESS_MESSAGE = "Google註冊成功";
        public const string FAIL_MESSAGE = "操作失敗";
        public const string TIMEOUT_MESSAGE = "請求逾時，請稍後再試";
        public const string SERVER_ERROR_MESSAGE = "內部伺服器錯誤";
        public const string NOT_FOUND_MESSAGE = "找不到資源";
        public const string PASSWORD_ERROR_MESSAGE = "密碼錯誤";
        public const string DATA_EXISTS_MESSAGE = "資料已存在";
        public const string UNAUTHORIZED_MESSAGE = "未經授權的存取";
        public const string CANNOT_ADD_SELF_MESSAGE = "無法將自己新增為好友";
        public const string PENDING_MESSAGE = "請求處理中";


        public const string CODE = "code";
        public const string MESSAGE = "message";
        public const string DATA = "data";
    }
}
