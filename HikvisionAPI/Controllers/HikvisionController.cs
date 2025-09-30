using HikvisionAPI.Models;
using HikvisionAPI.Properties;
using HikvisionAPI.SdkInterop;
using HikvisionAPI.Services;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        [HttpPost("login")]
        public ActionResult<HikvisionLoginResponse> Login()
        {
            var response = new HikvisionLoginResponse();


            if (!HikvisionSdk.NET_DVR_Init())
            {
                response.Success = false;
                response.ErrorCode = 3;
                response.Message = "NET_DVR_Init failed";
                return BadRequest(response);
            }


            // Prepare login structs
            //var struLoginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
            //{
            //    sDeviceAddress = request.Ip,
            //    wPort = (ushort)request.Port,
            //    sUserName = request.Username,
            //    sPassword = request.Password
            //};

            var struLoginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
            {
                sDeviceAddress = "192.170.80.251",
                wPort = 8000,
                sUserName = "admin",
                sPassword = "Hikvision_2025"
            };

            HikvisionSdk.NET_DVR_DEVICEINFO_V40 struDeviceInfoV40 = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();

            // Attempt login
            int userId = HikvisionSdk.NET_DVR_Login_V40(ref struLoginInfo, ref struDeviceInfoV40);
            if (userId >= 0)
            {

                response.Success = true;
                response.Message = "Login Successful";
                response.UserId = userId;
                response.DeviceInfo = struDeviceInfoV40;

                return Ok(response);
            }
            else
            {
                int nErr = HikvisionSdk.NET_DVR_GetLastError();
                response.Success = false;
                response.ErrorCode = nErr;
                response.Message = HikvisionErrorHelper.GetErrorMessage(nErr);
                return BadRequest(response);
            }
        }



        [HttpGet("close-door")]
        public IActionResult CloseDoorHardcoded()
        {
            try
            {
                // Init SDK
                HikvisionSdk.NET_DVR_Init();

                // Login info
                var loginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
                {
                    sDeviceAddress = "192.170.80.251",
                    wPort = 8000,
                    sUserName = "admin",
                    sPassword = "Hikvision_2025",
                    bUseAsynLogin = 0
                };

                var deviceInfo = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();
                int userId = HikvisionSdk.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);

                if (userId < 0)
                {
                    int err = HikvisionSdk.NET_DVR_GetLastError();
                    return BadRequest(new { message = "Login failed", error = err });
                }

                // Close door (command = 0)
                bool success = HikvisionSdk.NET_DVR_ControlGateway(userId, 1, 0);

                if (!success)
                {
                    int err = HikvisionSdk.NET_DVR_GetLastError();
                    return BadRequest(new { message = "Failed to close door", error = err });
                }

                // Logout and cleanup
                HikvisionSdk.NET_DVR_Logout(userId);
                HikvisionSdk.NET_DVR_Cleanup();

                return Ok(new { message = "Door closed successfully (hardcoded)" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected error", exception = ex.Message });
            }
        }


        /// <summary>
        /// Hardcoded test endpoint to open the door.
        /// </summary>
        [HttpGet("open-door-v2")]
        public IActionResult OpenDoorHardcoded()
        {
            try
            {
                // Init SDK
                HikvisionSdk.NET_DVR_Init();

                // Login info
                var loginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
                {
                    sDeviceAddress = "192.170.80.251",
                    wPort = 8000,
                    sUserName = "admin",
                    sPassword = "Hikvision_2025",
                    bUseAsynLogin = 0
                };

                var deviceInfo = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();
                int userId = HikvisionSdk.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);

                if (userId < 0)
                {
                    int err = HikvisionSdk.NET_DVR_GetLastError();
                    return BadRequest(new { message = "Login failed", error = err });
                }

                // Open door (command = 1)
                bool success = HikvisionSdk.NET_DVR_ControlGateway(userId, 1, 1);

                if (!success)
                {
                    int err = HikvisionSdk.NET_DVR_GetLastError();
                    return BadRequest(new { message = "Failed to open door", error = err });
                }

                // Logout and cleanup
                HikvisionSdk.NET_DVR_Logout(userId);
                HikvisionSdk.NET_DVR_Cleanup();

                return Ok(new { message = "Door opened successfully (hardcoded)" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unexpected error", exception = ex.Message });
            }
        }



    }


}
