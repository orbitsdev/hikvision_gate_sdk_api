namespace HikvisionAPI.Models
{
    public class DoorControlRequest
    {
        public string Ip { get; set; } = string.Empty;
        public int Port { get; set; } = 8000;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Door index (1 = first door)
        /// -1 = all doors
        /// </summary>
        public int GatewayIndex { get; set; } = 1;

        /// <summary>
        /// Command:
        /// 0 = Close
        /// 1 = Open
        /// 2 = Remain Open
        /// 3 = Remain Closed
        /// </summary>
        public int Command { get; set; } = 1;
    }
}
