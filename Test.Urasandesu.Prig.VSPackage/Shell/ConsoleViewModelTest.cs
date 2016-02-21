/* 
 * File: ConsoleViewModelTest.cs
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



using Microsoft.Win32;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Urasandesu.Prig.VSPackage;
using Urasandesu.Prig.VSPackage.Models;
using Urasandesu.Prig.VSPackage.Shell;

namespace Test.Urasandesu.Prig.VSPackage.Shell
{
    [TestFixture]
    class ConsoleViewModelTest
    {
        [Test]
        public void ReportProfilerStatusCheckingProgress_should_report_the_progress()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var pathOfInstalling = fixture.Create<string>();
            var profLoc = new ProfilerLocation(RegistryView.Registry64, pathOfInstalling);

            var vm = new ConsoleViewModel();

            // Act
            vm.ReportProfilerStatusCheckingProgress(profLoc);


            // Assert
            Assert.That(vm.Message.Value, Is.StringMatching(pathOfInstalling));
        }



        [Test]
        public void ReportNuGetPackageCreatingProgress_should_report_the_progress()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var pkgName = fixture.Create<string>();

            var vm = new ConsoleViewModel();

            // Act
            vm.ReportNuGetPackageCreatingProgress(pkgName);


            // Assert
            Assert.That(vm.Message.Value, Is.StringMatching(pkgName));
        }



        [Test]
        public void ReportNuGetSourceProcessingProgress_should_report_the_progress(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var path = fixture.Create<string>();
            var name = fixture.Create<string>();

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportNuGetSourceProcessingProgress(path, name);


            // Assert
            Assert.That(vm.Message.Value, Is.StringMatching(string.Format("({0})|({1})", path, name)));
        }



        [Test]
        public void ReportEnvironmentVariableProcessingProgress_should_report_the_progress(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var name = fixture.Create<string>();
            var value = fixture.Create<string>();

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportEnvironmentVariableProcessingProgress(name, value);


            // Assert
            Assert.That(vm.Message.Value, Is.StringMatching(string.Format("({0})|({1})", name, value)));
        }



        [Test]
        public void ReportEnvironmentVariableProcessedProgress_should_report_the_progress(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportEnvironmentVariableProcessedProgress();


            // Assert
            Assert.IsNotNullOrEmpty(vm.Message.Value);
        }



        [Test]
        public void ReportProfilerProcessingProgress_should_report_the_progress(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var pathOfInstalling = fixture.Create<string>();
            var profLoc = new ProfilerLocation(RegistryView.Registry64, pathOfInstalling);

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportProfilerProcessingProgress(profLoc);


            // Assert
            Assert.That(vm.Message.Value, Is.StringMatching(pathOfInstalling));
        }



        [Test]
        public void ShowSkippedMachineWideProcessMessage_should_show_the_message(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc,
            [Values(SkippedReasons.AlreadyRegistered, SkippedReasons.Error)]
            SkippedReasons reason)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ShowSkippedMachineWideProcessMessage(reason);


            // Assert
            Assert.IsNotNullOrEmpty(vm.Message.Value);
        }



        [Test]
        public void EndSkippedMachineWideProcessProgress_should_report_the_progress(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc,
            [Values(SkippedReasons.AlreadyRegistered, SkippedReasons.Error)]
            SkippedReasons reason)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);
            vm.ShowSkippedMachineWideProcessMessage(reason);

            // Act
            vm.EndSkippedMachineWideProcessProgress(reason);


            // Assert
            Assert.IsNotNullOrEmpty(vm.Message.Value);
            Assert.AreEqual(10 + (int)reason, vm.ExitCode.Value);
        }



        [Test]
        public void EndCompletedMachineWideProcessProgress_should_report_the_progress(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new ConsoleViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);
            vm.ShowCompletedMachineWideProcessMessage();

            // Act
            vm.EndCompletedMachineWideProcessProgress();


            // Assert
            Assert.IsNotNullOrEmpty(vm.Message.Value);
            Assert.AreEqual(0, vm.ExitCode.Value);
        }



        [Test]
        public void ShowCurrentConsoleHasNotBeenElevatedYetMessage_should_show_the_message()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new ConsoleViewModel();

            // Act
            vm.ShowCurrentConsoleHasNotBeenElevatedYetMessage();


            // Assert
            Assert.IsNotNullOrEmpty(vm.Message.Value);
        }
    }
}
