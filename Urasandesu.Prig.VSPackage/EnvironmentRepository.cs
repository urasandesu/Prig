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
            var programDataPath = Environment.GetEnvironmentVariable("ALLUSERSPROFILE");
            return Path.Combine(programDataPath, @"chocolatey\lib\Prig");
        }

        public string GetPackageFolderKey()
        {
            return "URASANDESU_PRIG_PACKAGE_FOLDER";
        }

        public void StorePackageFolder(string variableValue)
        {
            Environment.SetEnvironmentVariable(GetPackageFolderKey(), variableValue);
            Environment.SetEnvironmentVariable(GetPackageFolderKey(), variableValue, EnvironmentVariableTarget.User);
        }
        
        public void RemovePackageFolder()
        {
            Environment.SetEnvironmentVariable(GetPackageFolderKey(), null, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(GetPackageFolderKey(), null);
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

        public RegistryKey OpenRegistryBaseKey(RegistryHive hKey, RegistryView view)
        {
            return RegistryKey.OpenBaseKey(hKey, view);
        }

        public RegistryKey OpenRegistrySubKey(RegistryKey key, string name)
        {
            return key.OpenSubKey(name);
        }

        public object GetRegistryValue(RegistryKey key, string name)
        {
            return key.GetValue(name);
        }

        public bool ExistsFile(string path)
        {
            return File.Exists(path);
        }

        public string GetFileDescription(string path)
        {
            return FileVersionInfo.GetVersionInfo(path).FileDescription;
        }
    }
}
