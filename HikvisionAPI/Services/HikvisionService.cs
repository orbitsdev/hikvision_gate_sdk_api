using HikvisionAPI.Models;
using HikvisionAPI.SdkInterop;


namespace HikvisionAPI.Services
{
    public class HikvisionService
    {

        public bool OpenDoor(DoorControlRequest request, out string error)
        {
            error = "";
            string dllPath = Path.Combine(AppContext.BaseDirectory, "sdk", "HCNetSDK");
            Environment.SetEnvironmentVariable("PATH", dllPath + ";" + Environment.GetEnvironmentVariable("PATH"));


            // Step 1: Init SDKw
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
                bUseAsynLogin = 0 // Sync login
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

        public object? GetDeviceInfo(DeviceInfoRequest request, out string error)
        {
            error = "";

            string dllPath = Path.Combine(AppContext.BaseDirectory, "sdk", "HCNetSDK");
            Environment.SetEnvironmentVariable("PATH", dllPath + ";" + Environment.GetEnvironmentVariable("PATH"));

            if (!HikvisionSdk.NET_DVR_Init())
            {
                error = "SDK Init failed";
                return null;
            }

            var loginInfo = new HikvisionSdk.NET_DVR_USER_LOGIN_INFO
            {
                sDeviceAddress = request.Ip,
                wPort = (ushort)request.Port,
                sUserName = request.Username,
                sPassword = request.Password,
                bUseAsynLogin = 0
            };

            var deviceInfo = new HikvisionSdk.NET_DVR_DEVICEINFO_V40();

            int userId = HikvisionSdk.NET_DVR_Login_V40(ref loginInfo, ref deviceInfo);
            if (userId < 0)
            {
                error = $"Login failed. Error Code: {HikvisionSdk.NET_DVR_GetLastError()}";
                HikvisionSdk.NET_DVR_Cleanup();
                return null;
            }

            HikvisionSdk.NET_DVR_Logout(userId);
            HikvisionSdk.NET_DVR_Cleanup();

            // Just return the entire struDeviceV30 struct
            return deviceInfo;
        }


    }



}
