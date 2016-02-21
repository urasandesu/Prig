/* 
 * File: PrigCommands.cs
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
    abstract class PrigCommand : PackageCommand
    {
        [Dependency]
        public PrigController Controller { protected get; set; }

        protected PrigViewModel ViewModel { get { return (PrigViewModel)PackageViewModel; } }

        protected PrigCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected PrigCommand(PrigViewModel vm, IObservable<bool> canExecuteSource)
            : base(vm, canExecuteSource)
        { }
    }

    class AddPrigAssemblyForMSCorLibCommand : PrigCommand
    {
        public AddPrigAssemblyForMSCorLibCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.AddPrigAssemblyForMSCorLib(ViewModel);
        }
    }

    class AddPrigAssemblyCommand : PrigCommand
    {
        public AddPrigAssemblyCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.AddPrigAssembly(ViewModel);
        }
    }

    class EnableTestAdapterCommand : PrigCommand
    {
        public EnableTestAdapterCommand(PrigViewModel vm, IObservable<bool> canExecuteSource)
            : base(vm, canExecuteSource)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.EnableTestAdapter(ViewModel);
        }
    }

    class TestAdapterBeforeQueryStatusCommand : PrigCommand
    {
        public TestAdapterBeforeQueryStatusCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void OnBegin(object parameter) { }
        protected override void OnError(object parameter, Exception e) { }
        protected override void OnEnd(object parameter) { }

        protected override void InvokeCore(object parameter)
        {
            Controller.BeforeQueryStatusTestAdapter(ViewModel);
        }
    }

    class DisableTestAdapterCommand : PrigCommand
    {
        public DisableTestAdapterCommand(PrigViewModel vm, IObservable<bool> canExecuteSource)
            : base(vm, canExecuteSource)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.DisableTestAdapter(ViewModel);
        }
    }

    class RegisterPrigCommand : PrigCommand
    {
        public RegisterPrigCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.PrepareRegisteringPrig(ViewModel);
        }
    }

    class UnregisterPrigCommand : PrigCommand
    {
        public UnregisterPrigCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.PrepareUnregisteringPrig(ViewModel);
        }
    }

    class EditPrigIndirectionSettingsCommand : PrigCommand
    {
        public EditPrigIndirectionSettingsCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.EditPrigIndirectionSettings(ViewModel);
        }
    }

    class RemovePrigAssemblyCommand : PrigCommand
    {
        public RemovePrigAssemblyCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.RemovePrigAssembly(ViewModel);
        }
    }

    class EditPrigIndirectionSettingsBeforeQueryStatusCommand : PrigCommand
    {
        public EditPrigIndirectionSettingsBeforeQueryStatusCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.BeforeQueryStatusEditPrigIndirectionSettings(ViewModel);
        }
    }

    class OnBuildDoneCommand : PrigCommand
    {
        public OnBuildDoneCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.OnBuildDone(ViewModel);
        }
    }

    class ProjectRemovedCommand : PrigCommand
    {
        public ProjectRemovedCommand(PrigViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.OnProjectRemoved(ViewModel, (Project)parameter);
        }
    }
}
