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
                return StatusCode(500, new
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
            var result = _hikvisionService.GetDeviceInfo(request, out string error);

            if (result == null)
            {
                return StatusCode(500, new
                {
                    message = "Failed to retrieve device info",
                    error
                });
            }

            return Ok(result); // Return raw struct as anonymous object
        }

    }


}
