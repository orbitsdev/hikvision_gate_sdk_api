using System;
using System.IO;
using System.Runtime.InteropServices;
using HikvisionAPI.Models;
using HikvisionAPI.SdkInterop;

namespace HikvisionAPI.Services
{
    public class HikvisionService
    {
        /// <summary>
        /// Ensure native library path is set depending on OS
        /// </summary>
        private void ConfigureLibraryPath()
        {
            string sdkPath = Path.Combine(AppContext.BaseDirectory, "SDK", "HCNetSDK");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string currentPath = Environment.GetEnvironmentVariable("PATH") ?? "";
                Environment.SetEnvironmentVariable("PATH", sdkPath + ";" + currentPath);
            }
            else
            {
                string currentLdPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? "";
                Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", sdkPath + ":" + currentLdPath);
            }
        }

        public bool OpenDoor(DoorControlRequest request, out string error)
        {
            error = "";
            ConfigureLibraryPath();

            // Step 1: Init SDK
            if (!HikvisionSdk.NET_DVR_Init())
            {
                error = "NET_DVR_Init failed";
                return false;
            }

            // Step 2: Prepare login info
            var loginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
            {
                sDeviceAddress = request.Ip,
                wPort = (ushort)request.Port,
                sUserName = request.Username,
                sPassword = request.Password,
                bUseAsynLogin = 0,
                byRes2 = new byte[128]
            };

            var deviceInfo = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();

            // Step 3: Login
            int userId = HikvisionSdk.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
            if (userId < 0)
            {
                error = $"Login failed. Error Code: {HikvisionSdk.NET_DVR_GetLastError()}";
                HikvisionSdk.NET_DVR_Cleanup();
                return false;
            }

            // Step 4: Send door control command
            bool result = HikvisionSdk.NET_DVR_ControlGateway(userId, request.GatewayIndex, request.Command);
            if (!result)
            {
                error = $"NET_DVR_ControlGateway failed. Error Code: {HikvisionSdk.NET_DVR_GetLastError()}";
            }

            // Step 5: Logout and Cleanup
            HikvisionSdk.NET_DVR_Logout(userId);
            HikvisionSdk.NET_DVR_Cleanup();

            return result;
        }

        //public object? GetDeviceInfo(DeviceInfoRequest request, out string error)
        //{
        //    error = "";
        //    ConfigureLibraryPath();

        //    if (!HikvisionSdk.NET_DVR_Init())
        //    {
        //        error = "SDK Init failed";
        //        return null;
        //    }

        //    var loginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
        //    {
        //        sDeviceAddress = request.Ip,
        //        wPort = (ushort)request.Port,
        //        sUserName = request.Username,
        //        sPassword = request.Password,
        //        bUseAsynLogin = 0,
        //        byRes2 = new byte[128]
        //    };

        //    var deviceInfo = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();

        //    int userId = HikvisionSdk.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
        //    if (userId < 0)
        //    {
        //        error = $"Login failed. Error Code: {HikvisionSdk.NET_DVR_GetLastError()}";
        //        HikvisionSdk.NET_DVR_Cleanup();
        //        return null;
        //    }

        //    HikvisionSdk.NET_DVR_Logout(userId);
        //    HikvisionSdk.NET_DVR_Cleanup();

        //    return deviceInfo;
        //}
        public object? GetDeviceInfo(DeviceInfoRequest request, out string error)
        {
            error = "";
            ConfigureLibraryPath();

            if (!HikvisionSdk.NET_DVR_Init())
            {
                error = "SDK Init failed";
                return null;
            }

            // Prepare login info
            var loginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
            {
                sDeviceAddress = request.Ip,
                wPort = (ushort)request.Port,
                sUserName = request.Username,
                sPassword = request.Password,
                bUseAsynLogin = 0,
                byRes2 = new byte[128]
            };

            // Prepare device info
            var deviceInfo = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();
            // Initialize embedded struct fields if needed
            deviceInfo.byRes = new byte[48]; // adjust length according to your struct definition

            // Call login
            int userId = HikvisionSdk.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
            if (userId < 0)
            {
                int lastErr = HikvisionSdk.NET_DVR_GetLastError();

                if (lastErr == 7) // NET_DVR_PASSWORD_ERROR
                {
                    error = "Username or password error!";
                }
                else if (lastErr == 153) // NET_DVR_USER_LOCKED
                {
                    error = "User is locked on the device!";
                }
                else
                {
                    error = $"Login failed. Error Code: {lastErr}";
                }

                HikvisionSdk.NET_DVR_Cleanup();
                return null;
            }

            // Clean up
            HikvisionSdk.NET_DVR_Logout(userId);
            HikvisionSdk.NET_DVR_Cleanup();

            return deviceInfo;
        }
    }
}
