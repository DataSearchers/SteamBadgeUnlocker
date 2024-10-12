﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace SteamBadgeUnlocker.API
{
    internal class NativeStrings
    {
        public sealed class StringHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            internal StringHandle(IntPtr preexistingHandle, bool ownsHandle)
                : base(ownsHandle)
            {
                this.SetHandle(preexistingHandle);
            }

            public IntPtr Handle
            {
                get { return this.handle; }
            }

            protected override bool ReleaseHandle()
            {
                if (handle != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(handle);
                    handle = IntPtr.Zero;
                    return true;
                }

                return false;
            }
        }

        public static unsafe StringHandle StringToStringHandle(string value)
        {
            if (value == null)
            {
                return new StringHandle(IntPtr.Zero, true);
            }

            var bytes = Encoding.UTF8.GetBytes(value);
            var length = bytes.Length;

            var p = Marshal.AllocHGlobal(length + 1);
            Marshal.Copy(bytes, 0, p, bytes.Length);
            ((byte*)p)[length] = 0;
            return new StringHandle(p, true);
        }

        public static unsafe string PointerToString(sbyte* bytes)
        {
            if (bytes == null)
            {
                return null;
            }

            int running = 0;

            var b = bytes;
            if (*b == 0)
            {
                return string.Empty;
            }

            while ((*b++) != 0)
            {
                running++;
            }

            return new string(bytes, 0, running, Encoding.UTF8);
        }

        public static unsafe string PointerToString(byte* bytes)
        {
            return PointerToString((sbyte*)bytes);
        }

        public static unsafe string PointerToString(IntPtr nativeData)
        {
            return PointerToString((sbyte*)nativeData.ToPointer());
        }

        public static unsafe string PointerToString(sbyte* bytes, int length)
        {
            if (bytes == null)
            {
                return null;
            }

            int running = 0;

            var b = bytes;
            if (length == 0 || *b == 0)
            {
                return string.Empty;
            }

            while ((*b++) != 0 &&
                   running < length)
            {
                running++;
            }

            return new string(bytes, 0, running, Encoding.UTF8);
        }

        public static unsafe string PointerToString(byte* bytes, int length)
        {
            return PointerToString((sbyte*)bytes, length);
        }

        public static unsafe string PointerToString(IntPtr nativeData, int length)
        {
            return PointerToString((sbyte*)nativeData.ToPointer(), length);
        }
    }
}
