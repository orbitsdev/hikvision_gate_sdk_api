using Microsoft.AspNetCore.Mvc;

namespace HikvisionAPI.Models
{
    public class HikvisionLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int UserId { get; set; }
        public object? DeviceInfo { get; set; }
        public int ErrorCode { get; set; }
    }
}
