/* 
 * File: PrigController.cs
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



using EnvDTE;
using Microsoft.Practices.Unity;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Urasandesu.NAnonym.Mixins.System.Diagnostics;
using Urasandesu.NAnonym.Mixins.System.Security.Principal;
using Urasandesu.Prig.VSPackage.Infrastructure;
using Urasandesu.Prig.VSPackage.Models;
using VSLangProj110;

namespace Urasandesu.Prig.VSPackage
{
    class PrigController
    {
        [Dependency]
        public MonitoringSelectionService MonitoringSelectionService { private get; set; }

        [Dependency]
        public IProjectWideInstaller ProjectWideInstaller { private get; set; }

        [Dependency]
        public IMachineWideInstaller MachineWideInstaller { private get; set; }

        [Dependency]
        public IManagementCommandExecutor ManagementCommandExecutor { private get; set; }

        public virtual void AddPrigAssemblyForMSCorLib(PrigViewModel vm)
        {
            AddPrigAssemblyCore(vm, "mscorlib");
        }

        public virtual void AddPrigAssembly(PrigViewModel vm)
        {
            AddPrigAssemblyCore(vm, MonitoringSelectionService.GetSelectedItem<Reference5>().Identity);
        }

        public virtual void EnableTestAdapter(PrigViewModel vm)
        {
            if (EnableTestAdapterCore(vm))
                vm.IsTestAdapterEnabled.Value = true;
        }

        public virtual void BeforeQueryStatusTestAdapter(PrigViewModel vm)
        {
            if (vm.IsTestAdapterEnabled.Value)
                return;

            var proj = MonitoringSelectionService.GetCurrentProject();
            vm.SetToCurrentProjectIfSupported(proj);
        }

        public virtual void DisableTestAdapter(PrigViewModel vm)
        {
            if (DisableTestAdapterCore(vm))
                vm.IsTestAdapterEnabled.Value = false;
        }

        public virtual void EditPrigIndirectionSettings(PrigViewModel vm)
        {
            var projItem = MonitoringSelectionService.GetSelectedItem<ProjectItem>();
            var editorialInclude = Regex.Replace(projItem.Name, @"(.*)\.v\d+\.\d+\.\d+\.v\d+\.\d+\.\d+\.\d+\.prig", "$1");
            EditPrigIndirectionSettingsCore(vm, editorialInclude);
        }

        public virtual void RemovePrigAssembly(PrigViewModel vm)
        {
            var projItem = MonitoringSelectionService.GetSelectedItem<ProjectItem>();
            var deletionalInclude = Regex.Replace(projItem.Name, @"(.*)\.prig$", "$1");
            RemovePrigAssemblyCore(vm, deletionalInclude);
        }

        public virtual void BeforeQueryStatusEditPrigIndirectionSettings(PrigViewModel vm)
        {
            var projItem = MonitoringSelectionService.GetSelectedItem<ProjectItem>();
            vm.SetEditPrigIndirectionSettingsCommandVisibility(projItem);
        }

        public virtual void OnBuildDone(PrigViewModel vm)
        {
            if (!vm.HasEnabledTestAdapter())
                DisableTestAdapter(vm);
            else
                EnableTestAdapterCore(vm);
        }

        public virtual void OnProjectRemoved(PrigViewModel vm, Project proj)
        {
            if (!vm.HasEnabledTestAdapter(proj))
                return;

            DisableTestAdapter(vm);
        }

        public virtual void PrepareRegisteringPrig(PrigViewModel vm)
        {
            vm.BeginMachineWideProcessProgress(MachineWideProcesses.Installing);

            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(50u, profLoc);
            if (MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                return;
            }


            if (!WindowsIdentity.GetCurrent().IsElevated())
            {
                vm.ShowVisualStudioHasNotBeenElevatedYetMessage();
                if (ProcessMixin.RestartCurrentProcessWith(_ => { _.Verb = "runas"; }))
                    return;

                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.CanceledByUser);
            }
            else
            {
                RegisterPrig(vm);
            }
        }

        public virtual void PrepareUnregisteringPrig(PrigViewModel vm)
        {
            vm.BeginMachineWideProcessProgress(MachineWideProcesses.Uninstalling);

            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(50u, profLoc);

            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                return;
            }


            if (!WindowsIdentity.GetCurrent().IsElevated())
            {
                vm.ShowVisualStudioHasNotBeenElevatedYetMessage();
                if (ProcessMixin.RestartCurrentProcessWith(_ => { _.Verb = "runas"; }))
                    return;

                vm.EndSkippedMachineWideProcessProgress(SkippedReasons.CanceledByUser);
            }
            else
            {
                UnregisterPrig(vm);
            }
        }

        void RegisterPrig(PrigViewModel vm)
        {
            var mwInstl = new MachineWideInstallation(Resources.NuGetRootPackageVersion);
            mwInstl.Preparing += () => vm.BeginMachineWideProcessProgress(MachineWideProcesses.Installing);
            mwInstl.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(8u, profLoc);
            mwInstl.NuGetPackageCreating += pkgName => vm.ReportNuGetPackageCreatingProgress(17u, pkgName);
            mwInstl.NuGetPackageCreated += stdout => vm.ReportNuGetPackageCreatedProgress(25u, stdout);
            mwInstl.NuGetSourceRegistering += (path, name) => vm.ReportNuGetSourceProcessingProgress(33u, path, name);
            mwInstl.NuGetSourceRegistered += stdout => vm.ReportNuGetSourceProcessedProgress(42u, stdout);
            mwInstl.EnvironmentVariableRegistering += (name, value) => vm.ReportEnvironmentVariableProcessingProgress(50u, name, value);
            mwInstl.EnvironmentVariableRegistered += (name, value) => vm.ReportEnvironmentVariableProcessedProgress(58u);
            mwInstl.ProfilerRegistering += profLoc => vm.ReportProfilerProcessingProgress(67u, profLoc);
            mwInstl.ProfilerRegistered += stdout => vm.ReportProfilerProcessedProgress(75u, stdout);
            mwInstl.PrigSourceInstalling += (pkgName, src) => vm.ReportPrigSourceProcessingProgress(83u, pkgName, src);
            mwInstl.PrigSourceInstalled += stdout => vm.ReportPrigSourceProcessedProgress(92u, stdout);
            mwInstl.Completed += result => OnCompletedRegisterPrig(vm, result);

            MachineWideInstaller.Install(mwInstl);
        }

        protected virtual void OnCompletedRegisterPrig(PrigViewModel vm, MachineWideProcessResults result)
        {
            switch (result)
            {
                case MachineWideProcessResults.Skipped:
                    vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                    vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                    break;
                case MachineWideProcessResults.Completed:
                    var restarts = vm.ConfirmRestartingVisualStudioToTakeEffect();
                    vm.EndCompletedMachineWideProcessProgress();
                    if (!restarts)
                        return;

                    ProcessMixin.RestartCurrentProcess();
                    break;
            }
        }

        public virtual void UnregisterPrig(PrigViewModel vm)
        {
            var umwPkg = new MachineWideUninstallation(Resources.NuGetRootPackageVersion);
            umwPkg.Preparing += () => vm.BeginMachineWideProcessProgress(MachineWideProcesses.Uninstalling);
            umwPkg.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(10u, profLoc);
            umwPkg.PrigSourceUninstalling += pkgName => vm.ReportPrigSourceProcessingProgress(20u, pkgName, null);
            umwPkg.PrigSourceUninstalled += stdout => vm.ReportPrigSourceProcessedProgress(30u, stdout);
            umwPkg.ProfilerUnregistering += profLoc => vm.ReportProfilerProcessingProgress(40u, profLoc);
            umwPkg.ProfilerUnregistered += stdout => vm.ReportProfilerProcessedProgress(50u, stdout);
            umwPkg.EnvironmentVariableUnregistering += name => vm.ReportEnvironmentVariableProcessingProgress(60u, name, null);
            umwPkg.EnvironmentVariableUnregistered += name => vm.ReportEnvironmentVariableProcessedProgress(70u);
            umwPkg.NuGetSourceUnregistering += name => vm.ReportNuGetSourceProcessingProgress(80u, name, null);
            umwPkg.NuGetSourceUnregistered += stdout => vm.ReportNuGetSourceProcessedProgress(90u, stdout);
            umwPkg.Completed += result => OnCompletedUnregisterPrig(vm, result);

            MachineWideInstaller.Uninstall(umwPkg);
        }

        protected virtual void OnCompletedUnregisterPrig(PrigViewModel vm, MachineWideProcessResults result)
        {
            switch (result)
            {
                case MachineWideProcessResults.Skipped:
                    vm.ShowSkippedMachineWideProcessMessage(SkippedReasons.AlreadyRegistered);
                    vm.EndSkippedMachineWideProcessProgress(SkippedReasons.AlreadyRegistered);
                    break;
                case MachineWideProcessResults.Completed:
                    var restarts = vm.ConfirmRestartingVisualStudioToTakeEffect();
                    vm.EndCompletedMachineWideProcessProgress();
                    if (!restarts)
                        return;

                    ProcessMixin.RestartCurrentProcess();
                    break;
            }
        }

        void AddPrigAssemblyCore(PrigViewModel vm, string additionalInclude)
        {
            vm.BeginProjectWideProcessProgress(ProjectWideProcesses.PrigAssemblyAdding);


            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(13u, profLoc);
            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedProjectWideProcessMessage(SkippedReasons.NotRegisteredYet, additionalInclude);
                vm.EndSkippedProjectWideProcessProgress(SkippedReasons.NotRegisteredYet, additionalInclude);
                return;
            }


            var proj = MonitoringSelectionService.GetCurrentProject();


            var pwPkg = new ProjectWidePackage(Resources.NuGetRootPackageId, Resources.NuGetRootPackageVersion, proj);
            pwPkg.PackagePreparing += () => vm.ReportPackagePreparingProgress(25u);
            pwPkg.PackageInstalling += metadata => vm.ReportPackageInstallingProgress(50u, metadata);
            pwPkg.PackageInstalled += metadata => vm.ReportPackageInstalledProgress(50u, metadata);
            pwPkg.PackageReferenceAdded += metadata => vm.ReportPackageReferenceAddedProgress(50u, metadata);
            ProjectWideInstaller.Install(pwPkg);


            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -NoIntro -AdditionalInclude {0} -Project $Project
", additionalInclude);
            var mci = new ManagementCommandInfo(command, proj);
            mci.CommandExecuting += () => vm.ReportProcessingProjectWideProcessProgress(75u, additionalInclude);
            mci.CommandExecuted +=
                () =>
                {
                    vm.ShowCompletedProjectWideProcessMessage(additionalInclude);
                    vm.EndCompletedProjectWideProcessProgress(additionalInclude);
                };
            ManagementCommandExecutor.Execute(mci);
        }

        void EditPrigIndirectionSettingsCore(PrigViewModel vm, string editorialInclude)
        {
            vm.BeginProjectWideProcessProgress(ProjectWideProcesses.PrigIndirectionSettingsEditing);


            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(13u, profLoc);
            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedProjectWideProcessMessage(SkippedReasons.NotRegisteredYet, editorialInclude);
                vm.EndSkippedProjectWideProcessProgress(SkippedReasons.NotRegisteredYet, editorialInclude);
                return;
            }


            var proj = MonitoringSelectionService.GetCurrentProject();


            var pwPkg = new ProjectWidePackage(Resources.NuGetRootPackageId, Resources.NuGetRootPackageVersion, proj);
            pwPkg.PackagePreparing += () => vm.ReportPackagePreparingProgress(25u);
            pwPkg.PackageInstalling += metadata => vm.ReportPackageInstallingProgress(50u, metadata);
            pwPkg.PackageInstalled += metadata => vm.ReportPackageInstalledProgress(50u, metadata);
            pwPkg.PackageReferenceAdded += metadata => vm.ReportPackageReferenceAddedProgress(50u, metadata);
            ProjectWideInstaller.Install(pwPkg);


            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -EditorialInclude {0} -Project $Project
", editorialInclude);
            var mci = new ManagementCommandInfo(command, proj);
            mci.CommandExecuting += () => vm.ReportProcessingProjectWideProcessProgress(75u, editorialInclude);
            mci.CommandExecuted += () => vm.EndCompletedProjectWideProcessProgress(editorialInclude);
            ManagementCommandExecutor.Execute(mci);
        }

        void RemovePrigAssemblyCore(PrigViewModel vm, string deletionalInclude)
        {
            vm.BeginProjectWideProcessProgress(ProjectWideProcesses.PrigAssemblyRemoving);


            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(13u, profLoc);
            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedProjectWideProcessMessage(SkippedReasons.NotRegisteredYet, deletionalInclude);
                vm.EndSkippedProjectWideProcessProgress(SkippedReasons.NotRegisteredYet, deletionalInclude);
                return;
            }


            if (!vm.ConfirmRemovingPrigAssembly(deletionalInclude))
                return;


            var proj = MonitoringSelectionService.GetCurrentProject();


            var pwPkg = new ProjectWidePackage(Resources.NuGetRootPackageId, Resources.NuGetRootPackageVersion, proj);
            pwPkg.PackagePreparing += () => vm.ReportPackagePreparingProgress(25u);
            pwPkg.PackageInstalling += metadata => vm.ReportPackageInstallingProgress(50u, metadata);
            pwPkg.PackageInstalled += metadata => vm.ReportPackageInstalledProgress(50u, metadata);
            pwPkg.PackageReferenceAdded += metadata => vm.ReportPackageReferenceAddedProgress(50u, metadata);
            ProjectWideInstaller.Install(pwPkg);


            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -DeletionalInclude {0} -Project $Project
", deletionalInclude);
            var mci = new ManagementCommandInfo(command, proj);
            mci.CommandExecuting += () => vm.ReportProcessingProjectWideProcessProgress(75u, deletionalInclude);
            mci.CommandExecuted +=
                () =>
                {
                    vm.ShowCompletedProjectWideProcessMessage(deletionalInclude);
                    vm.EndCompletedProjectWideProcessProgress(deletionalInclude);
                };
            ManagementCommandExecutor.Execute(mci);
        }

        bool EnableTestAdapterCore(PrigViewModel vm)
        {
            vm.BeginProjectWideProcessProgress(ProjectWideProcesses.TestAdapterEnabling);


            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(25u, profLoc);
            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedProjectWideProcessMessage(SkippedReasons.NotRegisteredYet, null);
                vm.EndSkippedProjectWideProcessProgress(SkippedReasons.NotRegisteredYet, null);
                return false;
            }


            var projs = vm.GetTargetProjects(vm.GetCurrentProjectOrException().DTE);


            var command =
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Enable-PrigTestAdapter -Project $Project
";
            var mci = new ManagementCommandInfo(command, projs);
            mci.CommandExecuting += () => vm.ReportProcessingProjectWideProcessProgress(50u, projs);
            mci.CommandExecuted += () => vm.EndCompletedProjectWideProcessProgress(projs);
            ManagementCommandExecutor.Execute(mci);

            return true;
        }

        bool DisableTestAdapterCore(PrigViewModel vm)
        {
            vm.BeginProjectWideProcessProgress(ProjectWideProcesses.TestAdapterDisabling);


            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerStatusChecking += profLoc => vm.ReportProfilerStatusCheckingProgress(25u, profLoc);
            if (!MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                vm.ShowSkippedProjectWideProcessMessage(SkippedReasons.NotRegisteredYet, null);
                vm.EndSkippedProjectWideProcessProgress(SkippedReasons.NotRegisteredYet, null);
                return false;
            }


            var command =
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Disable-PrigTestAdapter
";
            var mci = new ManagementCommandInfo(command);
            mci.CommandExecuting += () => vm.ReportProcessingProjectWideProcessProgress(50u, default(string));
            mci.CommandExecuted += () => vm.EndCompletedProjectWideProcessProgress(default(string));
            ManagementCommandExecutor.Execute(mci);

            return true;
        }
    }
}
