using System;

namespace SteamBadgeUnlocker.API
{
    public interface INativeWrapper
    {
        void SetupFunctions(IntPtr objectAddress);
    }
}
