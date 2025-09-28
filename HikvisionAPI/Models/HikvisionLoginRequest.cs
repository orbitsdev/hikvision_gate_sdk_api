using Microsoft.AspNetCore.Mvc;

namespace HikvisionAPI.Models
{
    public class HikvisionLoginRequest
    {
        public string Ip { get; set; } = string.Empty;
        public int Port { get; set; } = 8000;
        public string Username { get; set; } = "admin";
        public string Password { get; set; } = string.Empty;
    }
}
