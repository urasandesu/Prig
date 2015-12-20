/* 
 * File: EnvironmentRepository.cs
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



using Microsoft.VisualBasic.Devices;
using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Urasandesu.Prig.VSPackage
{
    class EnvironmentRepository : IEnvironmentRepository
    {
        public string GetVsixPackageFolder()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        public string GetVsixToolsPath()
        {
            var pkgDir = GetVsixPackageFolder();
            return Path.Combine(pkgDir, "tools");
        }

        public string GetVsixLibPath()
        {
            var pkgDir = GetVsixPackageFolder();
            return Path.Combine(pkgDir, "lib");
        }

        readonly Computer Computer = new Computer();

        public void RegisterPackageFolder()
        {
            if (Computer.FileSystem.DirectoryExists(GetToolsPath()))
                Computer.FileSystem.DeleteDirectory(GetToolsPath(), DeleteDirectoryOption.DeleteAllContents);
            if (Computer.FileSystem.DirectoryExists(GetLibPath()))
                Computer.FileSystem.DeleteDirectory(GetLibPath(), DeleteDirectoryOption.DeleteAllContents);
            Computer.FileSystem.CopyDirectory(GetVsixToolsPath(), GetToolsPath());
            Computer.FileSystem.CopyDirectory(GetVsixLibPath(), GetLibPath());
        }
        
        public void UnregisterPackageFolder()
        {
            if (Computer.FileSystem.DirectoryExists(GetToolsPath()))
                Computer.FileSystem.DeleteDirectory(GetToolsPath(), DeleteDirectoryOption.DeleteAllContents);
            if (Computer.FileSystem.DirectoryExists(GetLibPath()))
                Computer.FileSystem.DeleteDirectory(GetLibPath(), DeleteDirectoryOption.DeleteAllContents);
        }

        public string GetPackageFolder()
        {
            var programDataPath = GetEnvironmentVariable("ALLUSERSPROFILE", EnvironmentVariableTarget.Process);
            return Path.Combine(programDataPath, @"chocolatey\lib\Prig");
        }

        public string GetPackageFolderKey()
        {
            return "URASANDESU_PRIG_PACKAGE_FOLDER";
        }

        public void StorePackageFolder(string variableValue)
        {
            SetEnvironmentVariable(GetPackageFolderKey(), variableValue);
            SetEnvironmentVariable(GetPackageFolderKey(), variableValue, EnvironmentVariableTarget.Machine);
        }
        
        public void RemovePackageFolder()
        {
            SetEnvironmentVariable(GetPackageFolderKey(), null, EnvironmentVariableTarget.Machine);
            SetEnvironmentVariable(GetPackageFolderKey(), null);
        }

        public void RegisterToolsPath()
        {
            RegisterToolsPath(
                () => GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine),
                value => SetEnvironmentVariable("Path", value, EnvironmentVariableTarget.Machine)
            );
            RegisterToolsPath(
                () => GetEnvironmentVariable("Path"),
                value => SetEnvironmentVariable("Path", value)
            );
        }

        void RegisterToolsPath(Func<string> pathGetter, Action<string> pathSetter)
        {
            var path = pathGetter();
            var pattern = @";?" + Regex.Escape(GetToolsPath());
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(path) && regex.IsMatch(path))
                return;

            pathSetter(path + (string.IsNullOrEmpty(path) ? "" : ";") + GetToolsPath());
        }

        public void UnregisterToolsPath()
        {
            UnregisterToolsPath(
                () => GetEnvironmentVariable("Path"),
                value => SetEnvironmentVariable("Path", value)
            );
            UnregisterToolsPath(
                () => GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine),
                value => SetEnvironmentVariable("Path", value, EnvironmentVariableTarget.Machine)
            );
        }

        void UnregisterToolsPath(Func<string> pathGetter, Action<string> pathSetter)
        {
            var path = pathGetter();
            var pattern = @";?" + Regex.Escape(GetToolsPath());
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            if (string.IsNullOrEmpty(path) || !regex.IsMatch(path))
                return;

            pathSetter(regex.Replace(path, ""));
        }

        internal Func<string, EnvironmentVariableTarget, string> EnvironmentGetEnvironmentVariable = Environment.GetEnvironmentVariable;
        internal Action<string, string, EnvironmentVariableTarget> EnvironmentSetEnvironmentVariable = Environment.SetEnvironmentVariable;

        public string GetEnvironmentVariable(string variable)
        {
            return GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
        }

        public string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target)
        {
            return EnvironmentGetEnvironmentVariable(variable, target);
        }

        public void SetEnvironmentVariable(string variable, string value)
        {
            SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Process);
        }

        const int WM_SETTINGCHANGE = 0x001A;

        [Flags]
        enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8,
            SMTO_ERRORONEXIT = 0x20
        }

        [DllImport("user32.dll", BestFitMapping = false, SetLastError = true)]
        static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            int Msg,
            IntPtr wParam,
            string lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            IntPtr lpdwResult);

        public void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target)
        {
            if (target == EnvironmentVariableTarget.Process)
            {
                EnvironmentSetEnvironmentVariable(variable, value, target);
                return;
            }

            switch (target)
            {
                case EnvironmentVariableTarget.Machine:
                    SetMachineEnvironmentVariable(variable, value);
                    break;
                case EnvironmentVariableTarget.User:
                    SetUserEnvironmentVariable(variable, value);
                    break;
                default:
                    throw new NotSupportedException(target.ToString());
            }

            var explorers = Process.GetProcessesByName("explorer");
            foreach (var explorer in explorers)
            {
                SendMessageTimeout(explorer.MainWindowHandle, WM_SETTINGCHANGE, IntPtr.Zero, "Environment", SendMessageTimeoutFlags.SMTO_NORMAL, 1, IntPtr.Zero);
            }
            SendMessageTimeout((IntPtr)0xFFFF, WM_SETTINGCHANGE, IntPtr.Zero, "Environment", SendMessageTimeoutFlags.SMTO_NORMAL, 1, IntPtr.Zero);
        }

        void SetMachineEnvironmentVariable(string variable, string value)
        {
            using (var environment = OpenRegistrySubKey(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", true))
            {
                if (environment == null)
                    return;

                if (value == null)
                    RegistryKeyDeleteValue(environment, variable, false);
                else
                    RegistryKeySetValue(environment, variable, value);
            }
        }

        void SetUserEnvironmentVariable(string variable, string value)
        {
            using (var environment = OpenRegistrySubKey(Registry.CurrentUser, @"Environment", true))
            {
                if (environment == null)
                    return;

                if (value == null)
                    RegistryKeyDeleteValue(environment, variable, false);
                else
                    RegistryKeySetValue(environment, variable, value);
            }
        }

        public string GetToolsPath()
        {
            var pkgDir = GetPackageFolder();
            return Path.Combine(pkgDir, "tools");
        }

        public string GetLibPath()
        {
            var pkgDir = GetPackageFolder();
            return Path.Combine(pkgDir, "lib");
        }

        ProfilerLocation[] m_profLocs;
        public ProfilerLocation[] GetProfilerLocations()
        {
            if (m_profLocs == null)
            {
                var toolsPath = GetToolsPath();
                m_profLocs =
                    new[] 
                    { 
                        new ProfilerLocation(RegistryView.Registry64, Path.Combine(toolsPath, @"x64\Urasandesu.Prig.dll")), 
                        new ProfilerLocation(RegistryView.Registry32, Path.Combine(toolsPath, @"x86\Urasandesu.Prig.dll")) 
                    };
            }
            return m_profLocs;
        }

        public string GetNuGetPath()
        {
            var toolsPath = GetToolsPath();
            return Path.Combine(toolsPath, "NuGet.exe");
        }

        public string GetRegsvr32Path()
        {
            return Path.Combine(Environment.ExpandEnvironmentVariables("%windir%"), @"SysNative\regsvr32.exe");
        }

        public string GetPrigPath()
        {
            var toolsPath = GetToolsPath();
            return Path.Combine(toolsPath, "prig.exe");
        }

        internal Func<RegistryHive, RegistryView, RegistryKey> RegistryKeyOpenBaseKey = RegistryKey.OpenBaseKey;
        internal Func<RegistryKey, string, RegistryKey> RegistryKeyOpenSubKeyString = (key, name) => key.OpenSubKey(name);
        internal Func<RegistryKey, string, bool, RegistryKey> RegistryKeyOpenSubKeyStringBoolean = (key, name, writable) => key.OpenSubKey(name, writable);
        internal Action<RegistryKey, string, bool> RegistryKeyDeleteValue = (key, name, throwOnMissingValue) => key.DeleteValue(name, throwOnMissingValue);
        internal Action<RegistryKey, string, object> RegistryKeySetValue = (key, name, value) => key.SetValue(name, value);

        public RegistryKey OpenRegistryBaseKey(RegistryHive hKey, RegistryView view)
        {
            return RegistryKeyOpenBaseKey(hKey, view);
        }

        public RegistryKey OpenRegistrySubKey(RegistryKey key, string name)
        {
            return RegistryKeyOpenSubKeyString(key, name);
        }

        public RegistryKey OpenRegistrySubKey(RegistryKey key, string name, bool writable)
        {
            return RegistryKeyOpenSubKeyStringBoolean(key, name, writable);
        }

        public object GetRegistryValue(RegistryKey key, string name)
        {
            return key.GetValue(name);
        }

        public bool ExistsFile(string path)
        {
            return File.Exists(path);
        }

        public bool ExistsDirectory(string path)
        {
            return Directory.Exists(path);
        }

        public string GetFileDescription(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileDescription;
        }
    }
}
