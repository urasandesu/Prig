/* 
 * File: InstanceGetters.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2014 Akira Sugiura
 *  
 *  This software is MIT License.
 *  
 *  Permission is hereby granted, free of charge, to any person obtaining a copy
 *  of this software and associated documentation files (the "Software"), to deal
 *  in the Software without restriction, including without limitation the rights
 *  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *  copies of the Software, and to permit persons to whom the Software is
 *  furnished to do so, subject to the following conditions:
 *  
 *  The above copyright notice and this permission notice shall be included in
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 *  THE SOFTWARE.
 */


using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Urasandesu.Prig.Framework
{
    public static class InstanceGetters
    {
        static InstanceGetters()
        {
            var weaverPath = GetWeaverPath();
            var weaverDir = Path.GetDirectoryName(weaverPath);
            SetDllDirectory(weaverDir);
        }

        static string GetWeaverPath()
        {
            var subKey = Registry.ClassesRoot.OpenSubKey(@"CLSID\{532C1F05-F8F3-4FBA-8724-699A31756ABD}\InprocServer32");
            return (string)subKey.GetValue("");
        }

        
        
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersTryAdd")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TryAdd([MarshalAs(UnmanagedType.LPWStr)] string key, IntPtr pFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersTryGet")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TryGet([MarshalAs(UnmanagedType.LPWStr)] string key, out IntPtr ppFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersTryRemove")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool TryRemove([MarshalAs(UnmanagedType.LPWStr)] string key, out IntPtr ppFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersGetOrAdd")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetOrAdd([MarshalAs(UnmanagedType.LPWStr)] string key, IntPtr pFuncPtr, out IntPtr ppFuncPtr);

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersClear")]
        internal static extern void Clear();

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersEnterDisabledProcessing")]
        static extern void EnterDisabledProcessing();

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersExitDisabledProcessing")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ExitDisabledProcessing();

        [DllImport("Urasandesu.Prig.dll", EntryPoint = "InstanceGettersIsDisabledProcessing")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsDisabledProcessing();

        public struct InstanceGettersProcessingDisabled : IDisposable
        {
            public void Dispose()
            {
                ExitDisabledProcessing();
            }
        }

        public static InstanceGettersProcessingDisabled DisableProcessing()
        {
            EnterDisabledProcessing();
            return new InstanceGettersProcessingDisabled();
        }

        
        
        static Func<IndirectionAssemblyRepository> ms_newIndirectionAssemblyRepository;
        internal static Func<IndirectionAssemblyRepository> NewIndirectionAssemblyRepository
        {
            get
            {
                if (ms_newIndirectionAssemblyRepository == null)
                    ms_newIndirectionAssemblyRepository = () => new IndirectionAssemblyRepository();
                return ms_newIndirectionAssemblyRepository;
            }
            set { ms_newIndirectionAssemblyRepository = value; }
        }

        static Func<AdditionalDelegatesAssemblyRepository> ms_newAdditionalDelegatesAssemblyRepository;
        internal static Func<AdditionalDelegatesAssemblyRepository> NewAdditionalDelegatesAssemblyRepository
        {
            get
            {
                if (ms_newAdditionalDelegatesAssemblyRepository == null)
                    ms_newAdditionalDelegatesAssemblyRepository = () => new AdditionalDelegatesAssemblyRepository();
                return ms_newAdditionalDelegatesAssemblyRepository;
            }
            set { ms_newAdditionalDelegatesAssemblyRepository = value; }
        }
    }
}
