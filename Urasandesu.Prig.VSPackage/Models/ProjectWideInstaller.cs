/* 
 * File: ProjectWideInstaller.cs
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
using NuGet.VisualStudio;
using System;

namespace Urasandesu.Prig.VSPackage.Models
{
    class ProjectWideInstaller : IProjectWideInstaller
    {
        [Dependency]
        public IVsPackageInstallerServices InstallerServices { private get; set; }

        [Dependency]
        public IVsPackageInstaller Installer { private get; set; }

        [Dependency]
        public IVsPackageInstallerEvents InstallerEvents { private get; set; }

        [Dependency]
        public IVsPackageUninstaller Uninstaller { private get; set; }

        public void Install(ProjectWidePackage pwPkg)
        {
            if (pwPkg == null)
                throw new ArgumentNullException("pwPkg");

            pwPkg.OnPackagePreparing();

            var targetProj = pwPkg.TargetProject;
            var pkgId = pwPkg.PackageId;
            var pkgVer = pwPkg.PackageVersion;
            if (!InstallerServices.IsPackageInstalledEx(targetProj, pkgId, pkgVer))
            {
                if (InstallerServices.IsPackageInstalled(targetProj, pkgId))
                    Uninstaller.UninstallPackage(targetProj, pkgId, false);
                InstallPackage(pwPkg, targetProj, pkgId, pkgVer);
            }
        }

        void InstallPackage(ProjectWidePackage pwPkg, Project proj, string pkgId, string pkgVer)
        {
            try
            {
                InstallerEvents.PackageInstalling += pwPkg.OnPackageInstalling;
                InstallerEvents.PackageInstalled += pwPkg.OnPackageInstalled;
                InstallerEvents.PackageReferenceAdded += pwPkg.OnPackageReferenceAdded;

                var source = default(string);
                var ignoreDependencies = false;
                Installer.InstallPackage(source, proj, pkgId, pkgVer, ignoreDependencies);
            }
            finally
            {
                InstallerEvents.PackageInstalling -= pwPkg.OnPackageInstalling;
                InstallerEvents.PackageInstalled -= pwPkg.OnPackageInstalled;
                InstallerEvents.PackageReferenceAdded -= pwPkg.OnPackageReferenceAdded;
            }
        }
    }
}
