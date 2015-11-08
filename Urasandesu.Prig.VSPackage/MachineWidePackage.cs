/* 
 * File: MachineWidePackage.cs
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



using System;

namespace Urasandesu.Prig.VSPackage
{
    class MachineWidePackage
    {
        public MachineWidePackage(string pkgVer)
        {
            if (string.IsNullOrEmpty(pkgVer))
                throw new ArgumentNullException("pkgVer");

            PackageVersion = pkgVer;
        }

        public string PackageVersion { get; private set; }

        MachinePrerequisite m_preq;
        public MachinePrerequisite Prerequisite
        {
            get
            {
                if (m_preq == null)
                    m_preq = new MachinePrerequisite(PackageVersion);
                return m_preq;
            }
        }

        public event Action RegistrationPreparing
        {
            add { Prerequisite.RegistrationPreparing += value; }
            remove { Prerequisite.RegistrationPreparing -= value; }
        }
        public event Action<ProfilerLocation> ProfilerRegistrationStatusChecking
        {
            add { Prerequisite.ProfilerRegistrationStatusChecking += value; }
            remove { Prerequisite.ProfilerRegistrationStatusChecking -= value; }
        }
        public event Action<string> NuGetPackageCreating;
        public event Action<string> NuGetPackageCreated;
        public event Action<string, string> NuGetSourceRegistering;
        public event Action<string> NuGetSourceRegistered;
        public event Action<string, string> EnvironmentVariableRegistering;
        public event Action<string, string> EnvironmentVariableRegistered;
        public event Action<ProfilerLocation> ProfilerRegistering;
        public event Action<string> ProfilerRegistered;
        public event Action<string, string> DefaultSourceInstalling;
        public event Action<string> DefaultSourceInstalled;
        public event Action<RegistrationResults> RegistrationCompleted;

        protected internal virtual void OnRegistrationPreparing()
        {
            Prerequisite.OnRegistrationPreparing();
        }

        protected internal virtual void OnProfilerInstallationStatusChecking(ProfilerLocation profLoc)
        {
            Prerequisite.OnProfilerInstallationStatusChecking(profLoc);
        }

        protected internal virtual void OnNuGetPackageCreating(string packageName)
        {
            var handler = NuGetPackageCreating;
            if (handler == null)
                return;

            handler(packageName);
        }

        protected internal virtual void OnNuGetPackageCreated(string stdout)
        {
            var handler = NuGetPackageCreated;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnNuGetSourceRegistering(string toolsPath, string name)
        {
            var handler = NuGetSourceRegistering;
            if (handler == null)
                return;

            handler(toolsPath, name);
        }

        protected internal virtual void OnNuGetSourceRegistered(string stdout)
        {
            var handler = NuGetSourceRegistered;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnEnvironmentVariableRegistering(string variableValue, string variableName)
        {
            var handler = EnvironmentVariableRegistering;
            if (handler == null)
                return;

            handler(variableValue, variableName);
        }

        protected internal virtual void OnEnvironmentVariableRegistered(string variableValue, string variableName)
        {
            var handler = EnvironmentVariableRegistered;
            if (handler == null)
                return;

            handler(variableValue, variableName);
        }

        protected internal virtual void OnProfilerRegistering(ProfilerLocation profLoc)
        {
            var handler = ProfilerRegistering;
            if (handler == null)
                return;

            handler(profLoc);
        }

        protected internal virtual void OnProfilerRegistered(string stdout)
        {
            var handler = ProfilerRegistered;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnDefaultSourceInstalling(string source, string packageName)
        {
            var handler = DefaultSourceInstalling;
            if (handler == null)
                return;

            handler(source, packageName);
        }

        protected internal virtual void OnDefaultSourceInstalled(string stdout)
        {
            var handler = DefaultSourceInstalled;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnRegistrationCompleted(RegistrationResults result)
        {
            var handler = RegistrationCompleted;
            if (handler == null)
                return;

            handler(result);
        }
    }
}
