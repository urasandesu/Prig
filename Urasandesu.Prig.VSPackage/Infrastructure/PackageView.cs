/* 
 * File: PackageView.cs
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;

namespace Urasandesu.Prig.VSPackage.Infrastructure
{
    public abstract class PackageView : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            BuildUpPackageViewModel(Container);
            RegisterPackageController(Container);
            RegisterUIShell(Container);
            RegisterStatusbar(Container);
            RegisterLog(Container);
            RegisterMonitorSelection(Container);
        }

        IUnityContainer m_container;
        protected virtual IUnityContainer Container
        {
            get
            {
                if (m_container == null)
                    m_container = new UnityContainer();
                return m_container;
            }
        }

        protected abstract PackageViewModel PackageViewModel { get; }

        protected virtual void BuildUpPackageViewModel(IUnityContainer container)
        {
            container.BuildUp(PackageViewModel);
        }

        protected abstract void RegisterPackageController(IUnityContainer container);

        protected virtual void RegisterUIShell(IUnityContainer container)
        {
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            PackageViewModel.IsWaitCursorEnabled.Subscribe(_ =>
            {
                ErrorHandler.ThrowOnFailure(uiShell.SetWaitCursor());
            });
            PackageViewModel.MessageBoxParameter.Subscribe(_ =>
            {
                var result = 0;
                ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(MessageBoxParameter.ReservedUInt,
                                                                   ref MessageBoxParameter.ReservedGuid,
                                                                   _.Title,
                                                                   _.Text,
                                                                   MessageBoxParameter.EmptyHelpFile,
                                                                   MessageBoxParameter.EmptyHelpContextId,
                                                                   _.Button,
                                                                   _.DefaultButton,
                                                                   _.Icon,
                                                                   MessageBoxParameter.NotSysAlert,
                                                                   out result));
                _.Result = (VSConstants.MessageBoxResult)result;
            });
            container.RegisterInstance(uiShell);
        }

        protected virtual void RegisterStatusbar(IUnityContainer container)
        {
            var statusbar = (IVsStatusbar)GetService(typeof(SVsStatusbar));
            PackageViewModel.Statusbar.ProgressState.Subscribe(_ => 
            {
                ErrorHandler.ThrowOnFailure(statusbar.Progress(ref ProgressState.Cookie, _.InProgress, _.Label, _.Value, _.Maximum));
            });
            PackageViewModel.Statusbar.Text.Subscribe(_ =>
            {
                ErrorHandler.ThrowOnFailure(statusbar.SetText(_));
            });
            container.RegisterInstance(statusbar);
        }

        protected abstract PackageLog NewLog(IUnityContainer container);

        protected virtual void RegisterLog(IUnityContainer container)
        {
            var activityLog = (IVsActivityLog)GetService(typeof(SVsActivityLog));
            container.RegisterInstance(activityLog);
            var log = NewLog(container);
            container.RegisterInstance(log);
        }

        protected virtual void RegisterMonitorSelection(IUnityContainer container)
        {
            var monitorSelection = (IVsMonitorSelection)GetService(typeof(SVsShellMonitorSelection));
            container.RegisterInstance(monitorSelection);
            container.RegisterType<MonitoringSelectionService, MonitoringSelectionService>(new ContainerControlledLifetimeManager());
        }
    }
}
