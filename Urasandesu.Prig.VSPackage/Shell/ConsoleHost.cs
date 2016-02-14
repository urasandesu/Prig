/* 
 * File: ConsoleHost.cs
 * 
 * Author: Akira Sugiura (urasandesu@gmail.com)
 * 
 * 
 * Copyright (c) 2016 Akira Sugiura
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
using System;
using Urasandesu.Prig.VSPackage.Models;

namespace Urasandesu.Prig.VSPackage.Shell
{
    public class ConsoleHost
    {
        public static int RegisterPrig()
        {
            var console = new ConsoleHost();
            console.Initialize();
            console.OnRegisterPrig();
            return console.ExitCode;
        }

        public static int UnregisterPrig()
        {
            var console = new ConsoleHost();
            console.Initialize();
            console.OnUnregisterPrig();
            return console.ExitCode;
        }

        protected ConsoleHost()
        {
//#if DEBUG
//            System.Diagnostics.Debugger.Launch();
//#endif
        }

        internal void Initialize()
        {
            BuildUpConsoleViewModel(Container);
            RegisterConsoleController(Container);
            RegisterPrigComponent(Container);
            RegisterUIShell(Container);
            RegisterLog(Container);
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

        ConsoleViewModel m_viewModel;
        ConsoleViewModel ViewModel
        {
            get
            {
                if (m_viewModel == null)
                    m_viewModel = new ConsoleViewModel();
                return m_viewModel;
            }
        }

        internal int ExitCode
        {
            get
            {
                return ViewModel.ExitCode.Value;
            }
        }

        protected virtual void BuildUpConsoleViewModel(IUnityContainer container)
        {
            container.BuildUp(ViewModel);
        }

        protected virtual void RegisterConsoleController(IUnityContainer container)
        {
            container.RegisterType<ConsoleController, ConsoleController>(new ContainerControlledLifetimeManager());
        }

        void RegisterPrigComponent(IUnityContainer container)
        {
            container.RegisterType<IEnvironmentRepository, EnvironmentRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<INuGetExecutor, NuGetExecutor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IRegsvr32Executor, Regsvr32Executor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IPrigExecutor, PrigExecutor>(new ContainerControlledLifetimeManager());
            container.RegisterType<IMachineWideInstaller, MachineWideInstaller>(new ContainerControlledLifetimeManager());
        }

        void RegisterUIShell(IUnityContainer container)
        {
            ViewModel.Message.Subscribe(_ =>
            {
                Console.WriteLine(_);
            });
        }

        void RegisterLog(IUnityContainer container)
        {
            container.RegisterInstance(new ConsoleActivityLog());
        }

        internal void OnRegisterPrig()
        {
            ViewModel.RegisterPrigCommand.Execute(this);
        }

        internal void OnUnregisterPrig()
        {
            ViewModel.UnregisterPrigCommand.Execute(this);
        }
    }
}
