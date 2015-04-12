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
using NuGet.VisualStudio;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text.RegularExpressions;
using Urasandesu.Prig.VSPackage.Infrastructure;
using VSLangProj110;

namespace Urasandesu.Prig.VSPackage
{
    class PrigPackageController
    {
        [Dependency]
        public MonitoringSelectionService MonitoringSelectionService { private get; set; }

        [Dependency]
        public IVsPackageInstallerServices InstallerServices { private get; set; }

        [Dependency]
        public IVsPackageInstaller Installer { private get; set; }

        [Dependency]
        public IVsPackageInstallerEvents InstallerEvents { private get; set; }

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

        protected virtual Runspace NewRunspace()
        {
            return RunspaceFactory.CreateRunspace();
        }

        void AddPrigAssemblyCore(PrigPackageViewModel viewModel, string additionalInclude)
        {
            viewModel.Statusbar.BeginProgress();

            viewModel.Statusbar.ReportProgress("Checking current project's packages...", 25u, 100u);
            var project = MonitoringSelectionService.GetCurrentProject();
            if (!InstallerServices.IsPackageInstalled(project, "Prig"))
                InstallPackage(viewModel, project, "Prig", 50u, 100u);

            viewModel.Statusbar.ReportProgress("Starting Prig setup session...", 75u, 100u);
            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -NoIntro -AdditionalInclude {0} -Project $Project
", additionalInclude);
            ExecuteCommand(command, project);

            viewModel.Statusbar.ReportProgress(string.Format("Completed adding Prig assembly for {0}.", additionalInclude), 100u, 100u);
            viewModel.ShowMessageBox(string.Format("Completed adding Prig assembly for {0}.", additionalInclude), OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
            viewModel.Statusbar.EndProgress();
            viewModel.Statusbar.Text.Value = string.Format("Completed adding Prig assembly for {0}.", additionalInclude);
        }

        void EditPrigIndirectionSettingsCore(PrigPackageViewModel viewModel, string editorialInclude)
        {
            viewModel.Statusbar.BeginProgress();

            viewModel.Statusbar.ReportProgress("Checking current project's packages...", 25u, 100u);
            var project = MonitoringSelectionService.GetCurrentProject();
            if (!InstallerServices.IsPackageInstalled(project, "Prig"))
                InstallPackage(viewModel, project, "Prig", 50u, 100u);

            viewModel.Statusbar.ReportProgress("Starting the Prig setup session...", 75u, 100u);
            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -EditorialInclude {0} -Project $Project
", editorialInclude);
            ExecuteCommand(command, project);

            viewModel.Statusbar.ReportProgress(string.Format("Completed editing Prig assembly for {0}.", editorialInclude), 100u, 100u);
            viewModel.Statusbar.EndProgress();
            viewModel.Statusbar.Text.Value = string.Format("Completed editing Prig assembly for {0}.", editorialInclude);
        }

        void RemovePrigAssemblyCore(PrigPackageViewModel viewModel, string deletionalInclude)
        {
            if (viewModel.ShowMessageBox(string.Format("Are you sure you want to remove Prig assembly {0}?", deletionalInclude), OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGICON.OLEMSGICON_QUERY) != VSConstants.MessageBoxResult.IDYES)
                return;

            viewModel.Statusbar.BeginProgress();

            viewModel.Statusbar.ReportProgress("Checking current project's packages...", 25u, 100u);
            var project = MonitoringSelectionService.GetCurrentProject();
            if (!InstallerServices.IsPackageInstalled(project, "Prig"))
                InstallPackage(viewModel, project, "Prig", 50u, 100u);

            viewModel.Statusbar.ReportProgress("Starting the Prig setup session...", 75u, 100u);
            var command = string.Format(
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Start-PrigSetup -DeletionalInclude {0} -Project $Project
", deletionalInclude);
            ExecuteCommand(command, project);

            viewModel.Statusbar.ReportProgress(string.Format("Completed removing Prig assembly {0}.", deletionalInclude), 100u, 100u);
            viewModel.ShowMessageBox(string.Format("Completed removing Prig assembly {0}.", deletionalInclude), OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
            viewModel.Statusbar.EndProgress();
            viewModel.Statusbar.Text.Value = string.Format("Completed removing Prig assembly {0}.", deletionalInclude);
        }

        void EnableTestAdapterCore(PrigPackageViewModel viewModel)
        {
            viewModel.Statusbar.BeginProgress();

            viewModel.Statusbar.ReportProgress("Enabling Prig test adapter...", 50u, 100u);
            var project = viewModel.CurrentProject.Value;
            if (project == null)
                throw new InvalidOperationException("Current project isn't selected.");
            var command =
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Enable-PrigTestAdapter -Project $Project
";
            ExecuteCommand(command, project);

            viewModel.Statusbar.ReportProgress(string.Format("Completed enabling Prig test adapter for {0}.", project.Name), 100u, 100u);
            viewModel.Statusbar.EndProgress();
            viewModel.Statusbar.Text.Value = string.Format("Completed enabling Prig test adapter for {0}.", project.Name);
        }

        void DisableTestAdapterCore(PrigPackageViewModel viewModel)
        {
            viewModel.Statusbar.BeginProgress();

            viewModel.Statusbar.ReportProgress("Disabling Prig test adapter...", 50u, 100u);
            var command =
@"
Import-Module ([IO.Path]::Combine($env:URASANDESU_PRIG_PACKAGE_FOLDER, 'tools\Urasandesu.Prig'))
Disable-PrigTestAdapter
";
            ExecuteCommand(command);

            viewModel.Statusbar.ReportProgress("Completed disabling Prig test adapter.", 100u, 100u);
            viewModel.Statusbar.EndProgress();
            viewModel.Statusbar.Text.Value = "Completed disabling Prig test adapter.";
        }

        void ExecuteCommand(string command, Project project = null)
        {
            using (var runspace = NewRunspace())
            {
                runspace.Open();
                if (project != null)
                    runspace.SessionStateProxy.SetVariable("Project", project);
                var results = default(Collection<PSObject>);
                command = "Set-ExecutionPolicy RemoteSigned -Scope Process -Force\r\n" + command;
                using (var pipeline = runspace.CreatePipeline(command, false))
                    results = pipeline.Invoke();
            }
        }

        void InstallPackage(PrigPackageViewModel viewModel, Project project, string packageId, uint progressValue, uint progressMaximum)
        {
            var onInstalling = new VsPackageEventHandler(metadata => viewModel.Statusbar.ReportProgress(string.Format("Installing '{0}'...", metadata.Id), progressValue, progressMaximum));
            var onInstalled = new VsPackageEventHandler(metadata => viewModel.Statusbar.ReportProgress(string.Format("Installed '{0}'.", metadata.Id), progressValue, progressMaximum));
            var onReferenceAdded = new VsPackageEventHandler(metadata => viewModel.Statusbar.ReportProgress(string.Format("Reference Added '{0}'.", metadata.Id), progressValue, progressMaximum));
            try
            {
                InstallerEvents.PackageInstalling += onInstalling;
                InstallerEvents.PackageInstalled += onInstalled;
                InstallerEvents.PackageReferenceAdded += onReferenceAdded;

                var source = default(string);
                var version = default(string);
                var ignoreDependencies = false;
                Installer.InstallPackage(source, project, packageId, version, ignoreDependencies);
            }
            finally
            {
                InstallerEvents.PackageInstalling -= onInstalling;
                InstallerEvents.PackageInstalled -= onInstalled;
                InstallerEvents.PackageReferenceAdded -= onReferenceAdded;
            }
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
