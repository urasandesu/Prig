/* 
 * File: ULInstanceGetters.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
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
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace UntestableLibrary
{
    public static class ULInstanceGetters
    {
        static ULInstanceGetters()
        {
            var weaverPath = GetWeaverPath();
            var weaverDir = Path.GetDirectoryName(weaverPath);
            SetDllDirectory(weaverDir);
            WeaverDirectory = weaverDir;
        }

        static string GetWeaverPath()
        {
            var subKey = Registry.ClassesRoot.OpenSubKey(@"CLSID\{532C1F05-F8F3-4FBA-8724-699A31756ABD}\InprocServer32");
            return (string)subKey.GetValue("");
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetDllDirectory(string lpPathName);

        public static string WeaverDirectory { get; private set; }
    }

    
    
    public static class ULProcessMixin
    {
        const int ErrorCancelled = 1223;

        public static bool RestartCurrentProcess()
        {
            return RestartCurrentProcessWith(null);
        }

        public static bool RestartCurrentProcessWith(Action<ProcessStartInfo> additionalSetup)
        {
            var curProc = Process.GetCurrentProcess();
            var startInfo = curProc.StartInfo;
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = Environment.CurrentDirectory;
            startInfo.FileName = curProc.MainModule.FileName;
            var commandLineArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
            if (commandLineArgs.Any())
                startInfo.Arguments = "\"" + string.Join("\" \"", commandLineArgs.Select(_ => _.Replace("\"", "\\\"")).ToArray()) + "\"";
            if (additionalSetup != null)
                additionalSetup(startInfo);
            try
            {
                Process.Start(startInfo);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == ErrorCancelled)
                    return false;
                throw;
            }

            curProc.CloseMainWindow();

            return true;
        }
    }
}
