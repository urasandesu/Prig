/* 
 * File: PrigPackageController.cs
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Security.Principal;
using System.Text.RegularExpressions;
using Urasandesu.NAnonym.Mixins.System.Diagnostics;
using Urasandesu.NAnonym.Mixins.System.Security.Principal;
using Urasandesu.Prig.VSPackage.Infrastructure;
using VSLangProj110;

namespace Urasandesu.Prig.VSPackage
{
    class PrigPackageController
    {
        [Dependency]
        public MonitoringSelectionService MonitoringSelectionService { private get; set; }

        [Dependency]
        public IProjectWideInstaller ProjectWideInstaller { private get; set; }

        [Dependency]
        public IMachineWideInstaller MachineWideInstaller { private get; set; }

        [Dependency]
        public IManagementCommandExecutor ManagementCommandExecutor { private get; set; }

        public virtual void AddPrigAssemblyForMSCorLib(PrigPackageViewModel viewModel)
        {
            AddPrigAssemblyCore(viewModel, "mscorlib");
        }

        public virtual void AddPrigAssembly(PrigPackageViewModel viewModel)
        {
            AddPrigAssemblyCore(viewModel, MonitoringSelectionService.GetSelectedItem<Reference5>().Name);
        }

        public virtual void EnableTestAdapter(PrigPackageViewModel viewModel)
        {
            EnableTestAdapterCore(viewModel);
            viewModel.IsTestAdapterEnabled.Value = true;
        }

        public virtual void BeforeQueryStatusTestAdapter(PrigPackageViewModel viewModel)
        {
            if (viewModel.IsTestAdapterEnabled.Value)
                return;

            var project = MonitoringSelectionService.GetCurrentProject();
            viewModel.CurrentProject.Value = IsSupportedProject(project) ? project : null;
        }

        public virtual void DisableTestAdapter(PrigPackageViewModel viewModel)
        {
            DisableTestAdapterCore(viewModel);
            viewModel.IsTestAdapterEnabled.Value = false;
        }

        public virtual void EditPrigIndirectionSettings(PrigPackageViewModel viewModel)
        {
            var projectItem = MonitoringSelectionService.GetSelectedItem<ProjectItem>();
            var editorialInclude = Regex.Replace(projectItem.Name, @"(.*)\.v\d+\.\d+\.\d+\.v\d+\.\d+\.\d+\.\d+\.prig", "$1");
            EditPrigIndirectionSettingsCore(viewModel, editorialInclude);
        }

        public virtual void RemovePrigAssembly(PrigPackageViewModel viewModel)
        {
            var projectItem = MonitoringSelectionService.GetSelectedItem<ProjectItem>();
            var deletionalInclude = Regex.Replace(projectItem.Name, @"(.*)\.prig$", "$1");
            RemovePrigAssemblyCore(viewModel, deletionalInclude);
        }

        public virtual void BeforeQueryStatusEditPrigIndirectionSettings(PrigPackageViewModel viewModel)
        {
            var projectItem = MonitoringSelectionService.GetSelectedItem<ProjectItem>();
            var extension = Path.GetExtension(projectItem.Name);
            viewModel.IsEditPrigIndirectionSettingsCommandVisible.Value = string.Equals(extension, ".prig", StringComparison.InvariantCultureIgnoreCase);
        }

        public virtual void OnBuildDone(PrigPackageViewModel viewModel)
        {
            if (!viewModel.IsTestAdapterEnabled.Value ||
                viewModel.CurrentProject.Value == null)
                DisableTestAdapter(viewModel);
            else
                EnableTestAdapterCore(viewModel);
        }

        public virtual void OnProjectRemoved(PrigPackageViewModel viewModel, Project project)
        {
            if (!viewModel.IsTestAdapterEnabled.Value ||
                viewModel.CurrentProject.Value == null ||
                viewModel.CurrentProject.Value.Name != project.Name)
                return;

            DisableTestAdapter(viewModel);
        }

        public virtual void PrepareRegisteringPrig(PrigPackageViewModel viewModel)
        {
            viewModel.Statusbar.BeginProgress();

            var machinePreq = new MachinePrerequisite(Resources.NuGetRootPackageVersion);
            machinePreq.ProfilerRegistrationStatusChecking += 
                profLoc => viewModel.Statusbar.ReportProgress(string.Format("Checking the installation status for the profiler '{0}'...", profLoc.PathOfInstalling), 50u, 100u);

            if (MachineWideInstaller.HasBeenInstalled(machinePreq))
            {
                viewModel.Statusbar.ReportProgress("Skipped Prig registration processes because Prig has been already registered.", 100u, 100u);
                viewModel.ShowMessageBox("Skipped Prig registration processes because Prig has been already registered.", OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
                viewModel.Statusbar.EndProgress();
                viewModel.Statusbar.Text.Value = "Skipped Prig registration processes because Prig has been already registered.";
                return;
            }


            if (!WindowsIdentity.GetCurrent().IsElevated())
            {
                // TODO: 確認メッセージ。
                ProcessMixin.RestartCurrentProcessWith(_ =>
                {
                    _.Verb = "runas";
                });
            }
            else
            {
                RegisterPrig(viewModel);
            }
        }

        public virtual void PrepareUnregisteringPrig(PrigPackageViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        void RegisterPrig(PrigPackageViewModel viewModel)
        {
            var mwPkg = new MachineWidePackage(Resources.NuGetRootPackageVersion);
            mwPkg.RegistrationPreparing += 
                () => viewModel.Statusbar.BeginProgress();
            mwPkg.ProfilerRegistrationStatusChecking += 
                profLoc => viewModel.Statusbar.ReportProgress(string.Format("Checking the installation status for the profiler '{0}'...", profLoc.PathOfInstalling), 25u, 100u);
            mwPkg.NuGetPackageCreating += 
                packageName => viewModel.Statusbar.ReportProgress(string.Format("Creating the nuget package '{0}'...", packageName), 25u, 100u);
            mwPkg.NuGetPackageCreated += 
                stdout => viewModel.Statusbar.ReportProgress(stdout, 25u, 100u);
            mwPkg.NuGetSourceRegistering += 
                (path, name) => viewModel.Statusbar.ReportProgress(string.Format("Registering the nuget source '{0}' as '{1}'...", path, name), 25u, 100u);
            mwPkg.NuGetSourceRegistered += 
                stdout => viewModel.Statusbar.ReportProgress(stdout, 25u, 100u);
            mwPkg.EnvironmentVariableRegistering += 
                (variableValue, variableName) => viewModel.Statusbar.ReportProgress(string.Format("Registering the environment variable '{0}' as '{1}'...", variableValue, variableName), 25u, 100u);
            mwPkg.EnvironmentVariableRegistered += 
                (variableValue, variableName) => viewModel.Statusbar.ReportProgress("Registered the environment variable.", 25u, 100u);
            mwPkg.ProfilerRegistering += 
                profLoc => viewModel.Statusbar.ReportProgress(string.Format("Registering the profiler '{0}' to Registry...", profLoc.PathOfInstalling), 25u, 100u);
            mwPkg.ProfilerRegistered += 
                stdout => viewModel.Statusbar.ReportProgress(stdout, 25u, 100u);
            mwPkg.DefaultSourceInstalling += 
                (source, packageName) => viewModel.Statusbar.ReportProgress(string.Format("Installing the default source '{0}' as the package '{1}'...", source, packageName), 25u, 100u);
            mwPkg.DefaultSourceInstalled += 
                stdout => viewModel.Statusbar.ReportProgress(stdout, 25u, 100u);
            mwPkg.RegistrationCompleted += 
                result =>
                {
                    viewModel.Statusbar.EndProgress();
                    switch (result)
                    {
                        case RegistrationResults.Skipped:
                            viewModel.Statusbar.Text.Value = "Skipped Prig installation processes.";
                            break;
                        case RegistrationResults.Completed:
                            viewModel.Statusbar.Text.Value = "Completed Prig installation processes.";
                            break;
                    }
                };

            MachineWideInstaller.Install(mwPkg);
        }

        public virtual void UnregisterPrig(PrigPackageViewModel viewModel)
        {
            throw new NotImplementedException();
        }

        void AddPrigAssemblyCore(PrigPackageViewModel viewModel, string additionalInclude)
        {
            var project = MonitoringSelectionService.GetCurrentProject();


            var pwPkg = new ProjectWidePackage(Resources.NuGetRootPackageId, Resources.NuGetRootPackageVersion, project);
            pwPkg.PackagePreparing += () => 
            { 
                viewModel.Statusbar.BeginProgress(); 
                viewModel.Statusbar.ReportProgress("Checking current project's packages...", 25u, 100u); 
            };
            pwPkg.PackageInstalling += metadata => viewModel.Statusbar.ReportProgress(string.Format("Installing '{0}'...", metadata.Id), 50u, 100u);
            pwPkg.PackageInstalled += metadata => viewModel.Statusbar.ReportProgress(string.Format("Installed '{0}'.", metadata.Id), 50u, 100u);
            pwPkg.PackageReferenceAdded += metadata => viewModel.Statusbar.ReportProgress(string.Format("Reference Added '{0}'.", metadata.Id), 50u, 100u);
            ProjectWideInstaller.Install(pwPkg);


            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -NoIntro -AdditionalInclude {0} -Project $Project
", additionalInclude);
            var mci = new ManagementCommandInfo(command, project);
            mci.CommandExecuting += () => viewModel.Statusbar.ReportProgress("Starting the Prig setup session...", 75u, 100u);
            mci.CommandExecuted += () =>
            {
                viewModel.Statusbar.ReportProgress(string.Format("Completed adding Prig assembly for {0}.", additionalInclude), 100u, 100u);
                viewModel.ShowMessageBox(string.Format("Completed adding Prig assembly for {0}.", additionalInclude), OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
                viewModel.Statusbar.EndProgress();
                viewModel.Statusbar.Text.Value = string.Format("Completed adding Prig assembly for {0}.", additionalInclude);
            };
            ManagementCommandExecutor.Execute(mci);
        }

        void EditPrigIndirectionSettingsCore(PrigPackageViewModel viewModel, string editorialInclude)
        {
            var project = MonitoringSelectionService.GetCurrentProject();


            var pwPkg = new ProjectWidePackage(Resources.NuGetRootPackageId, Resources.NuGetRootPackageVersion, project);
            pwPkg.PackagePreparing += () =>
            {
                viewModel.Statusbar.BeginProgress();
                viewModel.Statusbar.ReportProgress("Checking current project's packages...", 25u, 100u);
            };
            pwPkg.PackageInstalling += metadata => viewModel.Statusbar.ReportProgress(string.Format("Installing '{0}'...", metadata.Id), 50u, 100u);
            pwPkg.PackageInstalled += metadata => viewModel.Statusbar.ReportProgress(string.Format("Installed '{0}'.", metadata.Id), 50u, 100u);
            pwPkg.PackageReferenceAdded += metadata => viewModel.Statusbar.ReportProgress(string.Format("Reference Added '{0}'.", metadata.Id), 50u, 100u);
            ProjectWideInstaller.Install(pwPkg);


            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -EditorialInclude {0} -Project $Project
", editorialInclude);
            var mci = new ManagementCommandInfo(command, project);
            mci.CommandExecuting += () => viewModel.Statusbar.ReportProgress("Starting the Prig setup session...", 75u, 100u);
            mci.CommandExecuted += () =>
            {
                viewModel.Statusbar.ReportProgress(string.Format("Completed editing Prig assembly for {0}.", editorialInclude), 100u, 100u);
                viewModel.Statusbar.EndProgress();
                viewModel.Statusbar.Text.Value = string.Format("Completed editing Prig assembly for {0}.", editorialInclude);
            };
            ManagementCommandExecutor.Execute(mci);
        }

        void RemovePrigAssemblyCore(PrigPackageViewModel viewModel, string deletionalInclude)
        {
            if (viewModel.ShowMessageBox(string.Format("Are you sure you want to remove Prig assembly {0}?", deletionalInclude), OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGICON.OLEMSGICON_QUERY) != VSConstants.MessageBoxResult.IDYES)
                return;

            var project = MonitoringSelectionService.GetCurrentProject();


            var pwPkg = new ProjectWidePackage(Resources.NuGetRootPackageId, Resources.NuGetRootPackageVersion, project);
            pwPkg.PackagePreparing += () =>
            {
                viewModel.Statusbar.BeginProgress();
                viewModel.Statusbar.ReportProgress("Checking current project's packages...", 25u, 100u);
            };
            pwPkg.PackageInstalling += metadata => viewModel.Statusbar.ReportProgress(string.Format("Installing '{0}'...", metadata.Id), 50u, 100u);
            pwPkg.PackageInstalled += metadata => viewModel.Statusbar.ReportProgress(string.Format("Installed '{0}'.", metadata.Id), 50u, 100u);
            pwPkg.PackageReferenceAdded += metadata => viewModel.Statusbar.ReportProgress(string.Format("Reference Added '{0}'.", metadata.Id), 50u, 100u);
            ProjectWideInstaller.Install(pwPkg);


            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -DeletionalInclude {0} -Project $Project
", deletionalInclude);
            var mci = new ManagementCommandInfo(command, project);
            mci.CommandExecuting += () => viewModel.Statusbar.ReportProgress("Starting the Prig setup session...", 75u, 100u);
            mci.CommandExecuted += () =>
            {
                viewModel.Statusbar.ReportProgress(string.Format("Completed removing Prig assembly {0}.", deletionalInclude), 100u, 100u);
                viewModel.ShowMessageBox(string.Format("Completed removing Prig assembly {0}.", deletionalInclude), OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
                viewModel.Statusbar.EndProgress();
                viewModel.Statusbar.Text.Value = string.Format("Completed removing Prig assembly {0}.", deletionalInclude);
            };
            ManagementCommandExecutor.Execute(mci);
        }

        void EnableTestAdapterCore(PrigPackageViewModel viewModel)
        {
            var project = viewModel.CurrentProject.Value;
            if (project == null)
                throw new InvalidOperationException("Current project isn't selected.");


            var command =
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Enable-PrigTestAdapter -Project $Project
"; 
            var mci = new ManagementCommandInfo(command, project);
            mci.CommandExecuting += () =>
            {
                viewModel.Statusbar.BeginProgress();
                viewModel.Statusbar.ReportProgress("Enabling Prig test adapter...", 50u, 100u);
            };
            mci.CommandExecuted += () =>
            {
                viewModel.Statusbar.ReportProgress(string.Format("Completed enabling Prig test adapter for {0}.", project.Name), 100u, 100u);
                viewModel.Statusbar.EndProgress();
                viewModel.Statusbar.Text.Value = string.Format("Completed enabling Prig test adapter for {0}.", project.Name);
            };
            ManagementCommandExecutor.Execute(mci);
        }

        void DisableTestAdapterCore(PrigPackageViewModel viewModel)
        {
            var command =
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Disable-PrigTestAdapter
"; 
            var mci = new ManagementCommandInfo(command);
            mci.CommandExecuting += () =>
            {
                viewModel.Statusbar.BeginProgress();
                viewModel.Statusbar.ReportProgress("Disabling Prig test adapter...", 50u, 100u);
            };
            mci.CommandExecuted += () =>
            {
                viewModel.Statusbar.ReportProgress("Completed disabling Prig test adapter.", 100u, 100u);
                viewModel.Statusbar.EndProgress();
                viewModel.Statusbar.Text.Value = "Completed disabling Prig test adapter.";
            };
            ManagementCommandExecutor.Execute(mci);
        }

        static bool IsSupportedProject(Project project)
        {
            return project != null &&
                   (project.Kind == VSConstantsAlternative.UICONTEXT.CSharpProject_string ||
                    project.Kind == VSConstantsAlternative.UICONTEXT.FSharpProject_string ||
                    project.Kind == VSConstantsAlternative.UICONTEXT.VBProject_string);
        }
    }
}
