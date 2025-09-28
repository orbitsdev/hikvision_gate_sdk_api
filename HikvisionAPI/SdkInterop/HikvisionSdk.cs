using System;
using System.Runtime.InteropServices;

namespace HikvisionAPI.SdkInterop
{
    public static class HikvisionSdk
    {
        #if WINDOWS
                private const string SDK_DLL = "HCNetSDK.dll";
        #else
                private const string SDK_DLL = "libhcnetsdk.so";
        #endif

        // ---- SDK FUNCTIONS ----
        [DllImport(SDK_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool NET_DVR_Init();

        [DllImport(SDK_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NET_DVR_GetLastError();

        [DllImport(SDK_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern void NET_DVR_Cleanup();

        [DllImport(SDK_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NET_DVR_Login_V40(ref NET_DVR_USER_LOGIN_INFO pLoginInfo, ref NET_DVR_DEVICEINFO_V40 lpDeviceInfo);

        [DllImport(SDK_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool NET_DVR_Logout(int iUserID);

        [DllImport(SDK_DLL, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool NET_DVR_ControlGateway(int lUserID, int lGatewayIndex, uint dwSta);

        // ---- STRUCTS ----
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct NET_DVR_USER_LOGIN_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string sDeviceAddress;

            public ushort wPort;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sUserName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public string sPassword;

            public byte bUseAsynLogin;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] byRes2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct NET_DVR_DEVICEINFO_V40
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] byRes;
        }
    }
}
