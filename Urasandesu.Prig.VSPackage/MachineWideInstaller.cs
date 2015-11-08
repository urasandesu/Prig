/* 
 * File: MachineWideInstaller.cs
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



using Microsoft.Practices.Unity;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Urasandesu.Prig.VSPackage
{
    class MachineWideInstaller : IMachineWideInstaller
    {
        [Dependency]
        public IEnvironmentRepository EnvironmentRepository { private get; set; }

        [Dependency]
        public INuGetExecutor NuGetExecutor { private get; set; }

        [Dependency]
        public IRegsvr32Executor Regsvr32Executor { private get; set; }

        [Dependency]
        public IPrigExecutor PrigExecutor { private get; set; }

        
        public bool HasBeenInstalled(MachinePrerequisite machinePreq)
        {
            var isProfInstalled = true;
            foreach (var profLoc in EnvironmentRepository.GetProfilerLocations())
            {
                using (var classesRootKey = EnvironmentRepository.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLoc.RegistryView))
                using (var inprocServer32Key = EnvironmentRepository.OpenRegistrySubKey(classesRootKey, ProfilerLocation.InprocServer32Path))
                {
                    machinePreq.OnProfilerInstallationStatusChecking(profLoc);
                    isProfInstalled &= IsProfilerInstalled(inprocServer32Key, ProfilerLocation.GetExpectedFileDescription(machinePreq.PackageVersion));
                }
            }
            return isProfInstalled;
        }

        bool IsProfilerInstalled(RegistryKey inprocServer32Key, string expectedFileDesc)
        {
            if (inprocServer32Key == null)
                return false;

            var profPath = (string)EnvironmentRepository.GetRegistryValue(inprocServer32Key, null);
            if (!File.Exists(profPath))
                return false;

            return FileVersionInfo.GetVersionInfo(profPath).FileDescription == expectedFileDesc;
        }


        public void Install(MachineWidePackage mwPkg)
        {
            if (mwPkg == null)
                throw new ArgumentNullException("mwPkg");

            mwPkg.OnRegistrationPreparing();

            if (HasBeenInstalled(mwPkg.Prerequisite))
            {
                mwPkg.OnRegistrationCompleted(RegistrationResults.Skipped);
                return;
            }

            CreateNuspec(mwPkg);
            RegisterNuGetSource(mwPkg);
            RegisterEnvironmentVariables(mwPkg);
            RegisterProfiler(mwPkg);
            InstallDefaultSource(mwPkg);
        }

        void CreateNuspec(MachineWidePackage mwPkg)
        {
            var toolsPath = EnvironmentRepository.GetToolsPath();
            var packageName = "Prig";
            mwPkg.OnNuGetPackageCreating(packageName);
            var nugetPackageFolder = Path.Combine(toolsPath, "NuGet");
            var stdout = NuGetExecutor.StartPacking(Path.Combine(nugetPackageFolder, "Prig.nuspec"), toolsPath);
            mwPkg.OnNuGetPackageCreated(stdout);
        }

        void RegisterNuGetSource(MachineWidePackage mwPkg)
        {
            var toolsPath = EnvironmentRepository.GetToolsPath();
            var name = "Prig Source";
            mwPkg.OnNuGetSourceRegistering(toolsPath, name);
            var stdout = NuGetExecutor.StartSourcing(name, toolsPath);
            mwPkg.OnNuGetSourceRegistered(stdout);
        }

        void RegisterEnvironmentVariables(MachineWidePackage mwPkg)
        {
            var pkgDir = EnvironmentRepository.GetPackageFolder();
            var variableName = EnvironmentRepository.GetPackageFolderKey();
            var variableValue = pkgDir;
            mwPkg.OnEnvironmentVariableRegistering(variableValue, variableName);
            EnvironmentRepository.SetPackageFolder(variableValue);
            mwPkg.OnEnvironmentVariableRegistered(variableValue, variableName);
        }

        void RegisterProfiler(MachineWidePackage mwPkg)
        {
            foreach (var profLoc in EnvironmentRepository.GetProfilerLocations())
            {
                using (var classesRootKey = EnvironmentRepository.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLoc.RegistryView))
                using (var inprocServer32Key = EnvironmentRepository.OpenRegistrySubKey(classesRootKey, ProfilerLocation.InprocServer32Path))
                {
                    mwPkg.OnProfilerRegistering(profLoc);
                    var stdout = Regsvr32Executor.StartInstalling(profLoc.PathOfInstalling);
                    mwPkg.OnProfilerRegistered(stdout);
                }
            }
        }

        void InstallDefaultSource(MachineWidePackage mwPkg)
        {
            var packageName = "TestWindow";
            var msvsdirPath = new DirectoryInfo(@"C:\Program Files (x86)").EnumerateDirectories("Microsoft Visual Studio *").
                                                                           Where(_ => Regex.IsMatch(_.Name, @"Microsoft Visual Studio \d+\.\d+")).
                                                                           OrderByDescending(_ => _.Name).
                                                                           Select(_ => _.FullName).
                                                                           First();
            var source = Path.Combine(msvsdirPath, @"Common7\IDE\CommonExtensions\Microsoft\TestWindow");
            mwPkg.OnDefaultSourceInstalling(source, packageName);
            var stdout = PrigExecutor.StartInstalling(packageName, source);
            mwPkg.OnDefaultSourceInstalled(stdout);
        }
    }
}
