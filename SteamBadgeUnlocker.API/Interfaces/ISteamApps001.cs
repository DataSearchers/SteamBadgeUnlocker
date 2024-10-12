using System;
using System.Runtime.InteropServices;

namespace SteamBadgeUnlocker.API.Interfaces
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ISteamApps001
    {
        public IntPtr GetAppData;
    }
}
