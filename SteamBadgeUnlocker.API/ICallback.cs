using System;

namespace SteamBadgeUnlocker.API
{
    public interface ICallback
    {
        int Id { get; }
        bool IsServer { get; }
        void Run(IntPtr param);
    }
}
