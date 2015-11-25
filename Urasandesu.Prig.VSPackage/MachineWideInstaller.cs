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
            if (machinePreq == null)
                throw new ArgumentNullException("machinePreq");

            var profLocs = EnvironmentRepository.GetProfilerLocations();
            if (profLocs == null || profLocs.Length == 0)
                return false;

            foreach (var profLoc in profLocs)
            {
                using (var classesRootKey = EnvironmentRepository.OpenRegistryBaseKey(RegistryHive.ClassesRoot, profLoc.RegistryView))
                using (var inprocServer32Key = EnvironmentRepository.OpenRegistrySubKey(classesRootKey, ProfilerLocation.InprocServer32Path))
                {
                    machinePreq.OnProfilerStatusChecking(profLoc);
                    if (!IsProfilerInstalled(inprocServer32Key, ProfilerLocation.GetExpectedFileDescription(machinePreq.PackageVersion)))
                        return false;
                }
            }
            return true;
        }

        bool IsProfilerInstalled(RegistryKey inprocServer32Key, string expectedFileDesc)
        {
            if (inprocServer32Key == null)
                return false;

            var profPath = (string)EnvironmentRepository.GetRegistryValue(inprocServer32Key, null);
            if (!EnvironmentRepository.ExistsFile(profPath))
                return false;

            return EnvironmentRepository.GetFileDescription(profPath) == expectedFileDesc;
        }


        public void Install(MachineWideInstallation mwInstl)
        {
            if (mwInstl == null)
                throw new ArgumentNullException("mwInstl");

            mwInstl.OnPreparing();

            if (HasBeenInstalled(mwInstl.Prerequisite))
            {
                mwInstl.OnCompleted(MachineWideProcessResults.Skipped);
                return;
            }

            CreateNupkg(mwInstl);
            RegisterNuGetSource(mwInstl);
            RegisterEnvironmentVariables(mwInstl);
            RegisterProfiler(mwInstl);
            InstallDefaultSource(mwInstl);

            mwInstl.OnCompleted(MachineWideProcessResults.Completed);
        }

        void CreateNupkg(MachineWideInstallation mwInstl)
        {
            var toolsPath = EnvironmentRepository.GetToolsPath();
            var pkgName = "Prig";
            mwInstl.OnNuGetPackageCreating(pkgName);
            var nugetPackageFolder = Path.Combine(toolsPath, "NuGet");
            var stdout = NuGetExecutor.StartPacking(Path.Combine(nugetPackageFolder, "Prig.nuspec"), toolsPath);
            mwInstl.OnNuGetPackageCreated(stdout);
        }

        void RegisterNuGetSource(MachineWideInstallation mwInstl)
        {
            var toolsPath = EnvironmentRepository.GetToolsPath();
            var name = "Prig Source";
            mwInstl.OnNuGetSourceRegistering(name, toolsPath);
            var stdout = NuGetExecutor.StartSourcing(name, toolsPath);
            mwInstl.OnNuGetSourceRegistered(stdout);
        }

        void RegisterEnvironmentVariables(MachineWideInstallation mwInstl)
        {
            var pkgDir = EnvironmentRepository.GetPackageFolder();
            var name = EnvironmentRepository.GetPackageFolderKey();
            var value = pkgDir;
            mwInstl.OnEnvironmentVariableRegistering(name, value);
            EnvironmentRepository.SetPackageFolder(value);
            mwInstl.OnEnvironmentVariableRegistered(name, value);
        }

        void RegisterProfiler(MachineWideInstallation mwInstl)
        {
            var profLocs = EnvironmentRepository.GetProfilerLocations();
            if (profLocs == null || profLocs.Length == 0)
                return;

            foreach (var profLoc in EnvironmentRepository.GetProfilerLocations())
            {
                mwInstl.OnProfilerRegistering(profLoc);
                var stdout = Regsvr32Executor.StartInstalling(profLoc.PathOfInstalling);
                mwInstl.OnProfilerRegistered(stdout);
            }
        }

        void InstallDefaultSource(MachineWideInstallation mwInstl)
        {
            var pkgName = "TestWindow";
            var msvsdirPath = new DirectoryInfo(@"C:\Program Files (x86)").EnumerateDirectories("Microsoft Visual Studio *").
                                                                           Where(_ => Regex.IsMatch(_.Name, @"Microsoft Visual Studio \d+\.\d+")).
                                                                           OrderByDescending(_ => _.Name).
                                                                           Select(_ => _.FullName).
                                                                           First();
            var src = Path.Combine(msvsdirPath, @"Common7\IDE\CommonExtensions\Microsoft\TestWindow");
            mwInstl.OnDefaultSourceInstalling(pkgName, src);
            var stdout = PrigExecutor.StartInstalling(pkgName, src);
            mwInstl.OnDefaultSourceInstalled(stdout);
        }


        public void Uninstall(MachineWideUninstallation umwPkg)
        {
            if (umwPkg == null)
                throw new ArgumentNullException("umwPkg");

            umwPkg.OnPreparing();

            if (!HasBeenInstalled(umwPkg.Prerequisite))
            {
                umwPkg.OnCompleted(MachineWideProcessResults.Skipped);
                return;
            }

            UninstallDefaultSource(umwPkg);
            UnregisterProfiler(umwPkg);
            UnregisterEnvironmentVariables(umwPkg);
            UnregisterNuGetSource(umwPkg);

            umwPkg.OnCompleted(MachineWideProcessResults.Completed);
        }

        void UninstallDefaultSource(MachineWideUninstallation umwPkg)
        {
            var pkgName = "All";
            umwPkg.OnDefaultSourceUninstalling(pkgName);
            var stdout = PrigExecutor.StartUninstalling(pkgName);
            umwPkg.OnDefaultSourceUninstalled(stdout);
        }

        void UnregisterProfiler(MachineWideUninstallation umwPkg)
        {
            var profLocs = EnvironmentRepository.GetProfilerLocations();
            if (profLocs == null || profLocs.Length == 0)
                return;

            foreach (var profLoc in EnvironmentRepository.GetProfilerLocations())
            {
                umwPkg.OnProfilerUnregistering(profLoc);
                var stdout = Regsvr32Executor.StartUninstalling(profLoc.PathOfInstalling);
                umwPkg.OnProfilerUnregistered(stdout);
            }
        }

        void UnregisterEnvironmentVariables(MachineWideUninstallation umwPkg)
        {
            var name = EnvironmentRepository.GetPackageFolderKey();
            umwPkg.OnEnvironmentVariableUnregistering(name);
            EnvironmentRepository.RemovePackageFolder();
            umwPkg.OnEnvironmentVariableUnregistered(name);
        }

        void UnregisterNuGetSource(MachineWideUninstallation umwPkg)
        {
            var name = "Prig Source";
            umwPkg.OnNuGetSourceUnregistering(name);
            var stdout = NuGetExecutor.StartUnsourcing(name);
            umwPkg.OnNuGetSourceUnregistered(stdout);
        }
    }
}
