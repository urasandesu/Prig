/* 
 * File: ConsoleController.cs
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
using System.Security.Principal;
using Urasandesu.NAnonym.Mixins.System.Security.Principal;
using Urasandesu.Prig.VSPackage.Models;

namespace Urasandesu.Prig.VSPackage.Shell
{
    class ConsoleController
    {
        [Dependency]
        public IMachineWideInstaller MachineWideInstaller { private get; set; }

        public virtual void PrepareRegisteringPrig(ConsoleViewModel vm)
        {
            vm.BeginMachineWideProcessProgress(MachineWideProcesses.Installing);

            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(profLoc);
            if (MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                return;
            }


            if (!WindowsIdentity.GetCurrent().IsElevated())
            {
                vm.ShowCurrentConsoleHasNotBeenElevatedYetMessage();
                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.Error);
            }
            else
            {
                RegisterPrig(vm);
            }
        }

        void RegisterPrig(ConsoleViewModel vm)
        {
            var mwInstl = new MachineWideInstallation(Resources.NuGetRootPackageVersion);
            mwInstl.IsPrigSourceInstallationDisabled = true;
            mwInstl.Preparing += () => vm.BeginMachineWideProcessProgress(MachineWideProcesses.Installing);
            mwInstl.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(profLoc);
            mwInstl.NuGetPackageCreating += pkgName => vm.ReportNuGetPackageCreatingProgress(pkgName);
            mwInstl.NuGetPackageCreated += stdout => vm.ReportNuGetPackageCreatedProgress(stdout);
            mwInstl.NuGetSourceRegistering += (path, name) => vm.ReportNuGetSourceProcessingProgress(path, name);
            mwInstl.NuGetSourceRegistered += stdout => vm.ReportNuGetSourceProcessedProgress(stdout);
            mwInstl.EnvironmentVariableRegistering += (name, value) => vm.ReportEnvironmentVariableProcessingProgress(name, value);
            mwInstl.EnvironmentVariableRegistered += (name, value) => vm.ReportEnvironmentVariableProcessedProgress();
            mwInstl.ProfilerRegistering += profLoc => vm.ReportProfilerProcessingProgress(profLoc);
            mwInstl.ProfilerRegistered += stdout => vm.ReportProfilerProcessedProgress(stdout);
            mwInstl.Completed += result => OnCompletedRegisterPrig(vm, result);

            MachineWideInstaller.Install(mwInstl);
        }

        protected virtual void OnCompletedRegisterPrig(ConsoleViewModel vm, MachineWideProcessResults result)
        {
            switch (result)
            {
                case MachineWideProcessResults.Skipped:
                    vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                    vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                    break;
                case MachineWideProcessResults.Completed:
                    vm.ShowCompletedMachineWideProcessMessage();
                    vm.EndCompletedMachineWideProcessProgress();
                    break;
            }
        }

        public virtual void PrepareUnregisteringPrig(ConsoleViewModel vm)
        {
            vm.BeginMachineWideProcessProgress(MachineWideProcesses.Uninstalling);

            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(profLoc);

            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                return;
            }


            if (!WindowsIdentity.GetCurrent().IsElevated())
            {
                vm.ShowCurrentConsoleHasNotBeenElevatedYetMessage();
                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.Error);
            }
            else
            {
                UnregisterPrig(vm);
            }
        }

        void UnregisterPrig(ConsoleViewModel vm)
        {
            var umwPkg = new MachineWideUninstallation(Resources.NuGetRootPackageVersion);
            umwPkg.Preparing += () => vm.BeginMachineWideProcessProgress(MachineWideProcesses.Uninstalling);
            umwPkg.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(profLoc);
            umwPkg.ProfilerUnregistering += profLoc => vm.ReportProfilerProcessingProgress(profLoc);
            umwPkg.ProfilerUnregistered += stdout => vm.ReportProfilerProcessedProgress(stdout);
            umwPkg.EnvironmentVariableUnregistering += name => vm.ReportEnvironmentVariableProcessingProgress(name, null);
            umwPkg.EnvironmentVariableUnregistered += name => vm.ReportEnvironmentVariableProcessedProgress();
            umwPkg.NuGetSourceUnregistering += name => vm.ReportNuGetSourceProcessingProgress(name, null);
            umwPkg.NuGetSourceUnregistered += stdout => vm.ReportNuGetSourceProcessedProgress(stdout);
            umwPkg.Completed += result => OnCompletedUnregisterPrig(vm, result);

            MachineWideInstaller.Uninstall(umwPkg);
        }

        protected virtual void OnCompletedUnregisterPrig(ConsoleViewModel vm, MachineWideProcessResults result)
        {
            switch (result)
            {
                case MachineWideProcessResults.Skipped:
                    vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                    vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                    break;
                case MachineWideProcessResults.Completed:
                    vm.ShowCompletedMachineWideProcessMessage();
                    vm.EndCompletedMachineWideProcessProgress();
                    break;
            }
        }
    }
}
