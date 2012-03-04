using System;
using System.Runtime.InteropServices;

namespace Urasandesu.Prig.Framework
{
    public static class InstanceGetters
    {
        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersTryAdd")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TryAdd([MarshalAs(UnmanagedType.LPWStr)] string key, IntPtr pFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersTryGet")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TryGet([MarshalAs(UnmanagedType.LPWStr)] string key, out IntPtr ppFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersTryRemove")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TryRemove([MarshalAs(UnmanagedType.LPWStr)] string key, out IntPtr ppFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersClear")]
        public static extern void Clear();
    }
}
