/* 
 * File: ULDllImport.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2015 Akira Sugiura
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



using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace UntestableLibrary
{
    public class ULDllImport
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [DllImport("Kernel32.dll")]
        public static extern int GetCurrentProcessId();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, CallingConvention = CallingConvention.Winapi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWow64Process([In] IntPtr processHandle, [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

        public string FormatCurrentThreadTimes()
        {
            var creationTime = default(long);
            var exitTime = default(long);
            var kernelTime = default(long);
            var userTime = default(long);
            if (!GetThreadTimes(GetCurrentThread(), out creationTime, out exitTime, out kernelTime, out userTime))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return string.Format("Creation Time: {0}, Exit Time: {1}, Kernel Time: {2}, User Time: {3}", creationTime, exitTime, kernelTime, userTime);
        }

        public string FormatCurrentProcessId()
        {
            var pid = GetCurrentProcessId();
            return string.Format("The current process ID is {0}", pid);
        }

        public void Start()
        {
            var msgBoxRet = MessageBox(new IntPtr(0), "Process Starts Now!", "Message Dialog", 0);
            
            // TODO: ...
            throw new NotImplementedException();
        }

        public string Is64BitProcessMessage()
        {
            var wow64Process = false;
            if (!IsWow64Process(GetCurrentProcess(), out wow64Process))
                throw new Win32Exception(Marshal.GetLastWin32Error());

            return wow64Process ? 
                        string.Format("This is a 32 bit process!") :
                        string.Format("This is a 32 bit process on 32 bit windows or a 64 bit process on 64 bit windows!");
        }
    }
}
