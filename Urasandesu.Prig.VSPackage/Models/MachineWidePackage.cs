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

namespace Urasandesu.Prig.VSPackage.Models
{
    abstract class MachineWidePackage
    {
        public MachineWidePackage(string packageVersion)
        {
            if (string.IsNullOrEmpty(packageVersion))
                throw new ArgumentNullException("packageVersion");

            PackageVersion = packageVersion;
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

        public event Action Preparing
        {
            add { Prerequisite.Preparing += value; }
            remove { Prerequisite.Preparing -= value; }
        }
        public event Action<ProfilerLocation> ProfilerStatusChecking
        {
            add { Prerequisite.ProfilerStatusChecking += value; }
            remove { Prerequisite.ProfilerStatusChecking -= value; }
        }
        public event Action<MachineWideProcessResults> Completed;

        protected internal virtual void OnPreparing()
        {
            Prerequisite.OnPreparing();
        }

        protected internal virtual void OnProfilerStatusChecking(ProfilerLocation profLoc)
        {
            Prerequisite.OnProfilerStatusChecking(profLoc);
        }

        protected internal virtual void OnCompleted(MachineWideProcessResults result)
        {
            var handler = Completed;
            if (handler == null)
                return;

            handler(result);
        }
    }

    class MachineWideInstallation : MachineWidePackage
    {
        public MachineWideInstallation(string packageVersion)
            : base(packageVersion)
        { }

        public bool IsPrigSourceInstallationDisabled { get; set; }

        public event Action<string> NuGetPackageCreating;
        public event Action<string> NuGetPackageCreated;
        public event Action<string, string> NuGetSourceRegistering;
        public event Action<string> NuGetSourceRegistered;
        public event Action<string, string> EnvironmentVariableRegistering;
        public event Action<string, string> EnvironmentVariableRegistered;
        public event Action<ProfilerLocation> ProfilerRegistering;
        public event Action<string> ProfilerRegistered;
        public event Action<string, string> PrigSourceInstalling;
        public event Action<string> PrigSourceInstalled;

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

        protected internal virtual void OnNuGetSourceRegistering(string name, string toolsPath)
        {
            var handler = NuGetSourceRegistering;
            if (handler == null)
                return;

            handler(name, toolsPath);
        }

        protected internal virtual void OnNuGetSourceRegistered(string stdout)
        {
            var handler = NuGetSourceRegistered;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnEnvironmentVariableRegistering(string variableName, string variableValue)
        {
            var handler = EnvironmentVariableRegistering;
            if (handler == null)
                return;

            handler(variableName, variableValue);
        }

        protected internal virtual void OnEnvironmentVariableRegistered(string variableName, string variableValue)
        {
            var handler = EnvironmentVariableRegistered;
            if (handler == null)
                return;

            handler(variableName, variableValue);
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

        protected internal virtual void OnPrigSourceInstalling(string packageName, string source)
        {
            var handler = PrigSourceInstalling;
            if (handler == null)
                return;

            handler(packageName, source);
        }

        protected internal virtual void OnPrigSourceInstalled(string stdout)
        {
            var handler = PrigSourceInstalled;
            if (handler == null)
                return;

            handler(stdout);
        }
    }

    class MachineWideUninstallation : MachineWidePackage
    {
        public MachineWideUninstallation(string packageVersion)
            : base(packageVersion)
        { }

        public bool IsPrigSourceUninstallationDisabled { get; set; }

        public event Action<string> PrigSourceUninstalling;
        public event Action<string> PrigSourceUninstalled;
        public event Action<ProfilerLocation> ProfilerUnregistering;
        public event Action<string> ProfilerUnregistered;
        public event Action<string> EnvironmentVariableUnregistering;
        public event Action<string> EnvironmentVariableUnregistered;
        public event Action<string> NuGetSourceUnregistering;
        public event Action<string> NuGetSourceUnregistered;

        protected internal virtual void OnPrigSourceUninstalling(string packageName)
        {
            var handler = PrigSourceUninstalling;
            if (handler == null)
                return;

            handler(packageName);
        }

        protected internal virtual void OnPrigSourceUninstalled(string stdout)
        {
            var handler = PrigSourceUninstalled;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnProfilerUnregistering(ProfilerLocation profLoc)
        {
            var handler = ProfilerUnregistering;
            if (handler == null)
                return;

            handler(profLoc);
        }

        protected internal virtual void OnProfilerUnregistered(string stdout)
        {
            var handler = ProfilerUnregistered;
            if (handler == null)
                return;

            handler(stdout);
        }

        protected internal virtual void OnEnvironmentVariableUnregistering(string variableName)
        {
            var handler = EnvironmentVariableUnregistering;
            if (handler == null)
                return;

            handler(variableName);
        }

        protected internal virtual void OnEnvironmentVariableUnregistered(string variableName)
        {
            var handler = EnvironmentVariableUnregistered;
            if (handler == null)
                return;

            handler(variableName);
        }

        protected internal virtual void OnNuGetSourceUnregistering(string name)
        {
            var handler = NuGetSourceUnregistering;
            if (handler == null)
                return;

            handler(name);
        }

        protected internal virtual void OnNuGetSourceUnregistered(string stdout)
        {
            var handler = NuGetSourceUnregistered;
            if (handler == null)
                return;

            handler(stdout);
        }
    }
}
