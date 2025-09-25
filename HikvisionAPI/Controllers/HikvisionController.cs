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
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid request" });

                bool success = _hikvisionService.OpenDoor(request, out string error);

                if (!success)
                {
                    return BadRequest(new
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Unexpected error while controlling door",
                    exception = ex.Message
                });
            }
        }

        /// <summary>
        /// Retrieves device information.
        /// </summary>
        /// <param name="request">Device connection details</param>
        /// <returns>Device info or error</returns>
        [HttpPost("device-info")]
        public IActionResult GetDeviceInfo([FromBody] DeviceInfoRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Invalid request" });

                var result = _hikvisionService.GetDeviceInfo(request, out string error);

                if (result == null)
                {
                    return BadRequest(new
                    {
                        message = "Failed to get device info",
                        error
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Unexpected error while fetching device info",
                    exception = ex.Message
                });
            }
        }

        [HttpGet("device-info-test")]
        public IActionResult GetDeviceInfoTest()
        {
            try
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
                    return BadRequest(new
                    {
                        message = "Failed to get device info",
                        error
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Unexpected error while fetching device info",
                    exception = ex.Message
                });
            }
        }

        [HttpGet("open-door-test")]
        public IActionResult OpenDoorTest()
        {
            try
            {
                var request = new DoorControlRequest
                {
                    Ip = "192.170.80.251",
                    Port = 8000,
                    Username = "admin",
                    Password = "Hikvision_2025",
                    GatewayIndex = 1,
                    Command = 1
                };

                bool success = _hikvisionService.OpenDoor(request, out string error);

                if (!success)
                {
                    return BadRequest(new
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
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Unexpected error while attempting to open the door",
                    exception = ex.Message
                });
            }
        }


    }


}
