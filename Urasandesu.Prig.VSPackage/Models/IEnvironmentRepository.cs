/* 
 * File: IEnvironmentRepository.cs
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



using Microsoft.Win32;
using System;

namespace Urasandesu.Prig.VSPackage.Models
{
    interface IEnvironmentRepository
    {
        string GetVsixPackageFolder();
        string GetVsixToolsPath();
        string GetVsixLibPath();
        void RegisterPackageFolder();
        void UnregisterPackageFolder();
        void RegisterToolsPath();
        void UnregisterToolsPath();
        string GetPackageFolder();
        string GetPackageFolderKey();
        void StorePackageFolder(string variableValue);
        void RemovePackageFolder();
        string GetLogFolder();
        string GetLogFolderKey();
        void StoreLogFolder(string variableValue);
        void RemoveLogFolder();
        string GetEnvironmentVariable(string variable);
        string GetEnvironmentVariable(string variable, EnvironmentVariableTarget target);
        void SetEnvironmentVariable(string variable, string value);
        void SetEnvironmentVariable(string variable, string value, EnvironmentVariableTarget target);
        string GetToolsPath();
        string GetLibPath();
        ProfilerLocation[] GetProfilerLocations();
        string GetNuGetPath();
        string GetRegsvr32Path();
        string GetPrigPath();
        RegistryKey OpenRegistryBaseKey(RegistryHive hKey, RegistryView view);
        RegistryKey OpenRegistrySubKey(RegistryKey key, string name);
        RegistryKey OpenRegistrySubKey(RegistryKey key, string name, bool writable);
        object GetRegistryValue(RegistryKey key, string name);
        bool Is64BitOperatingSystem();
        bool ExistsFile(string path);
        bool ExistsDirectory(string path);
        string GetFileDescription(string path);
    }
}
