/* 
 * File: ConsoleViewModel.cs
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



using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Urasandesu.Prig.VSPackage.Infrastructure;
using Urasandesu.Prig.VSPackage.Models;

namespace Urasandesu.Prig.VSPackage.Shell
{
    class ConsoleViewModel : ApplicationViewModel
    {
        ApplicationProperty<string> m_message;
        public ApplicationProperty<string> Message
        {
            get
            {
                if (m_message == null)
                    m_message = new ApplicationProperty<string>();
                return m_message;
            }
        }

        ApplicationProperty<int> m_exitCode;
        public ApplicationProperty<int> ExitCode
        {
            get
            {
                if (m_exitCode == null)
                    m_exitCode = new ApplicationProperty<int>();
                return m_exitCode;
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



        MachineWideProcesses m_mwProc;

        internal void BeginMachineWideProcessProgress(MachineWideProcesses mwProc)
        {
            m_mwProc = mwProc;
        }

        internal void ReportProfilerStatusCheckingProgress(ProfilerLocation profLoc)
        {
            var msg = string.Format(ConsoleResources.GetString("CheckingInstallationStatusForProfiler_0_MessageFormat"), profLoc.PathOfInstalling);
            Message.Value = msg;
        }

        internal void ReportNuGetPackageCreatingProgress(string pkgName)
        {
            var msg = string.Format(ConsoleResources.GetString("CreatingNugetPackage_0_MessageFormat"), pkgName);
            Message.Value = msg;
        }

        internal void ReportNuGetPackageCreatedProgress(string stdout)
        {
            Message.Value = stdout;
        }

        internal void ReportNuGetSourceProcessingProgress(string path, string name)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingNugetSource_0_As_1_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(ConsoleResources.GetString(resName), path, name);
            Message.Value = msg;
        }

        internal void ReportNuGetSourceProcessedProgress(string stdout)
        {
            Message.Value = stdout;
        }

        internal void ReportEnvironmentVariableProcessingProgress(string name, string value)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingEnvironmentVariable_0_As_1_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(ConsoleResources.GetString(resName), value, name);
            Message.Value = msg;
        }

        internal void ReportEnvironmentVariableProcessedProgress()
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessedEnvironmentVariable_{0}_Message", m_mwProc);
            var msg = ConsoleResources.GetString(resName);
            Message.Value = msg;
        }

        internal void ReportProfilerProcessingProgress(ProfilerLocation profLoc)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var resName = string.Format("ProcessingProfiler_0_ToRegistry_{0}_MessageFormat", m_mwProc);
            var msg = string.Format(ConsoleResources.GetString(resName), profLoc.PathOfInstalling);
            Message.Value = msg;
        }

        internal void ReportProfilerProcessedProgress(string stdout)
        {
            Message.Value = stdout;
        }

        static string GetSkippedMachineWideProcessMessage(MachineWideProcesses mwProc, SkippedReasons reason)
        {
            var resName = string.Format("SkippedMachineWideProcess_{0}_{1}_Message", mwProc, reason);
            return ConsoleResources.GetString(resName);
        }

        internal void ShowSkippedMachineWideProcessMessage(SkippedReasons reason)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var msg = GetSkippedMachineWideProcessMessage(m_mwProc, reason);
            Message.Value = msg;
        }

        internal void EndSkippedMachineWideProcessProgress(SkippedReasons reason)
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            m_mwProc = MachineWideProcesses.None;
            ExitCode.Value = 10 + (int)reason;
        }

        static string GetCompletedMachineWideProcessMessage(MachineWideProcesses mwProc)
        {
            var resName = string.Format("CompletedMachineWideProcess_{0}_Message", mwProc);
            return ConsoleResources.GetString(resName);
        }

        internal void ShowCompletedMachineWideProcessMessage()
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            var msg = GetCompletedMachineWideProcessMessage(m_mwProc);
            Message.Value = msg;
        }

        internal void EndCompletedMachineWideProcessProgress()
        {
            Debug.Assert(m_mwProc != MachineWideProcesses.None);
            m_mwProc = MachineWideProcesses.None;
            ExitCode.Value = 0;
        }

        internal void ShowCurrentConsoleHasNotBeenElevatedYetMessage()
        {
            var msg = ConsoleResources.GetString("CurrentConsoleHasNotBeenElevatedYetMessage");
            Message.Value = msg;
        }
    }
}
