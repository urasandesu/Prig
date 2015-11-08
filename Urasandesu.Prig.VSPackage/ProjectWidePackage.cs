/* 
 * File: ProjectWidePackage.cs
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
using NuGet.VisualStudio;
using System;

namespace Urasandesu.Prig.VSPackage
{
    class ProjectWidePackage
    {
        public ProjectWidePackage(string pkgId, string pkgVer, Project targetProj)
        {
            if (string.IsNullOrEmpty(pkgId))
                throw new ArgumentNullException("pkgId");

            if (string.IsNullOrEmpty(pkgVer))
                throw new ArgumentNullException("pkgVer");

            if (targetProj == null)
                throw new ArgumentNullException("targetProj");

            PackageId = pkgId;
            PackageVersion = pkgVer;
            TargetProject = targetProj;
        }

        public string PackageId { get; private set; }
        public string PackageVersion { get; private set; }
        public Project TargetProject { get; private set; }

        public event Action PackagePreparing;
        public event VsPackageEventHandler PackageInstalling;
        public event VsPackageEventHandler PackageInstalled;
        public event VsPackageEventHandler PackageReferenceAdded;

        protected internal virtual void OnPackagePreparing()
        {
            var handler = PackagePreparing;
            if (handler == null)
                return;

            handler();
        }

        protected internal virtual void OnPackageInstalling(IVsPackageMetadata metadata)
        {
            var handler = PackageInstalling;
            if (handler == null)
                return;

            handler(metadata);
        }

        protected internal virtual void OnPackageInstalled(IVsPackageMetadata metadata)
        {
            var handler = PackageInstalled;
            if (handler == null)
                return;

            handler(metadata);
        }

        protected internal virtual void OnPackageReferenceAdded(IVsPackageMetadata metadata)
        {
            var handler = PackageReferenceAdded;
            if (handler == null)
                return;

            handler(metadata);
        }
    }
}
