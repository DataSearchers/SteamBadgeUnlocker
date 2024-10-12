using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SteamBadgeUnlocker.API
{
    public abstract class NativeWrapper<TNativeFunctions> : INativeWrapper
    {
        protected IntPtr ObjectAddress;
        protected TNativeFunctions Functions;

        public override string ToString()
        {
            return $"Steam Interface<{typeof(TNativeFunctions)}> #{this.ObjectAddress.ToInt32():X8}";
        }

        public void SetupFunctions(IntPtr objectAddress)
        {
            this.ObjectAddress = objectAddress;

            var iface = (NativeClass)Marshal.PtrToStructure(
                this.ObjectAddress,
                typeof(NativeClass));

            this.Functions = (TNativeFunctions)Marshal.PtrToStructure(
                iface.VirtualTable,
                typeof(TNativeFunctions));
        }

        private readonly Dictionary<IntPtr, Delegate> _FunctionCache = new();

        protected Delegate GetDelegate<TDelegate>(IntPtr pointer)
        {
            if (this._FunctionCache.TryGetValue(pointer, out var function) == false)
            {
                function = Marshal.GetDelegateForFunctionPointer(pointer, typeof(TDelegate));
                this._FunctionCache[pointer] = function;
            }
            return function;
        }

        protected TDelegate GetFunction<TDelegate>(IntPtr pointer)
            where TDelegate : class
        {
            return (TDelegate)((object)this.GetDelegate<TDelegate>(pointer));
        }

        protected void Call<TDelegate>(IntPtr pointer, params object[] args)
        {
            this.GetDelegate<TDelegate>(pointer).DynamicInvoke(args);
        }

        protected TReturn Call<TReturn, TDelegate>(IntPtr pointer, params object[] args)
        {
            return (TReturn)this.GetDelegate<TDelegate>(pointer).DynamicInvoke(args);
        }
    }
}
