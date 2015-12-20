/* 
 * File: PrigPackageViewModelTest.cs
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using Moq;
using NuGet.VisualStudio;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using System;
using System.Collections;
using Urasandesu.Prig.VSPackage;
using Urasandesu.Prig.VSPackage.Infrastructure;

namespace Test.Urasandesu.Prig.VSPackage
{
    [TestFixture]
    class PrigPackageViewModelTest
    {
        [TestCaseSource(typeof(TestSourceForSetToCurrentProjectIfSupported), "TestCases")]
        [Test]
        public Project SetToCurrentProjectIfSupported_(IFixture fixture)
        {
            // Arrange
            var vm = new PrigPackageViewModel();

            // Act
            vm.SetToCurrentProjectIfSupported(fixture.Freeze<Project>());

            // Assert
            fixture.Freeze<Mock<Project>>().VerifyAll();
            return vm.CurrentProject.Value;
        }

        class TestSourceForSetToCurrentProjectIfSupported
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return should_set_specified_project_if_CSharpProject_is_passed();
                    yield return should_set_specified_project_if_FSharpProject_is_passed();
                    yield return should_set_specified_project_if_VBProject_is_passed();
                    yield return should_set_null_if_another_project_is_passed();
                    yield return should_set_null_if_null_is_passed();
                }
            }

            #region static TestCaseData should_set_specified_project_if_CSharpProject_is_passed()
            static TestCaseData should_set_specified_project_if_CSharpProject_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var proj = default(Project);
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    m.Setup(_ => _.Kind).Returns(VSConstantsAlternative.UICONTEXT.CSharpProject_string);
                    proj = m.Object;
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(proj).SetName("should_set_specified_project_if_CSharpProject_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_specified_project_if_FSharpProject_is_passed()
            static TestCaseData should_set_specified_project_if_FSharpProject_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var proj = default(Project);
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    m.Setup(_ => _.Kind).Returns(VSConstantsAlternative.UICONTEXT.FSharpProject_string);
                    proj = m.Object;
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(proj).SetName("should_set_specified_project_if_FSharpProject_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_specified_project_if_VBProject_is_passed()
            static TestCaseData should_set_specified_project_if_VBProject_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var proj = default(Project);
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    m.Setup(_ => _.Kind).Returns(VSConstantsAlternative.UICONTEXT.VBProject_string);
                    proj = m.Object;
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(proj).SetName("should_set_specified_project_if_VBProject_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_null_if_another_project_is_passed()
            static TestCaseData should_set_null_if_another_project_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    m.Setup(_ => _.Kind).Returns(Guid.NewGuid().ToString("B"));
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(null).SetName("should_set_null_if_another_project_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_null_if_null_is_passed()
            static TestCaseData should_set_null_if_null_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                fixture.Inject(default(Project));
                return new TestCaseData(fixture).Returns(null).SetName("should_set_null_if_null_is_passed");
            }
            #endregion
        }



        [TestCaseSource(typeof(TestSourceForSetEditPrigIndirectionSettingsCommandVisibility), "TestCases")]
        [Test]
        public bool SetEditPrigIndirectionSettingsCommandVisibility_(IFixture fixture)
        {
            // Arrange
            var vm = new PrigPackageViewModel();

            // Act
            vm.SetEditPrigIndirectionSettingsCommandVisibility(fixture.Freeze<ProjectItem>());

            // Assert
            fixture.Freeze<Mock<ProjectItem>>().VerifyAll();
            return vm.IsEditPrigIndirectionSettingsCommandVisible.Value;
        }

        class TestSourceForSetEditPrigIndirectionSettingsCommandVisibility
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return should_set_true_if_Stub_Settings_File_is_passed();
                    yield return should_set_true_if_Stub_Settings_File_that_has_letter_case_unmatch_extension_is_passed();
                    yield return should_set_false_if_another_project_item_is_passed();
                    yield return should_set_false_if_null_is_passed();
                }
            }

            #region static TestCaseData should_set_true_if_Stub_Settings_File_is_passed()
            static TestCaseData should_set_true_if_Stub_Settings_File_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<ProjectItem>(MockBehavior.Strict);
                    m.Setup(_ => _.Name).Returns(fixture.Create<string>() + ".prig");
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(true).SetName("should_set_true_if_Stub_Settings_File_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_true_if_Stub_Settings_File_that_has_letter_case_unmatch_extension_is_passed()
            static TestCaseData should_set_true_if_Stub_Settings_File_that_has_letter_case_unmatch_extension_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<ProjectItem>(MockBehavior.Strict);
                    m.Setup(_ => _.Name).Returns(fixture.Create<string>() + ".pRIg");
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(true).SetName("should_set_true_if_Stub_Settings_File_that_has_letter_case_unmatch_extension_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_false_if_another_project_item_is_passed()
            static TestCaseData should_set_false_if_another_project_item_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<ProjectItem>(MockBehavior.Strict);
                    m.Setup(_ => _.Name).Returns(fixture.Create<string>() + "." + fixture.Create<string>().Substring(0, 3));
                    fixture.Inject(m);
                }
                return new TestCaseData(fixture).Returns(false).SetName("should_set_false_if_another_project_item_is_passed");
            }
            #endregion

            #region static TestCaseData should_set_false_if_null_is_passed()
            static TestCaseData should_set_false_if_null_is_passed()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                fixture.Inject(default(ProjectItem));
                return new TestCaseData(fixture).Returns(false).SetName("should_set_false_if_null_is_passed");
            }
            #endregion
        }



        [TestCaseSource(typeof(TestSourceForCanEnableTestAdapter), "TestCases")]
        [Test]
        public bool CanEnableTestAdapter_(bool isTestAdapterEnabled, Project currentProject, IFixture fixture)
        {
            // Arrange
            var vm = new PrigPackageViewModel();
            vm.IsTestAdapterEnabled.Value = isTestAdapterEnabled;
            vm.CurrentProject.Value = currentProject;

            // Act
            var result = vm.HasEnabledTestAdapter(fixture.Freeze<Project>());

            // Assert
            fixture.Freeze<Mock<Project>>().VerifyAll();
            return result;
        }

        class TestSourceForCanEnableTestAdapter
        {
            public static IEnumerable TestCases
            {
                get
                {
                    yield return should_return_true_when_test_adapter_has_been_enabled();
                    yield return should_return_false_when_test_adapter_has_not_been_enabled_because_another_project_is_selected();
                    yield return should_return_false_when_test_adapter_has_not_been_enabled_because_current_project_is_not_selected();
                    yield return should_return_false_when_test_adapter_has_not_been_enabled_because_test_adapter_is_not_enabled();
                }
            }

            #region static TestCaseData should_return_true_when_test_adapter_has_been_enabled()
            static TestCaseData should_return_true_when_test_adapter_has_been_enabled()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var proj = default(Project);
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    m.Setup(_ => _.Name).Returns(fixture.Create<string>());
                    proj = m.Object;
                    fixture.Inject(m);
                }
                return new TestCaseData(true, proj, fixture).Returns(true).SetName("should_return_true_when_test_adapter_has_been_enabled");
            }
            #endregion

            #region static TestCaseData should_return_false_when_test_adapter_has_not_been_enabled_because_another_project_is_selected()
            static TestCaseData should_return_false_when_test_adapter_has_not_been_enabled_because_another_project_is_selected()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                var proj = default(Project);
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    var calledCount = 0;
                    m.Setup(_ => _.Name).Returns(() => calledCount++ == 0 ? fixture.Create<string>() : fixture.Create<string>());
                    proj = m.Object;
                    fixture.Inject(m);
                }
                return new TestCaseData(true, proj, fixture).Returns(false).SetName("should_return_false_when_test_adapter_has_not_been_enabled_because_another_project_is_selected");
            }
            #endregion

            #region static TestCaseData should_return_false_when_test_adapter_has_not_been_enabled_because_current_project_is_not_selected()
            static TestCaseData should_return_false_when_test_adapter_has_not_been_enabled_because_current_project_is_not_selected()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    fixture.Inject(m);
                }
                return new TestCaseData(true, null, fixture).Returns(false).SetName("should_return_false_when_test_adapter_has_not_been_enabled_because_current_project_is_not_selected");
            }
            #endregion

            #region static TestCaseData should_return_false_when_test_adapter_has_not_been_enabled_because_test_adapter_is_not_enabled()
            static TestCaseData should_return_false_when_test_adapter_has_not_been_enabled_because_test_adapter_is_not_enabled()
            {
                var fixture = new Fixture().Customize(new AutoMoqCustomization());
                {
                    var m = new Mock<Project>(MockBehavior.Strict);
                    fixture.Inject(m);
                }
                return new TestCaseData(false, null, fixture).Returns(false).SetName("should_return_false_when_test_adapter_has_not_been_enabled_because_test_adapter_is_not_enabled");
            }
            #endregion
        }



        [Test]
        public void GetCurrentProjectOrException_should_return_current_project_if_it_is_set()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var curProj = default(Project);
            {
                var m = new Mock<Project>(MockBehavior.Strict);
                curProj = m.Object;
                fixture.Inject(m);
            }

            var vm = new PrigPackageViewModel();
            vm.CurrentProject.Value = curProj;


            // Act
            var result = vm.GetCurrentProjectOrException();


            // Assert
            Assert.AreSame(curProj, result);
        }

        [Test]
        public void GetCurrentProjectOrException_should_throw_InvalidOperationException_if_it_is_not_set()
        {
            // Arrange
            var vm = new PrigPackageViewModel();
            vm.CurrentProject.Value = null;


            // Act, Assert
            Assert.Throws<InvalidOperationException>(() => vm.GetCurrentProjectOrException());
        }



        [Test]
        public void ReportProfilerStatusCheckingProgress_should_report_the_progress_to_statusbar()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var pathOfInstalling = fixture.Create<string>();
            var profLoc = new ProfilerLocation(RegistryView.Registry64, pathOfInstalling);

            var vm = new PrigPackageViewModel();

            // Act
            vm.ReportProfilerStatusCheckingProgress(prog, profLoc);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(pathOfInstalling));
        }



        [Test]
        public void ReportNuGetPackageCreatingProgress_should_report_the_progress_to_statusbar()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var pkgName = fixture.Create<string>();

            var vm = new PrigPackageViewModel();

            // Act
            vm.ReportNuGetPackageCreatingProgress(prog, pkgName);


            // Assert
            Assert.AreEqual(prog, vm.Statusbar.ProgressState.Value.Value);
            Assert.That(vm.Statusbar.ProgressState.Value.Label, Is.StringMatching(pkgName));
        }



        [Test]
        public void ReportNuGetSourceProcessingProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var path = fixture.Create<string>();
            var name = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportNuGetSourceProcessingProgress(prog, path, name);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(string.Format("({0})|({1})", path, name)));
        }



        [Test]
        public void ReportEnvironmentVariableProcessingProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var name = fixture.Create<string>();
            var value = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportEnvironmentVariableProcessingProgress(prog, name, value);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(string.Format("({0})|({1})", name, value)));
        }



        [Test]
        public void ReportEnvironmentVariableProcessedProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportEnvironmentVariableProcessedProgress(prog);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.IsNotNullOrEmpty(progState.Label);
        }



        [Test]
        public void ReportProfilerProcessingProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var pathOfInstalling = fixture.Create<string>();
            var profLoc = new ProfilerLocation(RegistryView.Registry64, pathOfInstalling);

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportProfilerProcessingProgress(prog, profLoc);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(pathOfInstalling));
        }



        [Test]
        public void ReportDefaultSourceProcessingProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var pkgName = fixture.Create<string>();
            var src = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ReportDefaultSourceProcessingProgress(prog, pkgName, src);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(string.Format("({0})|({1})", pkgName, src)));
        }



        [Test]
        public void ShowSkippedMachineWideProcessMessage_should_show_the_message_to_message_box(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc,
            [Values(SkippedReasons.AlreadyRegistered, SkippedReasons.CanceledByUser)]
            SkippedReasons reason)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.ShowSkippedMachineWideProcessMessage(reason);


            // Assert
            var msgBox = vm.MessageBoxParameter.Value;
            Assert.AreEqual(OLEMSGBUTTON.OLEMSGBUTTON_OK, msgBox.Button);
            Assert.AreEqual(OLEMSGICON.OLEMSGICON_INFO, msgBox.Icon);
            Assert.IsNotNullOrEmpty(msgBox.Text);
        }



        [Test]
        public void EndSkippedMachineWideProcessProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc,
            [Values(SkippedReasons.AlreadyRegistered, SkippedReasons.CanceledByUser)]
            SkippedReasons reason)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.EndSkippedMachineWideProcessProgress(reason);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(0u, progState.Value);
            Assert.IsNullOrEmpty(progState.Label);
            Assert.IsNotNullOrEmpty(vm.Statusbar.Text.Value);
        }



        [Test]
        public void ConfirmRestartingVisualStudioToTakeEffect_should_show_the_message_to_message_box(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc,
            [Values(VSConstants.MessageBoxResult.IDYES, VSConstants.MessageBoxResult.IDNO)]
            VSConstants.MessageBoxResult response)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);
            vm.MessageBoxParameter.Subscribe(_ => _.Result = response);

            // Act
            var result = vm.ConfirmRestartingVisualStudioToTakeEffect();

            // Assert
            Assert.AreEqual(response == VSConstants.MessageBoxResult.IDYES, result);
            var msgBox = vm.MessageBoxParameter.Value;
            Assert.AreEqual(OLEMSGBUTTON.OLEMSGBUTTON_YESNO, msgBox.Button);
            Assert.AreEqual(OLEMSGICON.OLEMSGICON_WARNING, msgBox.Icon);
            Assert.IsNotNullOrEmpty(msgBox.Text);
        }



        [Test]
        public void EndCompletedMachineWideProcessProgress_should_report_the_progress_to_statusbar(
            [Values(MachineWideProcesses.Installing, MachineWideProcesses.Uninstalling)]
            MachineWideProcesses mwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new PrigPackageViewModel();
            vm.BeginMachineWideProcessProgress(mwProc);

            // Act
            vm.EndCompletedMachineWideProcessProgress();


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(0u, progState.Value);
            Assert.IsNullOrEmpty(progState.Label);
            Assert.IsNotNullOrEmpty(vm.Statusbar.Text.Value);
        }



        [Test]
        public void ShowVisualStudioHasNotBeenElevatedYetMessage_should_show_the_message_to_message_box()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var vm = new PrigPackageViewModel();

            // Act
            vm.ShowVisualStudioHasNotBeenElevatedYetMessage();


            // Assert
            var msgBox = vm.MessageBoxParameter.Value;
            Assert.AreEqual(OLEMSGBUTTON.OLEMSGBUTTON_OK, msgBox.Button);
            Assert.AreEqual(OLEMSGICON.OLEMSGICON_INFO, msgBox.Icon);
            Assert.IsNotNullOrEmpty(msgBox.Text);
        }



        [Test]
        public void ReportPackagePreparingProgress_should_report_the_progress_to_statusbar()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();

            var vm = new PrigPackageViewModel();

            // Act
            vm.ReportPackagePreparingProgress(prog);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.IsNotNullOrEmpty(progState.Label);
        }



        [Test]
        public void ReportPackageInstallingProgress_should_report_the_progress_to_statusbar()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var id = fixture.Create<string>();
            {
                var m = new Mock<IVsPackageMetadata>(MockBehavior.Strict);
                m.Setup(_ => _.Id).Returns(id);
                fixture.Inject(m);
            }

            var vm = new PrigPackageViewModel();

            // Act
            vm.ReportPackageInstallingProgress(prog, fixture.Freeze<IVsPackageMetadata>());


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(id));
        }



        [Test]
        public void ReportPackageInstalledProgress_should_report_the_progress_to_statusbar()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var id = fixture.Create<string>();
            {
                var m = new Mock<IVsPackageMetadata>(MockBehavior.Strict);
                m.Setup(_ => _.Id).Returns(id);
                fixture.Inject(m);
            }

            var vm = new PrigPackageViewModel();

            // Act
            vm.ReportPackageInstalledProgress(prog, fixture.Freeze<IVsPackageMetadata>());


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(id));
        }



        [Test]
        public void ReportPackageReferenceAddedProgress_should_report_the_progress_to_statusbar()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var id = fixture.Create<string>();
            {
                var m = new Mock<IVsPackageMetadata>(MockBehavior.Strict);
                m.Setup(_ => _.Id).Returns(id);
                fixture.Inject(m);
            }

            var vm = new PrigPackageViewModel();

            // Act
            vm.ReportPackageReferenceAddedProgress(prog, fixture.Freeze<IVsPackageMetadata>());


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.That(progState.Label, Is.StringMatching(id));
        }



        [Test]
        public void ReportProcessingProjectWideProcessProgress_should_report_the_progress_to_statusbar(
            [Values(
                ProjectWideProcesses.PrigAssemblyAdding,
                ProjectWideProcesses.PrigIndirectionSettingsEditing,
                ProjectWideProcesses.PrigAssemblyRemoving,
                ProjectWideProcesses.TestAdapterEnabling,
                ProjectWideProcesses.TestAdapterDisabling)]
            ProjectWideProcesses pwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var prog = fixture.Create<uint>();
            var include = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginProjectWideProcessProgress(pwProc);


            // Act
            vm.ReportProcessingProjectWideProcessProgress(prog, include);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(prog, progState.Value);
            Assert.IsNotNullOrEmpty(progState.Label);
        }



        [Test]
        public void EndCompletedProjectWideProcessProgress_should_report_the_progress_to_statusbar(
            [Values(
                ProjectWideProcesses.PrigAssemblyAdding,
                ProjectWideProcesses.PrigIndirectionSettingsEditing,
                ProjectWideProcesses.PrigAssemblyRemoving,
                ProjectWideProcesses.TestAdapterEnabling,
                ProjectWideProcesses.TestAdapterDisabling)]
            ProjectWideProcesses pwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var include = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginProjectWideProcessProgress(pwProc);


            // Act
            vm.EndCompletedProjectWideProcessProgress(include);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(0u, progState.Value);
            Assert.IsNullOrEmpty(progState.Label);
            Assert.IsNotNullOrEmpty(vm.Statusbar.Text.Value);
        }



        [Test]
        public void ShowCompletedProjectWideProcessMessage_should_report_the_progress_to_statusbar(
            [Values(
                ProjectWideProcesses.PrigAssemblyAdding,
                ProjectWideProcesses.PrigIndirectionSettingsEditing,
                ProjectWideProcesses.PrigAssemblyRemoving,
                ProjectWideProcesses.TestAdapterEnabling,
                ProjectWideProcesses.TestAdapterDisabling)]
            ProjectWideProcesses pwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var include = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginProjectWideProcessProgress(pwProc);


            // Act
            vm.ShowCompletedProjectWideProcessMessage(include);


            // Assert
            var msgBox = vm.MessageBoxParameter.Value;
            Assert.AreEqual(OLEMSGBUTTON.OLEMSGBUTTON_OK, msgBox.Button);
            Assert.AreEqual(OLEMSGICON.OLEMSGICON_INFO, msgBox.Icon);
            Assert.IsNotNullOrEmpty(msgBox.Text);
        }



        [Test]
        public void EndSkippedProjectWideProcessProgress_should_report_the_progress_to_statusbar(
            [Values(
                ProjectWideProcesses.PrigAssemblyAdding,
                ProjectWideProcesses.PrigIndirectionSettingsEditing,
                ProjectWideProcesses.PrigAssemblyRemoving,
                ProjectWideProcesses.TestAdapterEnabling,
                ProjectWideProcesses.TestAdapterDisabling)]
            ProjectWideProcesses pwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var include = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginProjectWideProcessProgress(pwProc);


            // Act
            vm.EndSkippedProjectWideProcessProgress(SkippedReasons.NotRegisteredYet, include);


            // Assert
            var progState = vm.Statusbar.ProgressState.Value;
            Assert.AreEqual(0u, progState.Value);
            Assert.IsNullOrEmpty(progState.Label);
            Assert.IsNotNullOrEmpty(vm.Statusbar.Text.Value);
        }



        [Test]
        public void ShowSkippedProjectWideProcessMessage_should_report_the_progress_to_statusbar(
            [Values(
                ProjectWideProcesses.PrigAssemblyAdding,
                ProjectWideProcesses.PrigIndirectionSettingsEditing,
                ProjectWideProcesses.PrigAssemblyRemoving,
                ProjectWideProcesses.TestAdapterEnabling,
                ProjectWideProcesses.TestAdapterDisabling)]
            ProjectWideProcesses pwProc)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var include = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.BeginProjectWideProcessProgress(pwProc);


            // Act
            vm.ShowSkippedProjectWideProcessMessage(SkippedReasons.NotRegisteredYet, include);


            // Assert
            var msgBox = vm.MessageBoxParameter.Value;
            Assert.AreEqual(OLEMSGBUTTON.OLEMSGBUTTON_OK, msgBox.Button);
            Assert.AreEqual(OLEMSGICON.OLEMSGICON_WARNING, msgBox.Icon);
            Assert.IsNotNullOrEmpty(msgBox.Text);
        }



        [Test]
        public void ConfirmRemovingPrigAssembly_should_show_the_message_to_message_box(
            [Values(VSConstants.MessageBoxResult.IDYES, VSConstants.MessageBoxResult.IDNO)]
            VSConstants.MessageBoxResult response)
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());

            var deletionalInclude = fixture.Create<string>();

            var vm = new PrigPackageViewModel();
            vm.MessageBoxParameter.Subscribe(_ => _.Result = response);

            // Act
            var result = vm.ConfirmRemovingPrigAssembly(deletionalInclude);

            // Assert
            Assert.AreEqual(response == VSConstants.MessageBoxResult.IDYES, result);
            var msgBox = vm.MessageBoxParameter.Value;
            Assert.AreEqual(OLEMSGBUTTON.OLEMSGBUTTON_YESNO, msgBox.Button);
            Assert.AreEqual(OLEMSGICON.OLEMSGICON_QUERY, msgBox.Icon);
            Assert.IsNotNullOrEmpty(msgBox.Text);
        }
    }
}
