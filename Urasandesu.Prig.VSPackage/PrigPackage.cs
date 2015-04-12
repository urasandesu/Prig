/* 
 * File: PrigPackage.cs
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
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using NuGet.VisualStudio;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Urasandesu.Prig.VSPackage.Infrastructure;

namespace Urasandesu.Prig.VSPackage
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.PrigPackageString)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.SolutionExists_string)]
    public sealed class PrigPackage : PackageView
    {
        public PrigPackage()
        {
            //System.Diagnostics.Debugger.Launch();

            Debug.WriteLine(string.Format("Entering constructor for: {0}", this));
        }

        protected override void Initialize()
        {
            Debug.WriteLine (string.Format("Entering Initialize() of: {0}", this));
            base.Initialize();

            RegisterNuGetComponent(Container);
            RegisterMenuCommandService(Container);
            RegisterDTE(Container);
        }

        protected override PackageViewModel PackageViewModel
        {
            get { return ViewModel; }
        }

        PrigPackageViewModel m_viewModel;
        PrigPackageViewModel ViewModel
        {
            get
            {
                if (m_viewModel == null)
                    m_viewModel = new PrigPackageViewModel();
                return m_viewModel;
            }
        }

        protected override void RegisterPackageController(IUnityContainer container)
        {
            container.RegisterType<PrigPackageController, PrigPackageController>(new ContainerControlledLifetimeManager());
        }

        void RegisterNuGetComponent(IUnityContainer container)
        {
            var componentModel = (IComponentModel)GetService(typeof(SComponentModel));
            
            var installerServices = componentModel.GetService<IVsPackageInstallerServices>();
            container.RegisterInstance(installerServices);
            
            var installer = componentModel.GetService<IVsPackageInstaller>();
            container.RegisterInstance(installer);

            var installerEvents = componentModel.GetService<IVsPackageInstallerEvents>();
            container.RegisterInstance(installerEvents);
        }

        void RegisterMenuCommandService(IUnityContainer container)
        {
            var menuCommandService = (IMenuCommandService)GetService(typeof(IMenuCommandService));
            menuCommandService.AddCommand(NewAddPrigAssemblyForMSCorLibCommand(ViewModel));
            menuCommandService.AddCommand(NewAddPrigAssemblyCommand(ViewModel));
            menuCommandService.AddCommand(NewEnableTestAdapterCommand(ViewModel));
            menuCommandService.AddCommand(NewDisableTestAdapterCommand(ViewModel));
            menuCommandService.AddCommand(NewEditPrigIndirectionSettingsCommand(ViewModel));
            menuCommandService.AddCommand(NewRemovePrigAssemblyCommand(ViewModel));
            container.RegisterInstance(menuCommandService);
        }

        static MenuCommand NewAddPrigAssemblyForMSCorLibCommand(PrigPackageViewModel viewModel)
        {
            var commandId = new CommandID(GuidList.AddPrigAssemblyForMSCorLibGroup, (int)PkgCmdIDList.AddPrigAssemblyForMSCorLibCommand);
            var handler = new EventHandler((sender, e) => viewModel.AddPrigAssemblyForMSCorLibCommand.Execute(sender));
            var menuCommand = new MenuCommand(handler, commandId);
            return menuCommand;
        }

        static MenuCommand NewAddPrigAssemblyCommand(PrigPackageViewModel viewModel)
        {
            var commandId = new CommandID(GuidList.AddPrigAssemblyGroup, (int)PkgCmdIDList.AddPrigAssemblyCommand);
            var handler = new EventHandler((sender, e) => viewModel.AddPrigAssemblyCommand.Execute(sender));
            var menuCommand = new MenuCommand(handler, commandId);
            return menuCommand;
        }

        static OleMenuCommand NewEnableTestAdapterCommand(PrigPackageViewModel viewModel)
        {
            var commandId = new CommandID(GuidList.MainMenuGroup, (int)PkgCmdIDList.EnableTestAdapterCommand);
            var handler = new EventHandler((sender, e) => viewModel.EnableTestAdapterCommand.Execute(sender));
            var menuCommand = new OleMenuCommand(handler, commandId);
            viewModel.EnableTestAdapterCommand.CanExecuteChanged += (sender, e) => menuCommand.Enabled = ((ICommand)sender).CanExecute(menuCommand);
            menuCommand.BeforeQueryStatus += (sender, e) => viewModel.TestAdapterBeforeQueryStatusCommand.Execute(sender);
            var text = "&Enable Test Adapter";
            viewModel.CurrentProject.Subscribe(project => menuCommand.Text = project == null ? text : text + " for " + project.Name);
            return menuCommand;
        }

        static OleMenuCommand NewDisableTestAdapterCommand(PrigPackageViewModel viewModel)
        {
            var commandId = new CommandID(GuidList.MainMenuGroup, (int)PkgCmdIDList.DisableTestAdapterCommand);
            var handler = new EventHandler((sender, e) => viewModel.DisableTestAdapterCommand.Execute(sender));
            var menuCommand = new OleMenuCommand(handler, commandId);
            viewModel.DisableTestAdapterCommand.CanExecuteChanged += (sender, e) => menuCommand.Enabled = ((ICommand)sender).CanExecute(menuCommand);
            menuCommand.Enabled = false;
            var text = "&Disable Test Adapter";
            viewModel.CurrentProject.Subscribe(project => menuCommand.Text = project == null ? text : text + " for " + project.Name);
            return menuCommand;
        }

        static OleMenuCommand NewEditPrigIndirectionSettingsCommand(PrigPackageViewModel viewModel)
        {
            var commandId = new CommandID(GuidList.EditPrigIndirectionSettingsGroup, (int)PkgCmdIDList.EditPrigIndirectionSettingsCommand);
            var handler = new EventHandler((sender, e) => viewModel.EditPrigIndirectionSettingsCommand.Execute(sender));
            var menuCommand = new OleMenuCommand(handler, commandId);
            viewModel.EditPrigIndirectionSettingsCommand.CanExecuteChanged += (sender, e) => menuCommand.Enabled = ((ICommand)sender).CanExecute(menuCommand);
            menuCommand.BeforeQueryStatus += (sender, e) => viewModel.EditPrigIndirectionSettingsBeforeQueryStatusCommand.Execute(sender);
            viewModel.IsEditPrigIndirectionSettingsCommandVisible.Subscribe(_ => menuCommand.Visible = _);
            return menuCommand;
        }

        static OleMenuCommand NewRemovePrigAssemblyCommand(PrigPackageViewModel viewModel)
        {
            var commandId = new CommandID(GuidList.EditPrigIndirectionSettingsGroup, (int)PkgCmdIDList.RemovePrigAssemblyCommand);
            var handler = new EventHandler((sender, e) => viewModel.RemovePrigAssemblyCommand.Execute(sender));
            var menuCommand = new OleMenuCommand(handler, commandId);
            viewModel.RemovePrigAssemblyCommand.CanExecuteChanged += (sender, e) => menuCommand.Enabled = ((ICommand)sender).CanExecute(menuCommand);
            menuCommand.BeforeQueryStatus += (sender, e) => viewModel.EditPrigIndirectionSettingsBeforeQueryStatusCommand.Execute(sender);
            viewModel.IsEditPrigIndirectionSettingsCommandVisible.Subscribe(_ => menuCommand.Visible = _);
            return menuCommand;
        }

        protected override PackageLog NewLog(IUnityContainer container)
        {
            return new PrigActivityLog(container.Resolve<IVsActivityLog>());
        }

        void RegisterDTE(IUnityContainer container)
        {
            var dte = (DTE)GetService(typeof(DTE));
            dte.Events.BuildEvents.OnBuildDone += (scope, action) => ViewModel.OnBuildDoneCommand.Execute(dte);
            dte.Events.SolutionEvents.ProjectRemoved += project => ViewModel.ProjectRemovedCommand.Execute(project);
            container.RegisterInstance(dte);
        }
    }
}
