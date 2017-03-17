/* 
 * File: PrigViewModel.cs
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Urasandesu.Prig.VSPackage.Infrastructure;
using Urasandesu.Prig.VSPackage.Models;

namespace Urasandesu.Prig.VSPackage
{
    class PrigViewModel : PackageViewModel
    {
        ApplicationProperty<bool> m_isTestAdapterEnabled;
        public ApplicationProperty<bool> IsTestAdapterEnabled
        {
            get
            {
                if (m_isTestAdapterEnabled == null)
                    m_isTestAdapterEnabled = new ApplicationProperty<bool>();
                return m_isTestAdapterEnabled;
            }
        }

        ApplicationProperty<Project> m_currentProject;
        public ApplicationProperty<Project> CurrentProject
        {
            get
            {
                if (m_currentProject == null)
                    m_currentProject = new ApplicationProperty<Project>();
                return m_currentProject;
            }
        }

        ApplicationProperty<bool> m_isEditPrigIndirectionSettingsCommandVisible;
        public ApplicationProperty<bool> IsEditPrigIndirectionSettingsCommandVisible
        {
            get
            {
                if (m_isEditPrigIndirectionSettingsCommandVisible == null)
                    m_isEditPrigIndirectionSettingsCommandVisible = new ApplicationProperty<bool>(true);
                return m_isEditPrigIndirectionSettingsCommandVisible;
            }
        }

        AddPrigAssemblyForMSCorLibCommand m_addPrigAssemblyForMSCorLibCommand;
        public ICommand AddPrigAssemblyForMSCorLibCommand
        {
            get
            {
                if (m_addPrigAssemblyForMSCorLibCommand == null)
                    m_addPrigAssemblyForMSCorLibCommand = BuildUpCommand(new AddPrigAssemblyForMSCorLibCommand(this));
                return m_addPrigAssemblyForMSCorLibCommand;
            }
        }

        AddPrigAssemblyCommand m_addPrigAssemblyCommand;
        public ICommand AddPrigAssemblyCommand
        {
            get
            {
                if (m_addPrigAssemblyCommand == null)
                    m_addPrigAssemblyCommand = BuildUpCommand(new AddPrigAssemblyCommand(this));
                return m_addPrigAssemblyCommand;
            }
        }

        EnableTestAdapterCommand m_enableTestAdapterCommand;
        public ICommand EnableTestAdapterCommand
        {
            get
            {
                if (m_enableTestAdapterCommand == null)
                {
                    var canExecuteSource =
                            IsTestAdapterEnabled.Select(_ => Unit.Default).
                            Merge(CurrentProject.Select(_ => Unit.Default)).
                            Select(_ => !IsTestAdapterEnabled.Value && CurrentProject.Value != null);
                    m_enableTestAdapterCommand = BuildUpCommand(new EnableTestAdapterCommand(this, canExecuteSource));
                }
                return m_enableTestAdapterCommand;
            }
        }

        TestAdapterBeforeQueryStatusCommand m_testAdapterBeforeQueryStatusCommand;
        public ICommand TestAdapterBeforeQueryStatusCommand
        {
            get
            {
                if (m_testAdapterBeforeQueryStatusCommand == null)
                    m_testAdapterBeforeQueryStatusCommand = BuildUpCommand(new TestAdapterBeforeQueryStatusCommand(this));
                return m_testAdapterBeforeQueryStatusCommand;
            }
        }

        DisableTestAdapterCommand m_disableTestAdapterCommand;
        public ICommand DisableTestAdapterCommand
        {
            get
            {
                if (m_disableTestAdapterCommand == null)
                    m_disableTestAdapterCommand = BuildUpCommand(new DisableTestAdapterCommand(this, IsTestAdapterEnabled));
                return m_disableTestAdapterCommand;
            }
        }

        RegisterPrigCommand m_registerPrigCommand;
        public ICommand RegisterPrigCommand
        {
            get
            {
                if (m_registerPrigCommand == null)
                    m_registerPrigCommand = BuildUpCommand(new RegisterPrigCommand(this));
                return m_registerPrigCommand;
            }
        }

        UnregisterPrigCommand m_unregisterPrigCommand;
        public ICommand UnregisterPrigCommand
        {
            get
            {
                if (m_unregisterPrigCommand == null)
                    m_unregisterPrigCommand = BuildUpCommand(new UnregisterPrigCommand(this));
                return m_unregisterPrigCommand;
            }
        }

        EditPrigIndirectionSettingsCommand m_editPrigIndirectionSettingsCommand;
        public ICommand EditPrigIndirectionSettingsCommand
        {
            get
            {
                if (m_editPrigIndirectionSettingsCommand == null)
                    m_editPrigIndirectionSettingsCommand = BuildUpCommand(new EditPrigIndirectionSettingsCommand(this));
                return m_editPrigIndirectionSettingsCommand;
            }
        }

        RemovePrigAssemblyCommand m_removePrigAssemblyCommand;
        public ICommand RemovePrigAssemblyCommand
        {
            get
            {
                if (m_removePrigAssemblyCommand == null)
                    m_removePrigAssemblyCommand = BuildUpCommand(new RemovePrigAssemblyCommand(this));
                return m_removePrigAssemblyCommand;
            }
        }

        EditPrigIndirectionSettingsBeforeQueryStatusCommand m_editPrigIndirectionSettingsBeforeQueryStatusCommand;
        public ICommand EditPrigIndirectionSettingsBeforeQueryStatusCommand
        {
            get
            {
                if (m_editPrigIndirectionSettingsBeforeQueryStatusCommand == null)
                    m_editPrigIndirectionSettingsBeforeQueryStatusCommand = BuildUpCommand(new EditPrigIndirectionSettingsBeforeQueryStatusCommand(this));
                return m_editPrigIndirectionSettingsBeforeQueryStatusCommand;
            }
        }

        OnBuildDoneCommand m_onBuildDoneCommand;
        public ICommand OnBuildDoneCommand
        {
            get
            {
                if (m_onBuildDoneCommand == null)
                    m_onBuildDoneCommand = BuildUpCommand(new OnBuildDoneCommand(this));
                return m_onBuildDoneCommand;
            }
        }

        ProjectRemovedCommand m_projectRemovedCommand;
        public ICommand ProjectRemovedCommand
        {
            get
            {
                if (m_projectRemovedCommand == null)
                    m_projectRemovedCommand = BuildUpCommand(new ProjectRemovedCommand(this));
                return m_projectRemovedCommand;
            }
        }


        
        public void SetToCurrentProjectIfSupported(Project proj)
        {
            CurrentProject.Value = IsSupportedProject(proj) ? proj : null;
        }

        static bool IsSupportedProject(Project proj)
        {
            return proj != null &&
                   (proj.Kind == VSConstantsAlternative.UICONTEXT.CSharpProject_string ||
                    proj.Kind == VSConstantsAlternative.UICONTEXT.FSharpProject_string ||
                    proj.Kind == VSConstantsAlternative.UICONTEXT.VBProject_string);
        }

        public void SetEditPrigIndirectionSettingsCommandVisibility(ProjectItem projItem)
        {
            if (projItem == null)
            {
                IsEditPrigIndirectionSettingsCommandVisible.Value = false;
                return;
            }

            var ext = Path.GetExtension(projItem.Name);
            IsEditPrigIndirectionSettingsCommandVisible.Value = string.Equals(ext, ".prig", StringComparison.InvariantCultureIgnoreCase);
        }

        public bool HasEnabledTestAdapter()
        {
            return IsTestAdapterEnabled.Value && CurrentProject.Value != null;
        }

        public bool HasEnabledTestAdapter(Project proj)
        {
            return HasEnabledTestAdapter() && CurrentProject.Value.Name == proj.Name;
        }

        public Project GetCurrentProjectOrException()
        {
            var proj = CurrentProject.Value;
            if (proj == null)
                throw new InvalidOperationException(PrigResources.GetString("CurrentProjectIsntSelectedMessage"));
            return proj;
        }

        public Project[] GetTargetProjects(DTE dte)
        {
            if (dte == null)
                return new Project[0];

            var sln = dte.Solution;
            if (sln == null)
                return new Project[0];

            var projs = sln.Projects;
            if (projs == null)
                return new Project[0];

            return projs.OfType<Project>().Where(IsSupportedProject).ToArray();
        }



        MachineWideProcesses m_mwProc;

        internal void BeginMachineWideProcessProgress(MachineWideProcesses mwProc)
        {
            m_mwProc = mwProc;
            Statusbar.BeginProgress(100u);
        }

        internal void ReportProfilerStatusCheckingProgress(uint prog, ProfilerLocation profLoc)
        {
            var msg = string.Format(PrigResources.GetString("CheckingInstallationStatusForProfiler_0_MessageFormat"), profLoc.PathOfInstalling);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportNuGetPackageCreatingProgress(uint prog, string pkgName)
        {
            var msg = string.Format(PrigResources.GetString("CreatingNugetPackage_0_MessageFormat"), pkgName);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportNuGetPackageCreatedProgress(uint prog, string stdout)
        {
            Statusbar.ReportProgress(stdout, prog);
        }

        internal void ReportNuGetSourceProcessingProgress(uint prog, string path, string name)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingNugetSource_0_As_1_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(PrigResources.GetString(resName), path, name);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportNuGetSourceProcessedProgress(uint prog, string stdout)
        {
            Statusbar.ReportProgress(stdout, prog);
        }

        internal void ReportEnvironmentVariableProcessingProgress(uint prog, string name, string value)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingEnvironmentVariable_0_As_1_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(PrigResources.GetString(resName), value, name);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportEnvironmentVariableProcessedProgress(uint prog)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessedEnvironmentVariable_{0}_Message", m_mwProc);
            var msg = PrigResources.GetString(resName);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportProfilerProcessingProgress(uint prog, ProfilerLocation profLoc)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingProfiler_0_ToRegistry_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(PrigResources.GetString(resName), profLoc.PathOfInstalling);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportProfilerProcessedProgress(uint prog, string stdout)
        {
            Statusbar.ReportProgress(stdout, prog);
        }

        internal void ReportPrigSourceProcessingProgress(uint prog, string pkgName, string src)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingDefaultSource_0_AsPackage_1_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(PrigResources.GetString(resName), src, pkgName);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportPrigSourceProcessedProgress(uint prog, string stdout)
        {
            Statusbar.ReportProgress(stdout, prog);
        }

        static string GetSkippedMachineWideProcessMessage(MachineWideProcesses mwProc, SkippedReasons reason)
        {
            var resName = string.Format("SkippedMachineWideProcess_{0}_{1}_Message", mwProc, reason);
            return PrigResources.GetString(resName);
        }

        internal void ShowSkippedMachineWideProcessMessage(SkippedReasons reason)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var msg = GetSkippedMachineWideProcessMessage(m_mwProc, reason);
            ShowMessageBox(msg, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
        }

        internal void EndSkippedMachineWideProcessProgress(SkippedReasons reason)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var msg = GetSkippedMachineWideProcessMessage(m_mwProc, reason);
            Statusbar.EndProgress();
            Statusbar.Text.Value = msg;
            m_mwProc = MachineWideProcesses.None;
        }

        static string GetCompletedMachineWideProcessMessage(MachineWideProcesses mwProc)
        {
            var resName = string.Format("CompletedMachineWideProcess_{0}_Message", mwProc);
            return PrigResources.GetString(resName);
        }

        internal bool ConfirmRestartingVisualStudioToTakeEffect()
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var msg = GetCompletedMachineWideProcessMessage(m_mwProc);
            var cfmMsg = string.Format(PrigResources.GetString("_0_YouMustRestartVisualStudioForTheseChangesToTakeEffectMessageFormat"), msg);
            var ret = ShowMessageBox(cfmMsg, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGICON.OLEMSGICON_WARNING);
            return ret == VSConstants.MessageBoxResult.IDYES;
        }

        internal void EndCompletedMachineWideProcessProgress()
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var msg = GetCompletedMachineWideProcessMessage(m_mwProc);
            Statusbar.EndProgress();
            Statusbar.Text.Value = msg;
            m_mwProc = MachineWideProcesses.None;
        }

        internal void ShowVisualStudioHasNotBeenElevatedYetMessage()
        {
            var msg = PrigResources.GetString("VisualStudioHasNotBeenElevatedYetMessage");
            ShowMessageBox(msg, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
        }



        ProjectWideProcesses m_pwProc;

        internal void BeginProjectWideProcessProgress(ProjectWideProcesses pwProc)
        {
            m_pwProc = pwProc;
            Statusbar.BeginProgress(100u);
        }

        internal void ReportPackagePreparingProgress(uint prog)
        {
            var msg = PrigResources.GetString("CheckingCurrentProjectsPackagesMessage");
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportPackageInstallingProgress(uint prog, IVsPackageMetadata metadata)
        {
            var msg = string.Format(PrigResources.GetString("Installing_0_MessageFormat"), metadata.Id);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportPackageInstalledProgress(uint prog, IVsPackageMetadata metadata)
        {
            var msg = string.Format(PrigResources.GetString("Installed_0_MessageFormat"), metadata.Id);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportPackageReferenceAddedProgress(uint prog, IVsPackageMetadata metadata)
        {
            var msg = string.Format(PrigResources.GetString("ReferenceAdded_0_MessageFormat"), metadata.Id);
            Statusbar.ReportProgress(msg, prog);
        }

        internal void ReportProcessingProjectWideProcessProgress(uint prog, Project[] projs)
        {
            Debug.Assert(projs != null);
            ReportProcessingProjectWideProcessProgress(prog, string.Join(";", projs.Select(_ => _.Name)));
        }

        internal void ReportProcessingProjectWideProcessProgress(uint prog, string include)
        {
            Debug.Assert(m_pwProc != ProjectWideProcesses.None);
            var resName = string.Format("ProcessingProjectWideProcess_0_{0}_MessageFormat", m_pwProc);
            var msg = string.Format(PrigResources.GetString(resName), include);
            Statusbar.ReportProgress(msg, prog);
        }

        static string GetSkippedProjectWideProcessMessage(ProjectWideProcesses pwProc, SkippedReasons reason, string include)
        {
            var resName = string.Format("SkippedProjectWideProcessFor_0_{0}_{1}_MessageFormat", pwProc, reason);
            return string.Format(PrigResources.GetString(resName), include);
        }

        internal void EndSkippedProjectWideProcessProgress(SkippedReasons reason, string include)
        {
            Debug.Assert(m_pwProc != ProjectWideProcesses.None);
            var msg = GetSkippedProjectWideProcessMessage(m_pwProc, reason, include);
            Statusbar.EndProgress();
            Statusbar.Text.Value = msg;
            m_pwProc = ProjectWideProcesses.None;
        }

        internal void ShowSkippedProjectWideProcessMessage(SkippedReasons reason, string include)
        {
            Debug.Assert(m_pwProc != ProjectWideProcesses.None);
            var msg = GetSkippedProjectWideProcessMessage(m_pwProc, reason, include);
            ShowMessageBox(msg, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_WARNING);
        }

        static string GetCompletedProjectWideProcessMessage(ProjectWideProcesses pwProc, string include)
        {
            var resName = string.Format("CompletedProjectWideProcessFor_0_{0}_MessageFormat", pwProc);
            return string.Format(PrigResources.GetString(resName), include);
        }

        internal void EndCompletedProjectWideProcessProgress(Project[] projs)
        {
            Debug.Assert(projs != null);
            EndCompletedProjectWideProcessProgress(string.Join(";", projs.Select(_ => _.Name)));
        }

        internal void EndCompletedProjectWideProcessProgress(string include)
        {
            Debug.Assert(m_pwProc != ProjectWideProcesses.None);
            var msg = GetCompletedProjectWideProcessMessage(m_pwProc, include);
            Statusbar.EndProgress();
            Statusbar.Text.Value = msg;
            m_pwProc = ProjectWideProcesses.None;
        }

        internal void ShowCompletedProjectWideProcessMessage(string include)
        {
            Debug.Assert(m_pwProc != ProjectWideProcesses.None);
            var msg = GetCompletedProjectWideProcessMessage(m_pwProc, include);
            ShowMessageBox(msg, OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGICON.OLEMSGICON_INFO);
        }

        internal bool ConfirmRemovingPrigAssembly(string deletionalInclude)
        {
            var msg = string.Format(PrigResources.GetString("AreYouSureYouWantToRemovePrigAssembly_0_MessageFormat"), deletionalInclude);
            var ret = ShowMessageBox(msg, OLEMSGBUTTON.OLEMSGBUTTON_YESNO, OLEMSGICON.OLEMSGICON_QUERY);
            return ret == VSConstants.MessageBoxResult.IDYES;
        }
    }
}
