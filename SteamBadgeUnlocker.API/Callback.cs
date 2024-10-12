using System;
using System.Runtime.InteropServices;

namespace SteamBadgeUnlocker.API
{
    public abstract class Callback : ICallback
    {
        public delegate void CallbackFunction(IntPtr param);

        public event CallbackFunction OnRun;

        public abstract int Id { get; }
        public abstract bool IsServer { get; }

        public void Run(IntPtr param)
        {
            this.OnRun(param);
        }
    }

    public abstract class Callback<TParameter> : ICallback
        where TParameter : struct
    {
        public delegate void CallbackFunction(TParameter arg);

        public event CallbackFunction OnRun;

        public abstract int Id { get; }
        public abstract bool IsServer { get; }

        public void Run(IntPtr pvParam)
        {
            var data = (TParameter)Marshal.PtrToStructure(pvParam, typeof(TParameter));
            this.OnRun(data);
        }
    }
}
