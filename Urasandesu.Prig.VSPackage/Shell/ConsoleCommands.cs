/* 
 * File: ConsoleCommands.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urasandesu.Prig.VSPackage.Infrastructure;

namespace Urasandesu.Prig.VSPackage.Shell
{
    abstract class ConsoleCommand : ApplicationCommand
    {
        [Dependency]
        public ConsoleController Controller { protected get; set; }

        [Dependency]
        public ConsoleActivityLog ActivityLog { protected get; set; }

        protected ConsoleViewModel ViewModel { get { return (ConsoleViewModel)ApplicationViewModel; } }

        protected ConsoleCommand(ConsoleViewModel vm)
            : base(vm)
        { }

        protected ConsoleCommand(ConsoleViewModel vm, IObservable<bool> canExecuteSource)
            : base(vm, canExecuteSource)
        { }

        protected override void OnBegin(object parameter)
        {
            if (ActivityLog != null)
                ActivityLog.Info(string.Format("Begin {0}...", this));
        }

        protected override void OnError(object parameter, Exception e)
        {
            if (ActivityLog != null)
                ActivityLog.Error(string.Format("Error {0}...", e));
        }

        protected override void OnEnd(object parameter)
        {
            if (ActivityLog != null)
                ActivityLog.Info(string.Format("End {0}...", this));
        }
    }

    class RegisterPrigCommand : ConsoleCommand
    {
        public RegisterPrigCommand(ConsoleViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.PrepareRegisteringPrig(ViewModel);
        }
    }

    class UnregisterPrigCommand : ConsoleCommand
    {
        public UnregisterPrigCommand(ConsoleViewModel vm)
            : base(vm)
        { }

        protected override void InvokeCore(object parameter)
        {
            Controller.PrepareUnregisteringPrig(ViewModel);
        }
    }
}
