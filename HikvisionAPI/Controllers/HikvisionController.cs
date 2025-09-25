using Microsoft.AspNetCore.Mvc;
using HikvisionAPI.Models;
using HikvisionAPI.Services;

namespace HikvisionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HikvisionController : ControllerBase
    {
        private readonly HikvisionService _hikvisionService;

        public HikvisionController()
        {
            _hikvisionService = new HikvisionService();
        }

        /// <summary>
        /// Remotely controls the door (open/close/lock/unlock).
        /// </summary>
        /// <param name="request">Door control details</param>
        /// <returns>Success or failure result</returns>
        [HttpPost("open-door")]
        public IActionResult OpenDoor([FromBody] DoorControlRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            bool success = _hikvisionService.OpenDoor(request, out string error);

            if (!success)
            {
                return StatusCode(400, new
                {
                    message = "Failed to control door",
                    error
                });
            }

            return Ok(new
            {
                message = "Door command sent successfully"
            });
        }

        [HttpPost("device-info")]
        public IActionResult GetDeviceInfo([FromBody] DeviceInfoRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid request");

            var result = _hikvisionService.GetDeviceInfo(request, out string error);
            if (result == null)
            {
                return StatusCode(400, new
                {
                    message = "Failed to get device info",
                    error
                });
            }

            return Ok(result);
        }

        [HttpGet("device-info-test")]
        public IActionResult GetDeviceInfoTest()
        {
            var request = new DeviceInfoRequest
            {
                Ip = "192.170.80.251",
                Port = 8000,
                Username = "admin",
                Password = "Hikvision_2025"
            };

            var result = _hikvisionService.GetDeviceInfo(request, out string error);

            if (result == null)
            {
                return StatusCode(400, new
                {
                    message = "Failed to get device info",
                    error
                });
            }

            return Ok(result); // return full object so you see what data is available
        }

        [HttpGet("open-door-test")]
        public IActionResult OpenDoorTest()
        {
            var request = new DoorControlRequest
            {
                Ip = "192.170.80.251",
                Port = 8000,
                Username = "admin",
                Password = "Hikvision_2025",
                GatewayIndex = 1,         // <-- Adjust index if needed
                Command = 1               // 1 = OPEN, 2 = CLOSE, etc.
            };

            bool success = _hikvisionService.OpenDoor(request, out string error);

            if (!success)
            {
                return StatusCode(400, new
                {
                    message = "Failed to open door",
                    error
                });
            }

            return Ok(new
            {
                message = "Door opened successfully"
            });
        }

    }


}
