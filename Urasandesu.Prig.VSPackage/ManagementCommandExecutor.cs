/* 
 * File: ManagementCommandExecutor.cs
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
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Urasandesu.Prig.VSPackage
{
    class ManagementCommandExecutor : IManagementCommandExecutor
    {
        public void Execute(ManagementCommandInfo mci)
        {
            if (mci == null)
                throw new ArgumentNullException("mci");

            mci.OnCommandExecuting();

            var command = mci.Command;
            var targetProj = mci.TargetProject;
            ExecuteCommand(command, targetProj);

            mci.OnCommandExecuted();
        }

        protected virtual Runspace NewRunspace()
        {
            return RunspaceFactory.CreateRunspace();
        }

        void ExecuteCommand(string command, Project proj)
        {
            using (var runspace = NewRunspace())
            {
                runspace.Open();
                if (proj != null)
                    runspace.SessionStateProxy.SetVariable("Project", proj);
                var results = default(Collection<PSObject>);
                command = "Set-ExecutionPolicy RemoteSigned -Scope Process -Force\r\n" + command;
                using (var pipeline = runspace.CreatePipeline(command, false))
                    results = pipeline.Invoke();
            }
        }
    }
}
