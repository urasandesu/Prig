/* 
 * File: PrigPackageCommands.cs
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
using System;
using Urasandesu.Prig.VSPackage.Infrastructure;

namespace Urasandesu.Prig.VSPackage
{
    abstract class PrigPackageCommand : PackageCommand
    {
        [Dependency]
        public PrigPackageController Controller { protected get; set; }

        protected PrigPackageViewModel ViewModel { get { return (PrigPackageViewModel)PackageViewModel; } }

        protected PrigPackageCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected PrigPackageCommand(PrigPackageViewModel viewModel, IObservable<bool> canExecuteSource)
            : base(viewModel, canExecuteSource)
        { }
    }

    class AddPrigAssemblyForMSCorLibCommand : PrigPackageCommand
    {
        public AddPrigAssemblyForMSCorLibCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.AddPrigAssemblyForMSCorLib(ViewModel);
        }
    }

    class AddPrigAssemblyCommand : PrigPackageCommand
    {
        public AddPrigAssemblyCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.AddPrigAssembly(ViewModel);
        }
    }

    class EnableTestAdapterCommand : PrigPackageCommand
    {
        public EnableTestAdapterCommand(PrigPackageViewModel viewModel, IObservable<bool> canExecuteSource)
            : base(viewModel, canExecuteSource)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.EnableTestAdapter(ViewModel);
        }
    }

    class TestAdapterBeforeQueryStatusCommand : PrigPackageCommand
    {
        public TestAdapterBeforeQueryStatusCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void OnBegin(object parameter) { }
        protected override void OnError(object parameter, Exception e) { }
        protected override void OnEnd(object parameter) { }

        protected override void InvokeCore(object parameter)
        {
            Controller.BeforeQueryStatusTestAdapter(ViewModel);
        }
    }

    class DisableTestAdapterCommand : PrigPackageCommand
    {
        public DisableTestAdapterCommand(PrigPackageViewModel viewModel, IObservable<bool> canExecuteSource)
            : base(viewModel, canExecuteSource)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.DisableTestAdapter(ViewModel);
        }
    }

    class EditPrigIndirectionSettingsCommand : PrigPackageCommand
    {
        public EditPrigIndirectionSettingsCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.EditPrigIndirectionSettings(ViewModel);
        }
    }

    class RemovePrigAssemblyCommand : PrigPackageCommand
    {
        public RemovePrigAssemblyCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.RemovePrigAssembly(ViewModel);
        }
    }

    class EditPrigIndirectionSettingsBeforeQueryStatusCommand : PrigPackageCommand
    {
        public EditPrigIndirectionSettingsBeforeQueryStatusCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.BeforeQueryStatusEditPrigIndirectionSettings(ViewModel);
        }
    }

    class OnBuildDoneCommand : PrigPackageCommand
    {
        public OnBuildDoneCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.OnBuildDone(ViewModel);
        }
    }

    class ProjectRemovedCommand : PrigPackageCommand
    {
        public ProjectRemovedCommand(PrigPackageViewModel viewModel)
            : base(viewModel)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.OnProjectRemoved(ViewModel, (Project)parameter);
        }
    }
}
