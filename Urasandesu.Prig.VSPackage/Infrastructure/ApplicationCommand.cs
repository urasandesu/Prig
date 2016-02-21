/* 
 * File: ApplicationCommand.cs
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



using Codeplex.Reactive;
using Microsoft.Practices.Unity;
using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace Urasandesu.Prig.VSPackage.Infrastructure
{
    public abstract class ApplicationCommand : ReactiveCommand
    {
        protected ApplicationViewModel ApplicationViewModel { get; private set; }

        [Dependency]
        public IUnityContainer Container { protected get; set; }

        protected ApplicationCommand(ApplicationViewModel vm)
            : this(vm, Scheduler.Immediate)
        { }

        protected ApplicationCommand(ApplicationViewModel vm, IScheduler scheduler)
            : this(vm, Observable.Never<bool>(), scheduler)
        { }

        protected ApplicationCommand(ApplicationViewModel vm, IObservable<bool> canExecuteSource, bool initialValue = true)
            : this(vm, canExecuteSource, Scheduler.Immediate, initialValue)
        { }

        protected ApplicationCommand(ApplicationViewModel vm, IObservable<bool> canExecuteSource, IScheduler scheduler, bool initialValue = true)
            : base(canExecuteSource, scheduler, initialValue)
        {
            if (vm == null)
                throw new ArgumentNullException("vm");

            ApplicationViewModel = vm;
            this.Subscribe(Invoke);
        }

        void Invoke(object parameter)
        {
            try
            {
                OnBegin(parameter);
                InvokeCore(parameter);
            }
            catch (Exception e)
            {
                OnError(parameter, e);
                throw;
            }
            finally
            {
                OnEnd(parameter);
            }
        }

        protected abstract void OnBegin(object parameter);
        protected abstract void InvokeCore(object parameter);
        protected abstract void OnError(object parameter, Exception e);
        protected abstract void OnEnd(object parameter);
    }
}
