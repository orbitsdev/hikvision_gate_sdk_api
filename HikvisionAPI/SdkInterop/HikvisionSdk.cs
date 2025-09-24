using System;
using System.Runtime.InteropServices;

namespace HikvisionAPI.SdkInterop
{
    public static class HikvisionSdk
    {
        [DllImport("HCNetSDK.dll")]
        public static extern bool NET_DVR_Init();

        [DllImport("HCNetSDK.dll")]
        public static extern bool NET_DVR_Cleanup();

        [DllImport("HCNetSDK.dll")]
        public static extern int NET_DVR_GetLastError();

        [DllImport("HCNetSDK.dll", CharSet = CharSet.Ansi)]
        public static extern int NET_DVR_Login_V40(
            ref NET_DVR_USER_LOGIN_INFO loginInfo,
            ref NET_DVR_DEVICEINFO_V40 deviceInfo);

        [DllImport("HCNetSDK.dll")]
        public static extern bool NET_DVR_ControlGateway(int userID, int gatewayIndex, int command);

        [DllImport("HCNetSDK.dll")]
        public static extern bool NET_DVR_Logout(int userID);

        #region Structures

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_USER_LOGIN_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 129)]
            public string sDeviceAddress;

            public ushort wPort;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sUserName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sPassword;

            public byte bUseAsynLogin;
            public byte byProxyType;
            public byte byUseUTCTime;
            public byte byLoginMode;

            public int dwRes1;
            public IntPtr pLoginResult;
            public int dwRes2;
            public int dwRes3;
            public int dwRes4;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V40
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 236)]
            public byte[] byRes;
        }

        #endregion
    }
}
