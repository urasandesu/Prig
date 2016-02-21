/* 
 * File: IFixtureMixin.cs
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
using Microsoft.Win32;
using Moq;
using NuGet.VisualStudio;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.Microsoft.Win32;
using Urasandesu.Prig.VSPackage;
using Urasandesu.Prig.VSPackage.Models;

namespace Test.Urasandesu.Prig.VSPackage.TestUtilities.Mixins.Ploeh.AutoFixture
{
    static class IFixtureMixin
    {
        public static MachineWideInstaller NewMachineWideInstaller(this IFixture fixture)
        {
            var mwInstllr = new MachineWideInstaller();
            mwInstllr.EnvironmentRepository = fixture.Freeze<IEnvironmentRepository>();
            mwInstllr.NuGetExecutor = fixture.Freeze<INuGetExecutor>();
            mwInstllr.Regsvr32Executor = fixture.Freeze<IRegsvr32Executor>();
            mwInstllr.PrigExecutor = fixture.Freeze<IPrigExecutor>();
            return mwInstllr;
        }

        public static void FreezeInstalledEnvironment(this IFixture fixture)
        {
            var profLocs = new[] { new ProfilerLocation(RegistryView.Registry32, fixture.Create<string>()) };
            fixture.FreezeInstalledEnvironment(profLocs);
        }

        public static void FreezeInstalledEnvironment(this IFixture fixture, ProfilerLocation[] profLocs)
        {
            var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
            m.Setup(_ => _.GetProfilerLocations()).Returns(profLocs);
            m.Setup(_ => _.OpenRegistryBaseKey(It.IsAny<RegistryHive>(), It.IsAny<RegistryView>())).Returns(RegistryKeyMixin.DummyX86ClassesRootKey);
            m.Setup(_ => _.OpenRegistrySubKey(It.IsAny<RegistryKey>(), It.IsAny<string>())).Returns(RegistryKeyMixin.DummyX86InProcServer32Key);
            m.Setup(_ => _.GetRegistryValue(It.IsAny<RegistryKey>(), It.IsAny<string>())).Returns(fixture.Create<string>());
            m.Setup(_ => _.ExistsFile(It.IsAny<string>())).Returns(true);
            m.Setup(_ => _.GetFileDescription(It.IsAny<string>())).Returns("Prig Profiler 2.0.0 Type Library");
        }

        public static void FreezeUninstalledEnvironment(this IFixture fixture)
        {
            fixture.FreezeUninstalledEnvironment(fixture.Create<string>());
        }

        public static void FreezeUninstalledEnvironment(this IFixture fixture, string toolsPath)
        {
            var m = fixture.Freeze<Mock<IEnvironmentRepository>>();
            m.Setup(_ => _.GetToolsPath()).Returns(toolsPath);
        }

        public static NuGetExecutor NewNuGetExecutor(this IFixture fixture)
        {
            var nugetExecutor = new NuGetExecutor();
            nugetExecutor.EnvironmentRepository = fixture.Freeze<IEnvironmentRepository>();
            return nugetExecutor;
        }

        public static PrigExecutor NewPrigExecutor(this IFixture fixture)
        {
            var prigExecutor = new PrigExecutor();
            prigExecutor.EnvironmentRepository = fixture.Freeze<IEnvironmentRepository>();
            return prigExecutor;
        }

        public static ProjectWideInstaller NewProjectWideInstaller(this IFixture fixture)
        {
            var pwInstllr = new ProjectWideInstaller();
            pwInstllr.InstallerServices = fixture.Freeze<IVsPackageInstallerServices>();
            pwInstllr.Installer = fixture.Freeze<IVsPackageInstaller>();
            pwInstllr.InstallerEvents = fixture.Freeze<IVsPackageInstallerEvents>();
            pwInstllr.Uninstaller = fixture.Freeze<IVsPackageUninstaller>();
            return pwInstllr;
        }
        // We have to access all members that we want to mock in advance, because Embed Interop Types is set to true against NuGet.VisualStudio. 
        // See also the following issues: 
        // * [A COM type can't make its mock by same procedure as usual. · Issue #215 · Moq-moq4](https://github.com/Moq/moq4/issues/215)
        // * [A COM type can't make its proxy by same procedure as usual. · Issue #117 · castleproject-Core](https://github.com/castleproject/Core/issues/117)
        static void Realize(IVsPackageInstallerServices installerServices)
        {
            installerServices.GetInstalledPackages();
            installerServices.GetInstalledPackages(default(Project));
            installerServices.IsPackageInstalled(default(Project), default(string));
            installerServices.IsPackageInstalledEx(default(Project), default(string), default(string));
        }
        static void Realize(IVsPackageInstaller installer)
        {
            installer.InstallPackage(default(string), default(Project), default(string), default(string), default(bool));
            installer.InstallPackage(default(string), default(Project), default(string), default(Version), default(bool));
            installer.InstallPackagesFromRegistryRepository(default(string), default(bool), default(bool), default(Project), default(IDictionary<string, string>));
            installer.InstallPackagesFromRegistryRepository(default(string), default(bool), default(bool), default(bool), default(Project), default(IDictionary<string, string>));
            installer.InstallPackagesFromVSExtensionRepository(default(string), default(bool), default(bool), default(Project), default(IDictionary<string, string>));
            installer.InstallPackagesFromVSExtensionRepository(default(string), default(bool), default(bool), default(bool), default(Project), default(IDictionary<string, string>));
        }
        static void Realize(IVsPackageInstallerEvents installerEvents)
        {
            installerEvents.PackageInstalled += metadata => { };
            installerEvents.PackageInstalling += metadata => { };
            installerEvents.PackageReferenceAdded += metadata => { };
            installerEvents.PackageReferenceRemoved += metadata => { };
            installerEvents.PackageUninstalled += metadata => { };
            installerEvents.PackageUninstalling += metadata => { };
        }
        static void Realize(IVsPackageUninstaller uninstaller)
        {
            uninstaller.UninstallPackage(default(Project), default(string), default(bool));
        }

        public static Regsvr32Executor NewRegsvr32Executor(this IFixture fixture)
        {
            var regsvr32Executor = new Regsvr32Executor();
            // Don't use mock because you have to write the logic same as `EnvironmentRepository` if using it.
            regsvr32Executor.EnvironmentRepository = new EnvironmentRepository();
            return regsvr32Executor;
        }
    }
}
